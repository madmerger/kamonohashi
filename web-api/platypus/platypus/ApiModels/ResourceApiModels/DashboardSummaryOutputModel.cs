namespace Nssol.Platypus.ApiModels.ResourceApiModels
{
    /// <summary>
    /// ダッシュボードのリソースサマリー出力モデル
    /// </summary>
    public class DashboardSummaryOutputModel
    {
        /// <summary>
        /// 割り当て可能なCPU合計
        /// </summary>
        public float TotalCpu { get; set; }

        /// <summary>
        /// 使用中のCPU合計
        /// </summary>
        public float UsedCpu { get; set; }

        /// <summary>
        /// 割り当て可能なメモリ合計(GB)
        /// </summary>
        public float TotalMemory { get; set; }

        /// <summary>
        /// 使用中のメモリ合計(GB)
        /// </summary>
        public float UsedMemory { get; set; }

        /// <summary>
        /// 割り当て可能なGPU合計
        /// </summary>
        public float TotalGpu { get; set; }

        /// <summary>
        /// 使用中のGPU合計
        /// </summary>
        public float UsedGpu { get; set; }

        /// <summary>
        /// CPU使用率(%)
        /// </summary>
        public float CpuUsageRate => TotalCpu > 0 ? (UsedCpu / TotalCpu) * 100 : 0;

        /// <summary>
        /// メモリ使用率(%)
        /// </summary>
        public float MemoryUsageRate => TotalMemory > 0 ? (UsedMemory / TotalMemory) * 100 : 0;

        /// <summary>
        /// GPU使用率(%)
        /// </summary>
        public float GpuUsageRate => TotalGpu > 0 ? (UsedGpu / TotalGpu) * 100 : 0;

        /// <summary>
        /// アクティブノード数
        /// </summary>
        public int ActiveNodeCount { get; set; }

        /// <summary>
        /// 総ノード数
        /// </summary>
        public int TotalNodeCount { get; set; }

        /// <summary>
        /// 実行中のコンテナ数
        /// </summary>
        public int RunningContainerCount { get; set; }
    }
}
