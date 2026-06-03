using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Nssol.Platypus.ApiModels.PipelineApiModels;
using Nssol.Platypus.Controllers.Util;
using Nssol.Platypus.DataAccess.Core;
using Nssol.Platypus.DataAccess.Repositories.Interfaces.TenantRepositories;
using Nssol.Platypus.Infrastructure;
using Nssol.Platypus.Logic.Interfaces;
using Nssol.Platypus.Models.TenantModels;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace Nssol.Platypus.Controllers.spa
{
    /// <summary>
    /// パイプラインを扱うためのAPI集
    /// </summary>
    [ApiController]
    [ApiVersion("1"), ApiVersion("2")]
    [Route("api/v{api-version:apiVersion}/pipelines")]
    public class PipelineController : PlatypusApiControllerBase
    {
        private readonly IPipelineRepository pipelineRepository;
        private readonly IPipelineLogic pipelineLogic;
        private readonly IUnitOfWork unitOfWork;

        public PipelineController(
            IPipelineRepository pipelineRepository,
            IPipelineLogic pipelineLogic,
            IUnitOfWork unitOfWork,
            IHttpContextAccessor accessor) : base(accessor)
        {
            this.pipelineRepository = pipelineRepository;
            this.pipelineLogic = pipelineLogic;
            this.unitOfWork = unitOfWork;
        }

        /// <summary>
        /// パイプライン一覧を取得
        /// </summary>
        [HttpGet]
        [Filters.PermissionFilter(MenuCode.Pipeline)]
        [ProducesResponseType(typeof(IEnumerable<IndexOutputModel>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetAll()
        {
            var pipelines = await pipelineRepository.GetAllWithChildrenAsync();
            var result = pipelines.Select(p => new IndexOutputModel
            {
                Id = p.Id,
                Name = p.Name,
                Memo = p.Memo,
                NodeCount = p.Nodes?.Count() ?? 0,
                CreatedAt = p.CreatedAt.ToString("yyyy/MM/dd HH:mm:ss"),
                ModifiedAt = p.ModifiedAt.ToString("yyyy/MM/dd HH:mm:ss"),
            });
            return JsonOK(result);
        }

        /// <summary>
        /// パイプライン詳細を取得
        /// </summary>
        [HttpGet("{id}")]
        [Filters.PermissionFilter(MenuCode.Pipeline)]
        [ProducesResponseType(typeof(DetailsOutputModel), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetById([FromRoute] long id)
        {
            var pipeline = await pipelineRepository.GetByIdWithChildrenAsync(id);
            if (pipeline == null)
            {
                return JsonNotFound($"Pipeline ID {id} is not found.");
            }

            var result = new DetailsOutputModel
            {
                Id = pipeline.Id,
                Name = pipeline.Name,
                Memo = pipeline.Memo,
                Nodes = pipeline.Nodes?.Select(n => new NodeOutputModel
                {
                    Id = n.Id,
                    Name = n.Name,
                    JobType = n.JobType,
                    PositionX = n.PositionX,
                    PositionY = n.PositionY,
                    JobParams = n.JobParams,
                }),
                Edges = pipeline.Edges?.Select(e => new EdgeOutputModel
                {
                    Id = e.Id,
                    SourceNodeId = e.SourceNodeId,
                    TargetNodeId = e.TargetNodeId,
                }),
                CreatedAt = pipeline.CreatedAt.ToString("yyyy/MM/dd HH:mm:ss"),
                ModifiedAt = pipeline.ModifiedAt.ToString("yyyy/MM/dd HH:mm:ss"),
            };
            return JsonOK(result);
        }

        /// <summary>
        /// パイプラインを作成
        /// </summary>
        [HttpPost]
        [Filters.PermissionFilter(MenuCode.Pipeline)]
        [ProducesResponseType(typeof(DetailsOutputModel), (int)HttpStatusCode.Created)]
        public IActionResult Create([FromBody] CreateInputModel model)
        {
            if (!ModelState.IsValid)
            {
                return JsonBadRequest("Invalid inputs.");
            }

            int nodeCount = model.Nodes?.Count() ?? 0;
            if (model.Edges != null)
            {
                foreach (var edgeInput in model.Edges)
                {
                    if (edgeInput.SourceNodeIndex < 0 || edgeInput.SourceNodeIndex >= nodeCount ||
                        edgeInput.TargetNodeIndex < 0 || edgeInput.TargetNodeIndex >= nodeCount)
                    {
                        return JsonBadRequest("Edge node index is out of range.");
                    }
                }
            }

            var pipeline = new Pipeline
            {
                Name = model.Name,
                Memo = model.Memo,
            };
            pipelineRepository.Add(pipeline);
            unitOfWork.Commit();

            var nodeList = new List<PipelineNode>();
            if (model.Nodes != null)
            {
                foreach (var nodeInput in model.Nodes)
                {
                    var node = new PipelineNode
                    {
                        PipelineId = pipeline.Id,
                        Name = nodeInput.Name,
                        JobType = nodeInput.JobType,
                        PositionX = nodeInput.PositionX,
                        PositionY = nodeInput.PositionY,
                        JobParams = nodeInput.JobParams,
                    };
                    pipelineRepository.AddNode(node);
                    nodeList.Add(node);
                }
                unitOfWork.Commit();
            }

            var edgeList = new List<PipelineEdge>();
            if (model.Edges != null)
            {
                foreach (var edgeInput in model.Edges)
                {
                    var edge = new PipelineEdge
                    {
                        PipelineId = pipeline.Id,
                        SourceNodeId = nodeList[edgeInput.SourceNodeIndex].Id,
                        TargetNodeId = nodeList[edgeInput.TargetNodeIndex].Id,
                    };
                    pipelineRepository.AddEdge(edge);
                    edgeList.Add(edge);
                }
                unitOfWork.Commit();
            }

            return JsonCreated(new DetailsOutputModel
            {
                Id = pipeline.Id,
                Name = pipeline.Name,
                Memo = pipeline.Memo,
                Nodes = nodeList.Select(n => new NodeOutputModel
                {
                    Id = n.Id,
                    Name = n.Name,
                    JobType = n.JobType,
                    PositionX = n.PositionX,
                    PositionY = n.PositionY,
                    JobParams = n.JobParams,
                }),
                Edges = edgeList.Select(e => new EdgeOutputModel
                {
                    Id = e.Id,
                    SourceNodeId = e.SourceNodeId,
                    TargetNodeId = e.TargetNodeId,
                }),
                CreatedAt = pipeline.CreatedAt.ToString("yyyy/MM/dd HH:mm:ss"),
                ModifiedAt = pipeline.ModifiedAt.ToString("yyyy/MM/dd HH:mm:ss"),
            });
        }

        /// <summary>
        /// パイプラインを更新
        /// </summary>
        [HttpPut("{id}")]
        [Filters.PermissionFilter(MenuCode.Pipeline)]
        [ProducesResponseType(typeof(DetailsOutputModel), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> Update([FromRoute] long id, [FromBody] EditInputModel model)
        {
            if (!ModelState.IsValid)
            {
                return JsonBadRequest("Invalid inputs.");
            }

            int nodeCount = model.Nodes?.Count() ?? 0;
            if (model.Edges != null)
            {
                foreach (var edgeInput in model.Edges)
                {
                    if (edgeInput.SourceNodeIndex < 0 || edgeInput.SourceNodeIndex >= nodeCount ||
                        edgeInput.TargetNodeIndex < 0 || edgeInput.TargetNodeIndex >= nodeCount)
                    {
                        return JsonBadRequest("Edge node index is out of range.");
                    }
                }
            }

            var pipeline = await pipelineRepository.GetByIdWithChildrenAsync(id);
            if (pipeline == null)
            {
                return JsonNotFound($"Pipeline ID {id} is not found.");
            }

            var hasRuns = await pipelineRepository.HasRunsAsync(id);
            if (hasRuns)
            {
                return JsonBadRequest("Cannot modify pipeline structure while execution history exists. Delete runs first.");
            }

            pipeline.Name = model.Name;
            pipeline.Memo = model.Memo;

            pipelineRepository.DeleteEdges(pipeline.Id);
            pipelineRepository.DeleteNodes(pipeline.Id);
            unitOfWork.Commit();

            var nodeList = new List<PipelineNode>();
            if (model.Nodes != null)
            {
                foreach (var nodeInput in model.Nodes)
                {
                    var node = new PipelineNode
                    {
                        PipelineId = pipeline.Id,
                        Name = nodeInput.Name,
                        JobType = nodeInput.JobType,
                        PositionX = nodeInput.PositionX,
                        PositionY = nodeInput.PositionY,
                        JobParams = nodeInput.JobParams,
                    };
                    pipelineRepository.AddNode(node);
                    nodeList.Add(node);
                }
                unitOfWork.Commit();
            }

            var edgeList = new List<PipelineEdge>();
            if (model.Edges != null)
            {
                foreach (var edgeInput in model.Edges)
                {
                    var edge = new PipelineEdge
                    {
                        PipelineId = pipeline.Id,
                        SourceNodeId = nodeList[edgeInput.SourceNodeIndex].Id,
                        TargetNodeId = nodeList[edgeInput.TargetNodeIndex].Id,
                    };
                    pipelineRepository.AddEdge(edge);
                    edgeList.Add(edge);
                }
                unitOfWork.Commit();
            }

            return JsonOK(new DetailsOutputModel
            {
                Id = pipeline.Id,
                Name = pipeline.Name,
                Memo = pipeline.Memo,
                Nodes = nodeList.Select(n => new NodeOutputModel
                {
                    Id = n.Id,
                    Name = n.Name,
                    JobType = n.JobType,
                    PositionX = n.PositionX,
                    PositionY = n.PositionY,
                    JobParams = n.JobParams,
                }),
                Edges = edgeList.Select(e => new EdgeOutputModel
                {
                    Id = e.Id,
                    SourceNodeId = e.SourceNodeId,
                    TargetNodeId = e.TargetNodeId,
                }),
                CreatedAt = pipeline.CreatedAt.ToString("yyyy/MM/dd HH:mm:ss"),
                ModifiedAt = pipeline.ModifiedAt.ToString("yyyy/MM/dd HH:mm:ss"),
            });
        }

        /// <summary>
        /// パイプラインを削除
        /// </summary>
        [HttpDelete("{id}")]
        [Filters.PermissionFilter(MenuCode.Pipeline)]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        public async Task<IActionResult> Delete([FromRoute] long id)
        {
            var pipeline = await pipelineRepository.GetByIdWithChildrenAsync(id);
            if (pipeline == null)
            {
                return JsonNotFound($"Pipeline ID {id} is not found.");
            }

            pipelineRepository.DeleteEdges(pipeline.Id);
            pipelineRepository.DeleteNodes(pipeline.Id);
            pipelineRepository.Delete(pipeline);
            unitOfWork.Commit();

            return JsonNoContent();
        }

        /// <summary>
        /// パイプラインを実行
        /// </summary>
        [HttpPost("{id}/runs")]
        [Filters.PermissionFilter(MenuCode.Pipeline)]
        [ProducesResponseType(typeof(RunOutputModel), (int)HttpStatusCode.Created)]
        public async Task<IActionResult> Run([FromRoute] long id)
        {
            var pipeline = await pipelineRepository.GetByIdWithChildrenAsync(id);
            if (pipeline == null)
            {
                return JsonNotFound($"Pipeline ID {id} is not found.");
            }

            if (pipeline.Nodes == null || !pipeline.Nodes.Any())
            {
                return JsonBadRequest("Pipeline has no nodes to execute.");
            }

            var run = new PipelineRun
            {
                PipelineId = pipeline.Id,
                Status = "Pending",
            };
            pipelineRepository.AddRun(run);
            unitOfWork.Commit();

            await pipelineLogic.ExecuteAsync(pipeline, run);

            var refreshedRun = await pipelineRepository.GetRunByIdAsync(run.Id);

            return JsonCreated(MapRunToOutput(refreshedRun));
        }

        /// <summary>
        /// パイプライン実行一覧を取得
        /// </summary>
        [HttpGet("{id}/runs")]
        [Filters.PermissionFilter(MenuCode.Pipeline)]
        [ProducesResponseType(typeof(IEnumerable<RunOutputModel>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetRuns([FromRoute] long id)
        {
            var runs = await pipelineRepository.GetRunsByPipelineIdAsync(id);
            var result = runs.Select(MapRunToOutput);
            return JsonOK(result);
        }

        /// <summary>
        /// パイプライン実行詳細を取得
        /// </summary>
        [HttpGet("runs/{runId}")]
        [Filters.PermissionFilter(MenuCode.Pipeline)]
        [ProducesResponseType(typeof(RunOutputModel), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetRunById([FromRoute] long runId)
        {
            var run = await pipelineRepository.GetRunByIdAsync(runId);
            if (run == null)
            {
                return JsonNotFound($"Pipeline Run ID {runId} is not found.");
            }
            return JsonOK(MapRunToOutput(run));
        }

        /// <summary>
        /// パイプライン実行のステップを完了にする（外部連携用）
        /// </summary>
        [HttpPost("runs/{runId}/steps/{stepId}/complete")]
        [Filters.PermissionFilter(MenuCode.Pipeline)]
        [ProducesResponseType(typeof(RunOutputModel), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> CompleteStep(
            [FromRoute] long runId,
            [FromRoute] long stepId,
            [FromQuery] string status = "Completed")
        {
            if (status != "Completed" && status != "Failed")
            {
                return JsonBadRequest("Status must be 'Completed' or 'Failed'.");
            }

            var run = await pipelineRepository.GetRunByIdAsync(runId);
            if (run == null)
            {
                return JsonNotFound($"Pipeline Run ID {runId} is not found.");
            }

            var step = run.Steps?.FirstOrDefault(s => s.Id == stepId);
            if (step == null)
            {
                return JsonNotFound($"Pipeline Run Step ID {stepId} is not found.");
            }

            if (step.Status != "Running")
            {
                return JsonBadRequest($"Step is not in Running state. Current status: {step.Status}");
            }

            step.Status = status;
            step.CompletedAt = System.DateTime.Now;
            unitOfWork.Commit();

            // DAGを進める
            await pipelineLogic.AdvanceAsync(run);

            var refreshedRun = await pipelineRepository.GetRunByIdAsync(runId);
            return JsonOK(MapRunToOutput(refreshedRun));
        }

        private RunOutputModel MapRunToOutput(PipelineRun run)
        {
            return new RunOutputModel
            {
                Id = run.Id,
                PipelineId = run.PipelineId,
                PipelineName = run.Pipeline?.Name,
                Status = run.Status,
                StartedAt = run.StartedAt?.ToString("yyyy/MM/dd HH:mm:ss"),
                CompletedAt = run.CompletedAt?.ToString("yyyy/MM/dd HH:mm:ss"),
                Memo = run.Memo,
                Steps = run.Steps?.Select(s => new RunStepOutputModel
                {
                    Id = s.Id,
                    PipelineNodeId = s.PipelineNodeId,
                    NodeName = s.PipelineNode?.Name,
                    JobType = s.PipelineNode?.JobType,
                    Status = s.Status,
                    TrainingHistoryId = s.TrainingHistoryId,
                    InferenceHistoryId = s.InferenceHistoryId,
                    PreprocessHistoryId = s.PreprocessHistoryId,
                    StartedAt = s.StartedAt?.ToString("yyyy/MM/dd HH:mm:ss"),
                    CompletedAt = s.CompletedAt?.ToString("yyyy/MM/dd HH:mm:ss"),
                }),
                CreatedAt = run.CreatedAt.ToString("yyyy/MM/dd HH:mm:ss"),
            };
        }
    }
}
