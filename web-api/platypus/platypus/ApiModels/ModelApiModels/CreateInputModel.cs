using System.ComponentModel.DataAnnotations;

namespace Nssol.Platypus.ApiModels.ModelApiModels
{
    /// <summary>
    /// モデル作成の入力モデル
    /// </summary>
    public class CreateInputModel
    {
        /// <summary>
        /// モデル名
        /// </summary>
        [Required]
        public string Name { get; set; }

        /// <summary>
        /// 説明
        /// </summary>
        public string Description { get; set; }
    }
}
