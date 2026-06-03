using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Nssol.Platypus.Models.TenantModels
{
    /// <summary>
    /// モデルバージョン
    /// </summary>
    public class ModelVersion : TenantModelBase
    {
        /// <summary>
        /// モデルID
        /// </summary>
        [Required]
        public long ModelId { get; set; }

        /// <summary>
        /// バージョン番号
        /// </summary>
        [Required]
        public int Version { get; set; }

        /// <summary>
        /// 学習履歴ID
        /// </summary>
        public long? TrainingHistoryId { get; set; }

        /// <summary>
        /// 精度
        /// </summary>
        public float? Accuracy { get; set; }

        /// <summary>
        /// ステータス (none/staging/production)
        /// </summary>
        [Required]
        public string Status { get; set; }

        /// <summary>
        /// メモ
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// モデル
        /// </summary>
        [ForeignKey(nameof(ModelId))]
        public virtual Model Model { get; set; }

        /// <summary>
        /// 学習履歴
        /// </summary>
        [ForeignKey(nameof(TrainingHistoryId))]
        public virtual TrainingHistory TrainingHistory { get; set; }

        /// <summary>
        /// モデルバージョンの文字列表現
        /// </summary>
        public override string ToString()
        {
            return $"{ModelId}:v{Version}";
        }
    }
}
