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
    /// HPOトライアルテーブルにアクセスするためのリポジトリクラス
    /// </summary>
    public class HpoTrialRepository : RepositoryForTenantBase<HpoTrial>, IHpoTrialRepository
    {
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public HpoTrialRepository(CommonDbContext context, Microsoft.AspNetCore.Http.IHttpContextAccessor accessor)
            : base(context, accessor)
        {
        }

        /// <summary>
        /// 指定されたHPOジョブIDに属するトライアルを取得する
        /// </summary>
        public IQueryable<HpoTrial> GetByHpoJobId(long hpoJobId)
        {
            return GetAll().Where(t => t.HpoJobId == hpoJobId)
                .OrderBy(t => t.TrialNo);
        }

        /// <summary>
        /// 指定されたHPOジョブIDに属するトライアルのうち、ベストスコアのものを取得する
        /// </summary>
        public async Task<HpoTrial> GetBestTrialAsync(long hpoJobId, string direction)
        {
            var trials = GetAll()
                .Where(t => t.HpoJobId == hpoJobId && t.MetricValue.HasValue && t.Status == "Completed");

            if (direction == "minimize")
            {
                return await trials.OrderBy(t => t.MetricValue).FirstOrDefaultAsync();
            }
            else
            {
                return await trials.OrderByDescending(t => t.MetricValue).FirstOrDefaultAsync();
            }
        }

        /// <summary>
        /// トライアルのステータスとメトリクスを更新する
        /// </summary>
        public async Task UpdateTrialResultAsync(long id, string status, double? metricValue)
        {
            var entity = await GetByIdAsync(id);
            if (entity != null)
            {
                entity.Status = status;
                entity.MetricValue = metricValue;
            }
        }
    }
}
