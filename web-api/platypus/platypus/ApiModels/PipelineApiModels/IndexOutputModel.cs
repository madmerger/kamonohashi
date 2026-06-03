namespace Nssol.Platypus.ApiModels.PipelineApiModels
{
    /// <summary>
    /// パイプライン一覧出力モデル
    /// </summary>
    public class IndexOutputModel
    {
        /// <summary>
        /// ID
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// パイプライン名
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// メモ
        /// </summary>
        public string Memo { get; set; }

        /// <summary>
        /// ノード数
        /// </summary>
        public int NodeCount { get; set; }

        /// <summary>
        /// 登録日時
        /// </summary>
        public string CreatedAt { get; set; }

        /// <summary>
        /// 更新日時
        /// </summary>
        public string ModifiedAt { get; set; }
    }
}
