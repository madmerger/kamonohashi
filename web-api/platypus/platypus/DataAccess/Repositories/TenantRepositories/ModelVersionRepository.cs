using Microsoft.EntityFrameworkCore;
using Nssol.Platypus.DataAccess.Core;
using Nssol.Platypus.DataAccess.Repositories.Interfaces.TenantRepositories;
using Nssol.Platypus.Models.TenantModels;
using System.Linq;
using System.Threading.Tasks;

namespace Nssol.Platypus.DataAccess.Repositories.TenantRepositories
{
    /// <summary>
    /// モデルバージョンテーブルにアクセスするためのリポジトリクラス
    /// </summary>
    public class ModelVersionRepository : RepositoryForTenantBase<ModelVersion>, IModelVersionRepository
    {
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public ModelVersionRepository(CommonDbContext context, Microsoft.AspNetCore.Http.IHttpContextAccessor accessor)
            : base(context, accessor)
        {
        }

        /// <summary>
        /// 指定されたモデルIDのバージョン一覧を取得します。
        /// </summary>
        public IQueryable<ModelVersion> GetAllByModelId(long modelId)
        {
            return FindAll(v => v.ModelId == modelId)
                .Include(v => v.TrainingHistory)
                .OrderByDescending(v => v.Version);
        }

        /// <summary>
        /// 指定されたモデルIDの最新バージョン番号を取得します。
        /// </summary>
        public async Task<int> GetLatestVersionNumberAsync(long modelId)
        {
            var latest = await FindAll(v => v.ModelId == modelId)
                .OrderByDescending(v => v.Version)
                .FirstOrDefaultAsync();
            return latest?.Version ?? 0;
        }

        /// <summary>
        /// 指定されたバージョンIDのバージョンエンティティ（学習履歴を含む）を取得します。
        /// </summary>
        public async Task<ModelVersion> GetIncludeAllAsync(long id)
        {
            return await FindAll(v => v.Id == id)
                .Include(v => v.Model)
                .Include(v => v.TrainingHistory)
                .SingleOrDefaultAsync();
        }
    }
}
