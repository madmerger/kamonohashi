using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Nssol.Platypus.Models.TenantModels
{
    /// <summary>
    /// パイプライン定義
    /// </summary>
    public class Pipeline : TenantModelBase
    {
        /// <summary>
        /// パイプライン名
        /// </summary>
        [Required]
        public string Name { get; set; }

        /// <summary>
        /// メモ
        /// </summary>
        public string Memo { get; set; }

        /// <summary>
        /// パイプラインに含まれるノード一覧
        /// </summary>
        public virtual ICollection<PipelineNode> Nodes { get; set; }

        /// <summary>
        /// パイプラインに含まれるエッジ一覧
        /// </summary>
        public virtual ICollection<PipelineEdge> Edges { get; set; }

        /// <summary>
        /// パイプライン実行履歴一覧
        /// </summary>
        public virtual ICollection<PipelineRun> Runs { get; set; }
    }
}
