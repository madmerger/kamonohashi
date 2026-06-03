using System.ComponentModel.DataAnnotations;

namespace Nssol.Platypus.Models.TenantModels
{
    /// <summary>
    /// プロジェクト。テナント内でリソースをグルーピングする単位。
    /// </summary>
    public class Project : TenantModelBase
    {
        /// <summary>
        /// プロジェクト名
        /// </summary>
        [Required]
        public string Name { get; set; }

        /// <summary>
        /// 説明
        /// </summary>
        public string Description { get; set; }
    }
}
