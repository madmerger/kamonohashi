using Nssol.Platypus.DataAccess.Core;
using Nssol.Platypus.Models.TenantModels;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Nssol.Platypus.DataAccess.Repositories.Interfaces.TenantRepositories
{
    /// <summary>
    /// HPOトライアルテーブルにアクセスするためのリポジトリインターフェイス
    /// </summary>
    public interface IHpoTrialRepository : IRepositoryForTenant<HpoTrial>
    {
        /// <summary>
        /// 指定されたHPOジョブIDに属するトライアルを取得する
        /// </summary>
        IQueryable<HpoTrial> GetByHpoJobId(long hpoJobId);

        /// <summary>
        /// 指定されたHPOジョブIDに属するトライアルのうち、ベストスコアのものを取得する
        /// </summary>
        Task<HpoTrial> GetBestTrialAsync(long hpoJobId, string direction);

        /// <summary>
        /// トライアルのステータスとメトリクスを更新する
        /// </summary>
        Task UpdateTrialResultAsync(long id, string status, double? metricValue);
    }
}
