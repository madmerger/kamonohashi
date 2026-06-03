using Microsoft.EntityFrameworkCore;
using Nssol.Platypus.DataAccess.Core;
using Nssol.Platypus.DataAccess.Repositories.Interfaces.TenantRepositories;
using Nssol.Platypus.Models.TenantModels;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Nssol.Platypus.DataAccess.Repositories.TenantRepositories
{
    /// <summary>
    /// HPOジョブテーブルにアクセスするためのリポジトリクラス
    /// </summary>
    public class HpoJobRepository : RepositoryForTenantBase<HpoJob>, IHpoJobRepository
    {
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public HpoJobRepository(CommonDbContext context, Microsoft.AspNetCore.Http.IHttpContextAccessor accessor)
            : base(context, accessor)
        {
        }

        /// <summary>
        /// 全HPOジョブ（データセットを含む）を並べ替えありで取得します。
        /// </summary>
        public IQueryable<HpoJob> GetAllIncludeDataSetWithOrdering()
        {
            return GetAll().OrderByDescending(t => t.Id)
                .Include(t => t.DataSet)
                .Include(t => t.Trials);
        }

        /// <summary>
        /// 指定されたHPOジョブIDのエンティティ（トライアルを含む）を取得します。
        /// </summary>
        public async Task<HpoJob> GetIncludeTrialsAsync(long id)
        {
            return await GetAll()
                .Include(t => t.DataSet)
                .Include(t => t.Trials)
                .SingleOrDefaultAsync(t => t.Id == id);
        }

        /// <summary>
        /// 全HPOジョブの名前とIDのみ取得する
        /// </summary>
        public async Task<IEnumerable<HpoJob>> GetAllNameAsync()
        {
            return await GetAll()
                .Select(t => new HpoJob { Id = t.Id, Name = t.Name })
                .ToListAsync();
        }

        /// <summary>
        /// ステータスを更新する
        /// </summary>
        public async Task UpdateStatusAsync(long id, string status)
        {
            var entity = await GetByIdAsync(id);
            if (entity != null)
            {
                entity.Status = status;
            }
        }

        /// <summary>
        /// DBから最新のステータスを取得する（EFキャッシュをバイパス）
        /// </summary>
        public async Task<string> GetCurrentStatusAsync(long id)
        {
            return await GetAll().AsNoTracking()
                .Where(j => j.Id == id)
                .Select(j => j.Status)
                .FirstOrDefaultAsync();
        }
    }
}
