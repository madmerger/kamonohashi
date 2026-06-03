namespace Nssol.Platypus.ApiModels.ResourceApiModels
{
    /// <summary>
    /// ノードステータス出力モデル
    /// </summary>
    public class NodeStatusOutputModel
    {
        /// <summary>
        /// ノード名
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// ステータス (Ready / NotReady / Disconnected)
        /// </summary>
        public string Status { get; set; }

        /// <summary>
        /// パーティション
        /// </summary>
        public string Partition { get; set; }

        /// <summary>
        /// メモ
        /// </summary>
        public string Memo { get; set; }

        /// <summary>
        /// 割り当て可能CPU
        /// </summary>
        public float AllocatableCpu { get; set; }

        /// <summary>
        /// 使用中CPU
        /// </summary>
        public float UsedCpu { get; set; }

        /// <summary>
        /// 割り当て可能メモリ(GB)
        /// </summary>
        public float AllocatableMemory { get; set; }

        /// <summary>
        /// 使用中メモリ(GB)
        /// </summary>
        public float UsedMemory { get; set; }

        /// <summary>
        /// 割り当て可能GPU
        /// </summary>
        public float AllocatableGpu { get; set; }

        /// <summary>
        /// 使用中GPU
        /// </summary>
        public float UsedGpu { get; set; }

        /// <summary>
        /// コンテナ数
        /// </summary>
        public int ContainerCount { get; set; }

        /// <summary>
        /// TensorBoard有効フラグ
        /// </summary>
        public bool TensorBoardEnabled { get; set; }
    }
}
