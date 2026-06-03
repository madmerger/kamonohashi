using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Nssol.Platypus.Models.TenantModels
{
    /// <summary>
    /// パイプラインのエッジ（DAGの辺）。ノード間の依存関係を定義する。
    /// </summary>
    public class PipelineEdge : TenantModelBase
    {
        /// <summary>
        /// 所属パイプラインID
        /// </summary>
        [Required]
        public long PipelineId { get; set; }

        /// <summary>
        /// 所属パイプライン
        /// </summary>
        [ForeignKey(nameof(PipelineId))]
        public virtual Pipeline Pipeline { get; set; }

        /// <summary>
        /// 依存元ノード（先行ジョブ）ID
        /// </summary>
        [Required]
        public long SourceNodeId { get; set; }

        /// <summary>
        /// 依存元ノード
        /// </summary>
        [ForeignKey(nameof(SourceNodeId))]
        public virtual PipelineNode SourceNode { get; set; }

        /// <summary>
        /// 依存先ノード（後続ジョブ）ID
        /// </summary>
        [Required]
        public long TargetNodeId { get; set; }

        /// <summary>
        /// 依存先ノード
        /// </summary>
        [ForeignKey(nameof(TargetNodeId))]
        public virtual PipelineNode TargetNode { get; set; }
    }
}
