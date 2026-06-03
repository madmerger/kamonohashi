using System.Collections.Generic;

namespace Nssol.Platypus.ApiModels.ResourceApiModels
{
    /// <summary>
    /// リソース使用履歴出力モデル
    /// </summary>
    public class ResourceHistoryOutputModel
    {
        /// <summary>
        /// タイムスタンプのリスト(ラベル)
        /// </summary>
        public List<string> Labels { get; set; } = new List<string>();

        /// <summary>
        /// CPU使用率の時系列データ
        /// </summary>
        public List<float> CpuUsage { get; set; } = new List<float>();

        /// <summary>
        /// メモリ使用率の時系列データ
        /// </summary>
        public List<float> MemoryUsage { get; set; } = new List<float>();

        /// <summary>
        /// GPU使用率の時系列データ
        /// </summary>
        public List<float> GpuUsage { get; set; } = new List<float>();

        /// <summary>
        /// CPU合計の時系列データ
        /// </summary>
        public List<float> CpuTotal { get; set; } = new List<float>();

        /// <summary>
        /// メモリ合計の時系列データ
        /// </summary>
        public List<float> MemoryTotal { get; set; } = new List<float>();

        /// <summary>
        /// GPU合計の時系列データ
        /// </summary>
        public List<float> GpuTotal { get; set; } = new List<float>();
    }
}
