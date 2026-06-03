using Microsoft.EntityFrameworkCore;
using Nssol.Platypus.DataAccess.Core;
using Nssol.Platypus.DataAccess.Repositories.Interfaces;
using Nssol.Platypus.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Nssol.Platypus.DataAccess.Repositories
{
    /// <summary>
    /// カスタムロールリポジトリ
    /// </summary>
    public class CustomRoleRepository : RepositoryBase<CustomRole>, ICustomRoleRepository
    {
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public CustomRoleRepository(CommonDbContext dataContext) : base(dataContext)
        {
        }

        /// <summary>
        /// テナントのカスタムロール一覧を取得する
        /// </summary>
        public async Task<IEnumerable<CustomRole>> GetAllAsync(long tenantId)
        {
            return await GetAll().Where(r => r.TenantId == tenantId).OrderBy(r => r.Name).ToListAsync();
        }

        /// <summary>
        /// IDからカスタムロールを取得する
        /// </summary>
        public new async Task<CustomRole> GetByIdAsync(long id)
        {
            return await base.GetByIdAsync(id);
        }

        /// <summary>
        /// カスタムロールを追加する
        /// </summary>
        public void Add(CustomRole role, IUnitOfWork unitOfWork)
        {
            base.Add(role);
            unitOfWork.Commit();
        }

        /// <summary>
        /// カスタムロールを更新する
        /// </summary>
        public void Update(CustomRole role, IUnitOfWork unitOfWork)
        {
            unitOfWork.Commit();
        }

        /// <summary>
        /// カスタムロールを削除する
        /// </summary>
        public async Task DeleteAsync(long id, IUnitOfWork unitOfWork)
        {
            var role = await base.GetByIdAsync(id);
            base.Delete(role);
            unitOfWork.Commit();
        }
    }
}
