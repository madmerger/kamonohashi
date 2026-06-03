using Nssol.Platypus.Models.TenantModels;
using System;

namespace Nssol.Platypus.ApiModels.ModelApiModels
{
    /// <summary>
    /// モデルバージョンの出力モデル
    /// </summary>
    public class VersionOutputModel
    {
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public VersionOutputModel(ModelVersion version)
        {
            Id = version.Id;
            ModelId = version.ModelId;
            Version = version.Version;
            TrainingHistoryId = version.TrainingHistoryId;
            TrainingHistoryName = version.TrainingHistory?.Name;
            Accuracy = version.Accuracy;
            Status = version.Status;
            Description = version.Description;
            CreatedAt = version.CreatedAt;
            CreatedBy = version.CreatedBy;
        }

        /// <summary>
        /// ID
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// モデルID
        /// </summary>
        public long ModelId { get; set; }

        /// <summary>
        /// バージョン番号
        /// </summary>
        public int Version { get; set; }

        /// <summary>
        /// 学習履歴ID
        /// </summary>
        public long? TrainingHistoryId { get; set; }

        /// <summary>
        /// 学習履歴名
        /// </summary>
        public string TrainingHistoryName { get; set; }

        /// <summary>
        /// 精度
        /// </summary>
        public float? Accuracy { get; set; }

        /// <summary>
        /// ステータス
        /// </summary>
        public string Status { get; set; }

        /// <summary>
        /// メモ
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
