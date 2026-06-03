using Nssol.Platypus.Infrastructure;
using System.ComponentModel.DataAnnotations;

namespace Nssol.Platypus.ApiModels.ProjectApiModels
{
    /// <summary>
    /// プロジェクトリソース追加入力モデル
    /// </summary>
    public class ResourceMapInputModel
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
    }
}
