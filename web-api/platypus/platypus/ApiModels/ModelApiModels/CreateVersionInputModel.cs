namespace Nssol.Platypus.ApiModels.ModelApiModels
{
    /// <summary>
    /// モデルバージョン追加の入力モデル
    /// </summary>
    public class CreateVersionInputModel
    {
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
        public string Status { get; set; }

        /// <summary>
        /// メモ
        /// </summary>
        public string Description { get; set; }
    }
}
