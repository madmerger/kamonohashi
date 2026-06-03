using Nssol.Platypus.DataAccess.Core;
using Nssol.Platypus.Models.TenantModels;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Nssol.Platypus.DataAccess.Repositories.Interfaces.TenantRepositories
{
    /// <summary>
    /// モデルレジストリテーブルにアクセスするためのリポジトリインターフェイス
    /// </summary>
    public interface IModelRepository : IRepositoryForTenant<Model>
    {
        /// <summary>
        /// 全モデルを取得します。
        /// </summary>
        IQueryable<Model> GetAllModels();

        /// <summary>
        /// 指定されたモデルIDのモデルエンティティ（バージョンを含む）を取得します。
        /// </summary>
        Task<Model> GetIncludeAllAsync(long id);

        /// <summary>
        /// 指定されたモデル名のモデルが既に存在するかチェックします。
        /// </summary>
        Task<bool> ExistsByNameAsync(string name);
    }
}
