using System.ComponentModel.DataAnnotations;

namespace Nssol.Platypus.ApiModels.PipelineApiModels
{
    /// <summary>
    /// パイプラインノード入力モデル
    /// </summary>
    public class NodeInputModel
    {
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
        /// UI上の表示X座標
        /// </summary>
        public int PositionX { get; set; }

        /// <summary>
        /// UI上の表示Y座標
        /// </summary>
        public int PositionY { get; set; }

        /// <summary>
        /// ジョブパラメータJSON
        /// </summary>
        public string JobParams { get; set; }
    }
}
