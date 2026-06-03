using Nssol.Platypus.Models.TenantModels;
using System.Threading.Tasks;

namespace Nssol.Platypus.Logic.Interfaces
{
    /// <summary>
    /// パイプライン操作のロジックインターフェース
    /// </summary>
    public interface IPipelineLogic
    {
        /// <summary>
        /// パイプラインを実行する。DAGに基づきルートノードから順次ジョブを起動する。
        /// </summary>
        /// <param name="pipeline">実行対象パイプライン</param>
        /// <param name="run">実行インスタンス</param>
        Task ExecuteAsync(Pipeline pipeline, PipelineRun run);

        /// <summary>
        /// パイプライン実行の次のステップを進める。
        /// 完了したステップの後続ノードのうち、全依存が完了したものを起動する。
        /// </summary>
        /// <param name="run">実行インスタンス</param>
        Task AdvanceAsync(PipelineRun run);
    }
}
