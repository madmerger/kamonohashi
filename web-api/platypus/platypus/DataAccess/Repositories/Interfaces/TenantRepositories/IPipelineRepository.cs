using Nssol.Platypus.DataAccess.Core;
using Nssol.Platypus.Models.TenantModels;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Nssol.Platypus.DataAccess.Repositories.Interfaces.TenantRepositories
{
    /// <summary>
    /// パイプラインリポジトリインターフェース
    /// </summary>
    public interface IPipelineRepository : IRepositoryForTenant<Pipeline>
    {
        /// <summary>
        /// パイプライン一覧を取得（ノード・エッジ含む）
        /// </summary>
        Task<IEnumerable<Pipeline>> GetAllWithChildrenAsync();

        /// <summary>
        /// パイプライン詳細を取得（ノード・エッジ含む）
        /// </summary>
        Task<Pipeline> GetByIdWithChildrenAsync(long id);

        /// <summary>
        /// パイプラインRunを取得（ステップ含む）
        /// </summary>
        Task<PipelineRun> GetRunByIdAsync(long runId);

        /// <summary>
        /// 指定パイプラインの全Runを取得
        /// </summary>
        Task<IEnumerable<PipelineRun>> GetRunsByPipelineIdAsync(long pipelineId);

        /// <summary>
        /// PipelineRunを追加
        /// </summary>
        void AddRun(PipelineRun run);

        /// <summary>
        /// PipelineNodeを追加
        /// </summary>
        void AddNode(PipelineNode node);

        /// <summary>
        /// PipelineEdgeを追加
        /// </summary>
        void AddEdge(PipelineEdge edge);

        /// <summary>
        /// PipelineRunStepを追加
        /// </summary>
        void AddRunStep(PipelineRunStep step);

        /// <summary>
        /// 指定パイプラインのノードを全削除
        /// </summary>
        void DeleteNodes(long pipelineId);

        /// <summary>
        /// 指定パイプラインのエッジを全削除
        /// </summary>
        void DeleteEdges(long pipelineId);
    }
}
