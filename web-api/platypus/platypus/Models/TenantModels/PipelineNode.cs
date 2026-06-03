using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Nssol.Platypus.Models.TenantModels
{
    /// <summary>
    /// パイプラインのノード（DAGの頂点）。各ノードはジョブ種別（前処理/学習/推論）とそのパラメータを持つ。
    /// </summary>
    public class PipelineNode : TenantModelBase
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
        /// ノード名
        /// </summary>
        [Required]
        public string Name { get; set; }

        /// <summary>
        /// ジョブ種別: Preprocessing / Training / Inference
        /// </summary>
        [Required]
        public string JobType { get; set; }

        /// <summary>
        /// DAG内の表示X座標
        /// </summary>
        public int PositionX { get; set; }

        /// <summary>
        /// DAG内の表示Y座標
        /// </summary>
        public int PositionY { get; set; }

        /// <summary>
        /// ジョブパラメータ (JSON)
        /// データセットID、Gitモデル情報、コンテナ情報、EntryPoint、CPU/Memory/GPU等を格納
        /// </summary>
        public string JobParams { get; set; }
    }
}
