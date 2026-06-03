namespace Nssol.Platypus.ApiModels.ModelApiModels
{
    /// <summary>
    /// モデルバージョン編集の入力モデル
    /// </summary>
    public class EditVersionInputModel
    {
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
