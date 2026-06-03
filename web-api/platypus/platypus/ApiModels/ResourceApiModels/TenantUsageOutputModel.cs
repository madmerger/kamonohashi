namespace Nssol.Platypus.ApiModels.ResourceApiModels
{
    /// <summary>
    /// テナント別リソース使用量出力モデル
    /// </summary>
    public class TenantUsageOutputModel
    {
        /// <summary>
        /// テナント名
        /// </summary>
        public string TenantName { get; set; }

        /// <summary>
        /// テナント表示名
        /// </summary>
        public string TenantDisplayName { get; set; }

        /// <summary>
        /// CPU使用量
        /// </summary>
        public float CpuUsed { get; set; }

        /// <summary>
        /// メモリ使用量(GB)
        /// </summary>
        public float MemoryUsed { get; set; }

        /// <summary>
        /// GPU使用量
        /// </summary>
        public float GpuUsed { get; set; }

        /// <summary>
        /// コンテナ数
        /// </summary>
        public int ContainerCount { get; set; }
    }
}
