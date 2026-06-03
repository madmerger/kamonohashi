using Nssol.Platypus.DataAccess.Core;
using Nssol.Platypus.DataAccess.Repositories.Interfaces.TenantRepositories;
using Nssol.Platypus.Logic.Interfaces;
using Nssol.Platypus.Models.TenantModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Nssol.Platypus.Logic
{
    /// <summary>
    /// パイプライン操作のロジック
    /// </summary>
    public class PipelineLogic : PlatypusLogicBase, IPipelineLogic
    {
        private readonly IPipelineRepository pipelineRepository;
        private readonly IUnitOfWork unitOfWork;

        public PipelineLogic(
            IPipelineRepository pipelineRepository,
            IUnitOfWork unitOfWork,
            ICommonDiLogic commonDiLogic) : base(commonDiLogic)
        {
            this.pipelineRepository = pipelineRepository;
            this.unitOfWork = unitOfWork;
        }

        /// <inheritdoc/>
        public async Task ExecuteAsync(Pipeline pipeline, PipelineRun run)
        {
            run.Status = "Running";
            run.StartedAt = DateTime.Now;

            // DAGのルートノード（入力辺がないノード）を特定
            var allNodeIds = pipeline.Nodes.Select(n => n.Id).ToHashSet();
            var nodesWithIncomingEdge = pipeline.Edges?.Select(e => e.TargetNodeId).ToHashSet() ?? new HashSet<long>();
            var rootNodes = pipeline.Nodes.Where(n => !nodesWithIncomingEdge.Contains(n.Id)).ToList();

            // 各ノード用のステップを作成
            foreach (var node in pipeline.Nodes)
            {
                var step = new PipelineRunStep
                {
                    PipelineRunId = run.Id,
                    PipelineNodeId = node.Id,
                    Status = "Pending",
                };
                pipelineRepository.AddRunStep(step);
            }
            unitOfWork.Commit();

            // ルートノードのステップをRunningに変更
            var refreshedRun = await pipelineRepository.GetRunByIdAsync(run.Id);
            if (refreshedRun != null)
            {
                foreach (var rootNode in rootNodes)
                {
                    var step = refreshedRun.Steps.FirstOrDefault(s => s.PipelineNodeId == rootNode.Id);
                    if (step != null)
                    {
                        step.Status = "Running";
                        step.StartedAt = DateTime.Now;
                    }
                }
                unitOfWork.Commit();
            }

            LogInformation($"Pipeline {pipeline.Id} execution started. Run ID: {run.Id}, Root nodes: {rootNodes.Count}");
        }

        /// <inheritdoc/>
        public async Task AdvanceAsync(PipelineRun run)
        {
            var pipeline = await pipelineRepository.GetByIdWithChildrenAsync(run.PipelineId);
            if (pipeline == null) return;

            var refreshedRun = await pipelineRepository.GetRunByIdAsync(run.Id);
            if (refreshedRun == null) return;

            var completedNodeIds = refreshedRun.Steps
                .Where(s => s.Status == "Completed")
                .Select(s => s.PipelineNodeId)
                .ToHashSet();

            var failedSteps = refreshedRun.Steps.Where(s => s.Status == "Failed").ToList();
            if (failedSteps.Any())
            {
                // 失敗ステップがある場合、残りのPendingステップをSkippedにしてRunをFailedに
                foreach (var step in refreshedRun.Steps.Where(s => s.Status == "Pending"))
                {
                    step.Status = "Skipped";
                }
                refreshedRun.Status = "Failed";
                refreshedRun.CompletedAt = DateTime.Now;
                unitOfWork.Commit();
                return;
            }

            // 全ステップ完了チェック
            if (refreshedRun.Steps.All(s => s.Status == "Completed"))
            {
                refreshedRun.Status = "Completed";
                refreshedRun.CompletedAt = DateTime.Now;
                unitOfWork.Commit();
                return;
            }

            // DAGに従い、全依存が完了済みの次のノードを起動
            var edges = pipeline.Edges?.ToList() ?? new List<PipelineEdge>();
            foreach (var step in refreshedRun.Steps.Where(s => s.Status == "Pending"))
            {
                var incomingEdges = edges.Where(e => e.TargetNodeId == step.PipelineNodeId);
                bool allDependenciesCompleted = incomingEdges.All(e => completedNodeIds.Contains(e.SourceNodeId));
                if (allDependenciesCompleted)
                {
                    step.Status = "Running";
                    step.StartedAt = DateTime.Now;
                }
            }
            unitOfWork.Commit();
        }
    }
}
