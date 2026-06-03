using Nssol.Platypus.Models.TenantModels;
using System;

namespace Nssol.Platypus.ApiModels.ModelApiModels
{
    /// <summary>
    /// モデル一覧の出力モデル
    /// </summary>
    public class IndexOutputModel
    {
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public IndexOutputModel(Model model)
        {
            Id = model.Id;
            Name = model.Name;
            Description = model.Description;
            CreatedAt = model.CreatedAt;
            CreatedBy = model.CreatedBy;
        }

        /// <summary>
        /// ID
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// モデル名
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 説明
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// 作成日時
        /// </summary>
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// 作成者
        /// </summary>
        public string CreatedBy { get; set; }
    }
}
