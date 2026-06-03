namespace Nssol.Platypus.ApiModels.PipelineApiModels
{
    /// <summary>
    /// パイプラインノード出力モデル
    /// </summary>
    public class NodeOutputModel
    {
        /// <summary>
        /// ノードID
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// ノード名
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// ジョブ種別
        /// </summary>
        public string JobType { get; set; }

        /// <summary>
        /// 表示X座標
        /// </summary>
        public int PositionX { get; set; }

        /// <summary>
        /// 表示Y座標
        /// </summary>
        public int PositionY { get; set; }

        /// <summary>
        /// ジョブパラメータJSON
        /// </summary>
        public string JobParams { get; set; }
    }
}
