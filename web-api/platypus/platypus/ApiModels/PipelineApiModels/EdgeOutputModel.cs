namespace Nssol.Platypus.ApiModels.PipelineApiModels
{
    /// <summary>
    /// パイプラインエッジ出力モデル
    /// </summary>
    public class EdgeOutputModel
    {
        /// <summary>
        /// エッジID
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// 依存元ノードID
        /// </summary>
        public long SourceNodeId { get; set; }

        /// <summary>
        /// 依存先ノードID
        /// </summary>
        public long TargetNodeId { get; set; }
    }
}
