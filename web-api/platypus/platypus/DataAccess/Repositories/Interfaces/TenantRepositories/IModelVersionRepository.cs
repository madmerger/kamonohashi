using Nssol.Platypus.DataAccess.Core;
using Nssol.Platypus.Models.TenantModels;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Nssol.Platypus.DataAccess.Repositories.Interfaces.TenantRepositories
{
    /// <summary>
    /// モデルバージョンテーブルにアクセスするためのリポジトリインターフェイス
    /// </summary>
    public interface IModelVersionRepository : IRepositoryForTenant<ModelVersion>
    {
        /// <summary>
        /// 指定されたモデルIDのバージョン一覧を取得します。
        /// </summary>
        IQueryable<ModelVersion> GetAllByModelId(long modelId);

        /// <summary>
        /// 指定されたモデルIDの最新バージョン番号を取得します。
        /// </summary>
        Task<int> GetLatestVersionNumberAsync(long modelId);

        /// <summary>
        /// 指定されたバージョンIDのバージョンエンティティ（学習履歴を含む）を取得します。
        /// </summary>
        Task<ModelVersion> GetIncludeAllAsync(long id);
    }
}
