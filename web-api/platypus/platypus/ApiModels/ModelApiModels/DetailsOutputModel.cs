using Nssol.Platypus.Models.TenantModels;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Nssol.Platypus.ApiModels.ModelApiModels
{
    /// <summary>
    /// モデル詳細の出力モデル
    /// </summary>
    public class DetailsOutputModel
    {
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public DetailsOutputModel(Model model)
        {
            Id = model.Id;
            Name = model.Name;
            Description = model.Description;
            CreatedAt = model.CreatedAt;
            CreatedBy = model.CreatedBy;
            Versions = model.ModelVersions?.OrderByDescending(v => v.Version)
                .Select(v => new VersionOutputModel(v)).ToList();
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

        /// <summary>
        /// バージョン一覧
        /// </summary>
        public List<VersionOutputModel> Versions { get; set; }
    }
}
