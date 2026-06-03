using System.Collections.Generic;

namespace Nssol.Platypus.ApiModels.PipelineApiModels
{
    /// <summary>
    /// パイプライン詳細出力モデル
    /// </summary>
    public class DetailsOutputModel
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
        /// ノード一覧
        /// </summary>
        public IEnumerable<NodeOutputModel> Nodes { get; set; }

        /// <summary>
        /// エッジ一覧
        /// </summary>
        public IEnumerable<EdgeOutputModel> Edges { get; set; }

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
