using System.ComponentModel.DataAnnotations;

namespace Nssol.Platypus.ApiModels.PipelineApiModels
{
    /// <summary>
    /// パイプラインエッジ入力モデル
    /// </summary>
    public class EdgeInputModel
    {
        /// <summary>
        /// 依存元ノードのインデックス（Nodesリスト内のインデックス）
        /// </summary>
        [Required]
        public int SourceNodeIndex { get; set; }

        /// <summary>
        /// 依存先ノードのインデックス（Nodesリスト内のインデックス）
        /// </summary>
        [Required]
        public int TargetNodeIndex { get; set; }
    }
}
