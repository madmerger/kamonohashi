using Nssol.Platypus.Models.TenantModels;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Nssol.Platypus.Logic.Interfaces
{
    /// <summary>
    /// HPO（ハイパーパラメータ最適化）ロジックのインターフェイス
    /// </summary>
    public interface IHpoLogic
    {
        /// <summary>
        /// 探索空間から次のトライアルのパラメータセットを生成する
        /// </summary>
        /// <param name="hpoJob">HPOジョブ</param>
        /// <param name="completedTrials">完了済みトライアル一覧</param>
        /// <param name="trialNo">トライアル番号</param>
        /// <returns>パラメータのディクショナリ</returns>
        Dictionary<string, string> GenerateNextParameters(HpoJob hpoJob, IEnumerable<HpoTrial> completedTrials, int trialNo);

        /// <summary>
        /// HPOジョブを停止する
        /// </summary>
        Task StopHpoJobAsync(HpoJob hpoJob);
    }
}
