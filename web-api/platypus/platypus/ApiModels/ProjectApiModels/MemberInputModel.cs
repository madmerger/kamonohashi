using Nssol.Platypus.Infrastructure;
using System.ComponentModel.DataAnnotations;

namespace Nssol.Platypus.ApiModels.ProjectApiModels
{
    /// <summary>
    /// プロジェクトメンバー追加・変更入力モデル
    /// </summary>
    public class MemberInputModel
    {
        /// <summary>
        /// ユーザーID
        /// </summary>
        [Required]
        public long UserId { get; set; }

        /// <summary>
        /// プロジェクト内のロール種別
        /// </summary>
        [Required]
        public ProjectRoleType RoleType { get; set; }
    }
}
