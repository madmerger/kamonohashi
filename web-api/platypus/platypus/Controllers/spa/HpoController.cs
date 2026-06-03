using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Nssol.Platypus.ApiModels.HpoApiModels;
using Nssol.Platypus.Controllers.Util;
using Nssol.Platypus.DataAccess.Core;
using Nssol.Platypus.DataAccess.Repositories.Interfaces.TenantRepositories;
using Nssol.Platypus.Infrastructure;
using Nssol.Platypus.Logic.Interfaces;
using Nssol.Platypus.Models.TenantModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace Nssol.Platypus.Controllers.spa
{
    /// <summary>
    /// HPO（ハイパーパラメータ最適化）を扱うためのAPI集
    /// </summary>
    [ApiController]
    [ApiVersion("1"), ApiVersion("2")]
    [Route("api/v{api-version:apiVersion}/hpo")]
    public class HpoController : PlatypusApiControllerBase
    {
        private readonly IHpoJobRepository hpoJobRepository;
        private readonly IHpoTrialRepository hpoTrialRepository;
        private readonly IHpoLogic hpoLogic;
        private readonly IUnitOfWork unitOfWork;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public HpoController(
            IHpoJobRepository hpoJobRepository,
            IHpoTrialRepository hpoTrialRepository,
            IHpoLogic hpoLogic,
            IUnitOfWork unitOfWork,
            IHttpContextAccessor accessor) : base(accessor)
        {
            this.hpoJobRepository = hpoJobRepository;
            this.hpoTrialRepository = hpoTrialRepository;
            this.hpoLogic = hpoLogic;
            this.unitOfWork = unitOfWork;
        }

        /// <summary>
        /// 全HPOジョブのIDと名前を取得
        /// </summary>
        [HttpGet("simple")]
        [Filters.PermissionFilter(MenuCode.Hpo)]
        [ProducesResponseType(typeof(IEnumerable<HpoSimpleOutputModel>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetAllSimple()
        {
            var jobs = await hpoJobRepository.GetAllNameAsync();
            return JsonOK(jobs.Select(j => new HpoSimpleOutputModel { Id = j.Id, Name = j.Name }));
        }

        /// <summary>
        /// 全HPOジョブを取得（ページング対応）
        /// </summary>
        [HttpGet]
        [Filters.PermissionFilter(MenuCode.Hpo)]
        [ProducesResponseType(typeof(IEnumerable<IndexOutputModel>), (int)HttpStatusCode.OK)]
        public IActionResult GetAll([FromQuery] int? perPage, [FromQuery] int page = 1, bool withTotal = false)
        {
            var data = hpoJobRepository.GetAllIncludeDataSetWithOrdering().AsEnumerable();

            if (withTotal)
            {
                int total = data.Count();
                SetTotalCountToHeader(total);
            }

            int pageCount = (perPage.HasValue && perPage.Value < 1000) ? perPage.Value : 1000;
            data = data.Paging(page, pageCount);

            return JsonOK(data.ToList().Select(j => new IndexOutputModel(j)));
        }

        /// <summary>
        /// 指定されたIDのHPOジョブ詳細を取得
        /// </summary>
        [HttpGet("{id}")]
        [Filters.PermissionFilter(MenuCode.Hpo)]
        [ProducesResponseType(typeof(DetailsOutputModel), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetById(long id)
        {
            var hpoJob = await hpoJobRepository.GetIncludeTrialsAsync(id);
            if (hpoJob == null)
            {
                return JsonNotFound($"HPO Job ID {id} is not found.");
            }

            return JsonOK(new DetailsOutputModel(hpoJob));
        }

        /// <summary>
        /// 新規HPOジョブを作成して実行開始する
        /// </summary>
        [HttpPost("run")]
        [Filters.PermissionFilter(MenuCode.Hpo)]
        [ProducesResponseType(typeof(IndexOutputModel), (int)HttpStatusCode.Created)]
        public async Task<IActionResult> Create([FromBody] CreateInputModel model)
        {
            if (!ModelState.IsValid)
            {
                return JsonBadRequest("Invalid inputs.");
            }

            // アルゴリズムのバリデーション
            var validAlgorithms = new[] { "grid", "random", "bayes" };
            if (!validAlgorithms.Contains(model.Algorithm?.ToLower()))
            {
                return JsonBadRequest("Algorithm must be one of: Grid, Random, Bayes.");
            }

            // 最適化方向のバリデーション
            var validDirections = new[] { "minimize", "maximize" };
            if (!validDirections.Contains(model.OptimizationDirection?.ToLower()))
            {
                return JsonBadRequest("OptimizationDirection must be one of: minimize, maximize.");
            }

            // 探索空間のバリデーション
            if (model.SearchSpace == null || model.SearchSpace.Count == 0)
            {
                return JsonBadRequest("SearchSpace must contain at least one parameter.");
            }

            foreach (var param in model.SearchSpace)
            {
                var validTypes = new[] { "int", "float", "categorical" };
                if (!validTypes.Contains(param.Type?.ToLower()))
                {
                    return JsonBadRequest($"Parameter '{param.Name}' has invalid type. Must be int, float, or categorical.");
                }

                if (param.Type?.ToLower() == "categorical" && (param.Values == null || param.Values.Count == 0))
                {
                    return JsonBadRequest($"Parameter '{param.Name}' is categorical but has no values.");
                }

                if ((param.Type?.ToLower() == "int" || param.Type?.ToLower() == "float") &&
                    (!param.Min.HasValue || !param.Max.HasValue))
                {
                    return JsonBadRequest($"Parameter '{param.Name}' must have Min and Max values.");
                }
            }

            // グリッドサーチの場合、Stepのバリデーション
            if (model.Algorithm.ToLower() == "grid")
            {
                foreach (var param in model.SearchSpace)
                {
                    if (param.Type?.ToLower() == "int" || param.Type?.ToLower() == "float")
                    {
                        if (!param.Step.HasValue || param.Step.Value <= 0)
                        {
                            return JsonBadRequest($"Parameter '{param.Name}' must have a positive Step value for grid search.");
                        }
                    }
                }
            }

            // GitIdの解決（未指定の場合はテナントのデフォルトを使用）
            long? gitId = model.GitModel.GitId ?? CurrentUserInfo.SelectedTenant.DefaultGit?.Id;
            if (!gitId.HasValue)
            {
                return JsonBadRequest("GitId is not specified and no default Git is configured for the tenant.");
            }

            // HPOジョブを作成
            var hpoJob = new HpoJob
            {
                Name = model.Name,
                Algorithm = model.Algorithm,
                SearchSpace = JsonConvert.SerializeObject(model.SearchSpace.Select(p => new HpoSearchSpaceParameter
                {
                    Name = p.Name,
                    Type = p.Type,
                    Min = p.Min,
                    Max = p.Max,
                    Step = p.Step,
                    Values = p.Values
                })),
                MaxTrials = model.MaxTrials.Value,
                ObjectiveMetric = model.ObjectiveMetric,
                OptimizationDirection = model.OptimizationDirection.ToLower(),
                DataSetId = model.DataSetId.Value,
                ModelGitId = gitId.Value,
                ModelRepository = model.GitModel.Repository,
                ModelRepositoryOwner = model.GitModel.Owner,
                ModelBranch = model.GitModel.Branch ?? "master",
                ModelCommitId = model.GitModel.CommitId,
                EntryPoint = model.EntryPoint,
                ContainerRegistryId = model.ContainerImage.RegistryId,
                ContainerImage = model.ContainerImage.Image,
                ContainerTag = model.ContainerImage.Tag,
                Cpu = model.Cpu.Value,
                Memory = model.Memory.Value,
                Gpu = model.Gpu.Value,
                Partition = model.Partition,
                Memo = model.Memo,
                Status = "Running",
                StartedAt = DateTime.Now
            };

            hpoJobRepository.Add(hpoJob);
            unitOfWork.Commit();

            // 初期トライアルを生成する
            await GenerateTrialsAsync(hpoJob);

            var result = await hpoJobRepository.GetIncludeTrialsAsync(hpoJob.Id);
            return JsonCreated(new IndexOutputModel(result));
        }

        /// <summary>
        /// HPOジョブを停止する
        /// </summary>
        [HttpPost("{id}/halt")]
        [Filters.PermissionFilter(MenuCode.Hpo)]
        [ProducesResponseType(typeof(IndexOutputModel), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> Halt(long id)
        {
            var hpoJob = await hpoJobRepository.GetIncludeTrialsAsync(id);
            if (hpoJob == null)
            {
                return JsonNotFound($"HPO Job ID {id} is not found.");
            }

            if (hpoJob.Status != "Running")
            {
                return JsonBadRequest($"HPO Job ID {id} is not running.");
            }

            await hpoLogic.StopHpoJobAsync(hpoJob);

            return JsonOK(new IndexOutputModel(hpoJob));
        }

        /// <summary>
        /// HPOジョブを削除する
        /// </summary>
        [HttpDelete("{id}")]
        [Filters.PermissionFilter(MenuCode.Hpo)]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        public async Task<IActionResult> Delete(long id)
        {
            var hpoJob = await hpoJobRepository.GetIncludeTrialsAsync(id);
            if (hpoJob == null)
            {
                return JsonNotFound($"HPO Job ID {id} is not found.");
            }

            if (hpoJob.Status == "Running")
            {
                return JsonBadRequest($"HPO Job ID {id} is still running. Please halt it first.");
            }

            // トライアルを削除
            if (hpoJob.Trials != null)
            {
                foreach (var trial in hpoJob.Trials.ToList())
                {
                    hpoTrialRepository.Delete(trial);
                }
            }

            hpoJobRepository.Delete(hpoJob);
            unitOfWork.Commit();

            return JsonNoContent();
        }

        /// <summary>
        /// トライアルのメトリクスを報告する（学習スクリプトから呼ばれる）
        /// </summary>
        [HttpPut("{id}/trials/{trialId}/metrics")]
        [Filters.PermissionFilter(MenuCode.Hpo)]
        [ProducesResponseType(typeof(TrialOutputModel), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> ReportMetrics(long id, long trialId, [FromBody] ReportMetricsInputModel model)
        {
            if (!ModelState.IsValid)
            {
                return JsonBadRequest("Invalid inputs.");
            }

            var hpoJob = await hpoJobRepository.GetByIdAsync(id);
            if (hpoJob == null)
            {
                return JsonNotFound($"HPO Job ID {id} is not found.");
            }

            var trial = await hpoTrialRepository.GetByIdAsync(trialId);
            if (trial == null || trial.HpoJobId != id)
            {
                return JsonNotFound($"Trial ID {trialId} is not found in HPO Job {id}.");
            }

            trial.MetricValue = model.MetricValue.Value;
            trial.Status = "Completed";
            trial.CompletedAt = DateTime.Now;

            unitOfWork.Commit();

            // 全トライアルが完了したかチェック
            await CheckAndCompleteHpoJobAsync(hpoJob);

            return JsonOK(new TrialOutputModel(trial));
        }

        /// <summary>
        /// トライアルを生成する
        /// ベイズ最適化の場合は初期バッチのみ生成し、残りはトライアル完了時に逐次生成する
        /// </summary>
        private async Task GenerateTrialsAsync(HpoJob hpoJob)
        {
            var existingTrials = hpoTrialRepository.GetByHpoJobId(hpoJob.Id).ToList();
            int startTrialNo = existingTrials.Count;

            // ベイズ最適化の場合は初期バッチ（最大3個）のみ生成
            int trialsToGenerate;
            if (hpoJob.Algorithm.ToLower() == "bayes")
            {
                trialsToGenerate = Math.Min(3, hpoJob.MaxTrials - startTrialNo);
            }
            else
            {
                trialsToGenerate = hpoJob.MaxTrials - startTrialNo;
            }

            for (int i = 0; i < trialsToGenerate; i++)
            {
                int trialNo = startTrialNo + i;
                var parameters = hpoLogic.GenerateNextParameters(hpoJob, existingTrials, trialNo);

                var trial = new HpoTrial
                {
                    TrialNo = trialNo,
                    HpoJobId = hpoJob.Id,
                    Parameters = JsonConvert.SerializeObject(parameters),
                    Status = "Pending",
                };

                hpoTrialRepository.Add(trial);
            }

            unitOfWork.Commit();
        }

        /// <summary>
        /// HPOジョブの完了チェックと次のトライアル生成
        /// </summary>
        private async Task CheckAndCompleteHpoJobAsync(HpoJob hpoJob)
        {
            var trials = hpoTrialRepository.GetByHpoJobId(hpoJob.Id).ToList();
            var completedCount = trials.Count(t => t.Status == "Completed" || t.Status == "Failed" || t.Status == "Cancelled");

            if (completedCount >= hpoJob.MaxTrials)
            {
                await hpoJobRepository.UpdateStatusAsync(hpoJob.Id, "Completed");
                hpoJob.CompletedAt = DateTime.Now;
                unitOfWork.Commit();
            }
            else if (hpoJob.Algorithm.ToLower() == "bayes" && hpoJob.Status == "Running")
            {
                // ベイズ最適化：完了済みトライアルの結果をもとに次のトライアルを逐次生成
                int totalGenerated = trials.Count;
                if (totalGenerated < hpoJob.MaxTrials)
                {
                    int trialNo = totalGenerated;
                    var parameters = hpoLogic.GenerateNextParameters(hpoJob, trials, trialNo);

                    var newTrial = new HpoTrial
                    {
                        TrialNo = trialNo,
                        HpoJobId = hpoJob.Id,
                        Parameters = JsonConvert.SerializeObject(parameters),
                        Status = "Pending",
                    };

                    hpoTrialRepository.Add(newTrial);
                    unitOfWork.Commit();
                }
            }
        }
    }

    /// <summary>
    /// メトリクス報告用入力モデル
    /// </summary>
    public class ReportMetricsInputModel
    {
        /// <summary>
        /// メトリクス値
        /// </summary>
        [System.ComponentModel.DataAnnotations.Required]
        public double? MetricValue { get; set; }
    }
}
