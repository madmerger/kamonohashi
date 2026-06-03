using System.ComponentModel.DataAnnotations;

namespace Nssol.Platypus.ApiModels.ProjectApiModels
{
    /// <summary>
    /// プロジェクト編集入力モデル
    /// </summary>
    public class EditInputModel
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
