using Nssol.Platypus.DataAccess.Core;
using Nssol.Platypus.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Nssol.Platypus.DataAccess.Repositories.Interfaces
{
    /// <summary>
    /// カスタムロールテーブルにアクセスするためのリポジトリインターフェイス
    /// </summary>
    public interface ICustomRoleRepository
    {
        /// <summary>
        /// テナントのカスタムロール一覧を取得する
        /// </summary>
        Task<IEnumerable<CustomRole>> GetAllAsync(long tenantId);

        /// <summary>
        /// IDからカスタムロールを取得する
        /// </summary>
        Task<CustomRole> GetByIdAsync(long id);

        /// <summary>
        /// カスタムロールを追加する
        /// </summary>
        void Add(CustomRole role, IUnitOfWork unitOfWork);

        /// <summary>
        /// カスタムロールを更新する
        /// </summary>
        void Update(CustomRole role, IUnitOfWork unitOfWork);

        /// <summary>
        /// カスタムロールを削除する
        /// </summary>
        Task DeleteAsync(long id, IUnitOfWork unitOfWork);
    }
}
