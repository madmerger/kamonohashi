using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Nssol.Platypus.ApiModels.PipelineApiModels
{
    /// <summary>
    /// パイプライン作成入力モデル
    /// </summary>
    public class CreateInputModel
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
        /// ノード定義
        /// </summary>
        [Required]
        public IEnumerable<NodeInputModel> Nodes { get; set; }

        /// <summary>
        /// エッジ定義
        /// </summary>
        public IEnumerable<EdgeInputModel> Edges { get; set; }
    }
}
