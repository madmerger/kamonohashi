using Nssol.Platypus.Infrastructure;
using System.ComponentModel.DataAnnotations;

namespace Nssol.Platypus.ApiModels.ResourcePermissionApiModels
{
    /// <summary>
    /// リソース権限追加入力モデル
    /// </summary>
    public class CreateInputModel
    {
        /// <summary>
        /// リソース種別
        /// </summary>
        [Required]
        public ResourceType2 ResourceType { get; set; }

        /// <summary>
        /// リソースID
        /// </summary>
        [Required]
        public long ResourceId { get; set; }

        /// <summary>
        /// ユーザーID
        /// </summary>
        [Required]
        public long UserId { get; set; }

        /// <summary>
        /// アクセスレベル
        /// </summary>
        [Required]
        public ProjectRoleType AccessLevel { get; set; }
    }
}
