using Nssol.Platypus.Infrastructure;
using Nssol.Platypus.Models.TenantModels;

namespace Nssol.Platypus.ApiModels.ResourcePermissionApiModels
{
    /// <summary>
    /// リソース権限出力モデル
    /// </summary>
    public class IndexOutputModel : Components.OutputModelBase
    {
        public IndexOutputModel(ResourcePermission permission) : base(permission)
        {
            Id = permission.Id;
            ResourceType = permission.ResourceType;
            ResourceId = permission.ResourceId;
            UserId = permission.UserId;
            UserName = permission.User?.Name;
            AccessLevel = permission.AccessLevel;
            AccessLevelName = permission.AccessLevel.ToString();
        }

        /// <summary>
        /// ID
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// リソース種別
        /// </summary>
        public ResourceType2 ResourceType { get; set; }

        /// <summary>
        /// リソースID
        /// </summary>
        public long ResourceId { get; set; }

        /// <summary>
        /// ユーザーID
        /// </summary>
        public long UserId { get; set; }

        /// <summary>
        /// ユーザー名
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// アクセスレベル
        /// </summary>
        public ProjectRoleType AccessLevel { get; set; }

        /// <summary>
        /// アクセスレベル名
        /// </summary>
        public string AccessLevelName { get; set; }
    }
}
