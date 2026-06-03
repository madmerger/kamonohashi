using Microsoft.EntityFrameworkCore;
using Nssol.Platypus.DataAccess.Core;
using Nssol.Platypus.DataAccess.Repositories.Interfaces.TenantRepositories;
using Nssol.Platypus.Models.TenantModels;
using System.Linq;
using System.Threading.Tasks;

namespace Nssol.Platypus.DataAccess.Repositories.TenantRepositories
{
    /// <summary>
    /// モデルレジストリテーブルにアクセスするためのリポジトリクラス
    /// </summary>
    public class ModelRepository : RepositoryForTenantBase<Model>, IModelRepository
    {
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public ModelRepository(CommonDbContext context, Microsoft.AspNetCore.Http.IHttpContextAccessor accessor)
            : base(context, accessor)
        {
        }

        /// <summary>
        /// 全モデルを取得します。
        /// </summary>
        public IQueryable<Model> GetAllModels()
        {
            return GetAll().OrderByDescending(m => m.Id);
        }

        /// <summary>
        /// 指定されたモデルIDのモデルエンティティ（バージョンを含む）を取得します。
        /// </summary>
        public async Task<Model> GetIncludeAllAsync(long id)
        {
            return await FindAll(m => m.Id == id)
                .Include(m => m.ModelVersions)
                    .ThenInclude(v => v.TrainingHistory)
                .SingleOrDefaultAsync();
        }

        /// <summary>
        /// 指定されたモデル名のモデルが既に存在するかチェックします。
        /// </summary>
        public async Task<bool> ExistsByNameAsync(string name)
        {
            return await FindAll(m => m.Name == name).AnyAsync();
        }
    }
}
