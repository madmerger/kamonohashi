using Nssol.Platypus.Infrastructure;
using Nssol.Platypus.Models.TenantModels;

namespace Nssol.Platypus.ApiModels.ProjectApiModels
{
    /// <summary>
    /// プロジェクトメンバー出力モデル
    /// </summary>
    public class MemberOutputModel : Components.OutputModelBase
    {
        public MemberOutputModel(ProjectMember member) : base(member)
        {
            Id = member.Id;
            UserId = member.UserId;
            UserName = member.User?.Name;
            RoleType = member.RoleType;
            RoleTypeName = member.RoleType.ToString();
        }

        /// <summary>
        /// ID
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// ユーザーID
        /// </summary>
        public long UserId { get; set; }

        /// <summary>
        /// ユーザー名
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// ロール種別
        /// </summary>
        public ProjectRoleType RoleType { get; set; }

        /// <summary>
        /// ロール種別名
        /// </summary>
        public string RoleTypeName { get; set; }
    }
}
