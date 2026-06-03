using System.Collections.Generic;

namespace Nssol.Platypus.ApiModels.DataSetApiModels
{
    /// <summary>
    /// データセット統計情報の出力モデル
    /// </summary>
    public class DataSetStatisticsOutputModel
    {
        /// <summary>
        /// データセットID
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// データセット名
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// データエントリ総数
        /// </summary>
        public int TotalDataCount { get; set; }

        /// <summary>
        /// ファイル総数
        /// </summary>
        public int TotalFileCount { get; set; }

        /// <summary>
        /// 合計ファイルサイズ(バイト)
        /// </summary>
        public long TotalFileSize { get; set; }

        /// <summary>
        /// ファイルタイプ別分布(拡張子 -> ファイル数)
        /// </summary>
        public Dictionary<string, int> FileTypeDistribution { get; set; }

        /// <summary>
        /// クラス分布(データ種別名 -> データ数)
        /// </summary>
        public Dictionary<string, int> ClassDistribution { get; set; }
    }
}
