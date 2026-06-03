using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Nssol.Platypus.ApiModels.ModelApiModels;
using Nssol.Platypus.Controllers.Util;
using Nssol.Platypus.DataAccess.Core;
using Nssol.Platypus.DataAccess.Repositories.Interfaces.TenantRepositories;
using Nssol.Platypus.Models.TenantModels;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace Nssol.Platypus.Controllers.spa
{
    /// <summary>
    /// モデルレジストリを扱うためのAPI集
    /// </summary>
    [ApiController]
    [ApiVersion("1"), ApiVersion("2")]
    [Route("api/v{api-version:apiVersion}/models")]
    public class ModelController : PlatypusApiControllerBase
    {
        private readonly IModelRepository modelRepository;
        private readonly IModelVersionRepository modelVersionRepository;
        private readonly ITrainingHistoryRepository trainingHistoryRepository;
        private readonly IUnitOfWork unitOfWork;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public ModelController(
            IModelRepository modelRepository,
            IModelVersionRepository modelVersionRepository,
            ITrainingHistoryRepository trainingHistoryRepository,
            IUnitOfWork unitOfWork,
            IHttpContextAccessor accessor) : base(accessor)
        {
            this.modelRepository = modelRepository;
            this.modelVersionRepository = modelVersionRepository;
            this.trainingHistoryRepository = trainingHistoryRepository;
            this.unitOfWork = unitOfWork;
        }

        /// <summary>
        /// モデル一覧を取得
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<IndexOutputModel>), (int)HttpStatusCode.OK)]
        public IActionResult GetAll()
        {
            var models = modelRepository.GetAllModels();
            var result = models.Select(m => new IndexOutputModel(m)).ToList();
            return JsonOK(result);
        }

        /// <summary>
        /// 指定されたIDのモデル詳細を取得
        /// </summary>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(DetailsOutputModel), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetById([FromRoute] long id)
        {
            var model = await modelRepository.GetIncludeAllAsync(id);
            if (model == null)
            {
                return JsonNotFound($"Model ID {id} is not found.");
            }
            return JsonOK(new DetailsOutputModel(model));
        }

        /// <summary>
        /// モデルを新規作成
        /// </summary>
        [HttpPost]
        [ProducesResponseType(typeof(IndexOutputModel), (int)HttpStatusCode.Created)]
        public async Task<IActionResult> Create([FromBody] CreateInputModel model)
        {
            if (!ModelState.IsValid)
            {
                return JsonBadRequest("Invalid inputs.");
            }

            if (await modelRepository.ExistsByNameAsync(model.Name))
            {
                return JsonConflict($"Model name '{model.Name}' already exists.");
            }

            var newModel = new Model
            {
                Name = model.Name,
                Description = model.Description,
            };

            modelRepository.Add(newModel);
            unitOfWork.Commit();

            return JsonCreated(new IndexOutputModel(newModel));
        }

        /// <summary>
        /// モデルを編集
        /// </summary>
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(IndexOutputModel), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> Edit([FromRoute] long id, [FromBody] EditInputModel model)
        {
            var existing = await modelRepository.GetByIdAsync(id);
            if (existing == null)
            {
                return JsonNotFound($"Model ID {id} is not found.");
            }

            if (model.Name != null)
            {
                existing.Name = model.Name;
            }
            if (model.Description != null)
            {
                existing.Description = model.Description;
            }

            modelRepository.Update(existing);
            unitOfWork.Commit();

            return JsonOK(new IndexOutputModel(existing));
        }

        /// <summary>
        /// モデルを削除
        /// </summary>
        [HttpDelete("{id}")]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        public async Task<IActionResult> Delete([FromRoute] long id)
        {
            var existing = await modelRepository.GetByIdAsync(id);
            if (existing == null)
            {
                return JsonNotFound($"Model ID {id} is not found.");
            }

            modelRepository.Delete(existing);
            unitOfWork.Commit();

            return JsonNoContent();
        }

        /// <summary>
        /// 指定モデルのバージョン一覧を取得
        /// </summary>
        [HttpGet("{id}/versions")]
        [ProducesResponseType(typeof(IEnumerable<VersionOutputModel>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetVersions([FromRoute] long id)
        {
            var model = await modelRepository.GetByIdAsync(id);
            if (model == null)
            {
                return JsonNotFound($"Model ID {id} is not found.");
            }

            var versions = modelVersionRepository.GetAllByModelId(id);
            var result = versions.Select(v => new VersionOutputModel(v)).ToList();
            return JsonOK(result);
        }

        /// <summary>
        /// 指定モデルにバージョンを追加
        /// </summary>
        [HttpPost("{id}/versions")]
        [ProducesResponseType(typeof(VersionOutputModel), (int)HttpStatusCode.Created)]
        public async Task<IActionResult> CreateVersion([FromRoute] long id, [FromBody] CreateVersionInputModel input)
        {
            var model = await modelRepository.GetByIdAsync(id);
            if (model == null)
            {
                return JsonNotFound($"Model ID {id} is not found.");
            }

            if (input.TrainingHistoryId.HasValue)
            {
                var training = await trainingHistoryRepository.GetByIdAsync(input.TrainingHistoryId.Value);
                if (training == null)
                {
                    return JsonNotFound($"Training history ID {input.TrainingHistoryId.Value} is not found.");
                }
            }

            int latestVersion = await modelVersionRepository.GetLatestVersionNumberAsync(id);

            var newVersion = new ModelVersion
            {
                ModelId = id,
                Version = latestVersion + 1,
                TrainingHistoryId = input.TrainingHistoryId,
                Accuracy = input.Accuracy,
                Status = input.Status ?? "none",
                Description = input.Description,
            };

            modelVersionRepository.Add(newVersion);
            unitOfWork.Commit();

            return JsonCreated(new VersionOutputModel(newVersion));
        }

        /// <summary>
        /// バージョンのステータスを更新
        /// </summary>
        [HttpPut("{id}/versions/{versionId}")]
        [ProducesResponseType(typeof(VersionOutputModel), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> EditVersion([FromRoute] long id, [FromRoute] long versionId, [FromBody] EditVersionInputModel input)
        {
            var version = await modelVersionRepository.GetIncludeAllAsync(versionId);
            if (version == null || version.ModelId != id)
            {
                return JsonNotFound($"Model version ID {versionId} is not found in model {id}.");
            }

            if (input.Status != null)
            {
                version.Status = input.Status;
            }
            if (input.Description != null)
            {
                version.Description = input.Description;
            }

            modelVersionRepository.Update(version);
            unitOfWork.Commit();

            return JsonOK(new VersionOutputModel(version));
        }

        /// <summary>
        /// バージョンを削除
        /// </summary>
        [HttpDelete("{id}/versions/{versionId}")]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        public async Task<IActionResult> DeleteVersion([FromRoute] long id, [FromRoute] long versionId)
        {
            var version = await modelVersionRepository.GetByIdAsync(versionId);
            if (version == null || version.ModelId != id)
            {
                return JsonNotFound($"Model version ID {versionId} is not found in model {id}.");
            }

            modelVersionRepository.Delete(version);
            unitOfWork.Commit();

            return JsonNoContent();
        }
    }
}
