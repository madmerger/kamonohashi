using Nssol.Platypus.DataAccess.Core;
using Nssol.Platypus.Models.TenantModels;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Nssol.Platypus.DataAccess.Repositories.Interfaces.TenantRepositories
{
    /// <summary>
    /// HPOジョブテーブルにアクセスするためのリポジトリインターフェイス
    /// </summary>
    public interface IHpoJobRepository : IRepositoryForTenant<HpoJob>
    {
        /// <summary>
        /// 全HPOジョブ（データセットを含む）を並べ替えありで取得します。
        /// </summary>
        IQueryable<HpoJob> GetAllIncludeDataSetWithOrdering();

        /// <summary>
        /// 指定されたHPOジョブIDのエンティティ（トライアルを含む）を取得します。
        /// </summary>
        Task<HpoJob> GetIncludeTrialsAsync(long id);

        /// <summary>
        /// 全HPOジョブの名前とIDのみ取得する
        /// </summary>
        Task<IEnumerable<HpoJob>> GetAllNameAsync();

        /// <summary>
        /// ステータスを更新する
        /// </summary>
        Task UpdateStatusAsync(long id, string status);
    }
}
