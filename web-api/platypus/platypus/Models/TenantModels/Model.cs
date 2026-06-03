using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Nssol.Platypus.Models.TenantModels
{
    /// <summary>
    /// モデルレジストリ
    /// </summary>
    public class Model : TenantModelBase
    {
        /// <summary>
        /// モデル名
        /// </summary>
        [Required]
        public string Name { get; set; }

        /// <summary>
        /// モデルの説明
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// モデルバージョン一覧
        /// </summary>
        public virtual ICollection<ModelVersion> ModelVersions { get; set; }

        /// <summary>
        /// モデルの文字列表現
        /// </summary>
        public override string ToString()
        {
            return $"{Id}:{Name}";
        }
    }
}
