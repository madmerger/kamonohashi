using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Nssol.Platypus.DataAccess.Core;
using Nssol.Platypus.DataAccess.Repositories.Interfaces.TenantRepositories;
using Nssol.Platypus.Models.TenantModels;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Nssol.Platypus.DataAccess.Repositories.TenantRepositories
{
    /// <summary>
    /// パイプラインリポジトリ
    /// </summary>
    public class PipelineRepository : RepositoryForTenantBase<Pipeline>, IPipelineRepository
    {
        private readonly CommonDbContext context;

        public PipelineRepository(CommonDbContext context, IHttpContextAccessor accessor)
            : base(context, accessor)
        {
            this.context = context;
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<Pipeline>> GetAllWithChildrenAsync()
        {
            return await GetAll()
                .Include(p => p.Nodes)
                .Include(p => p.Edges)
                .OrderByDescending(p => p.Id)
                .ToListAsync();
        }

        /// <inheritdoc/>
        public async Task<Pipeline> GetByIdWithChildrenAsync(long id)
        {
            return await GetAll()
                .Include(p => p.Nodes)
                .Include(p => p.Edges)
                .Include(p => p.Runs)
                    .ThenInclude(r => r.Steps)
                .SingleOrDefaultAsync(p => p.Id == id);
        }

        /// <inheritdoc/>
        public async Task<PipelineRun> GetRunByIdAsync(long runId)
        {
            return await context.PipelineRuns
                .Include(r => r.Steps)
                    .ThenInclude(s => s.PipelineNode)
                .Include(r => r.Pipeline)
                .Where(r => r.TenantId == CurrentTenantId)
                .SingleOrDefaultAsync(r => r.Id == runId);
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<PipelineRun>> GetRunsByPipelineIdAsync(long pipelineId)
        {
            return await context.PipelineRuns
                .Include(r => r.Steps)
                    .ThenInclude(s => s.PipelineNode)
                .Where(r => r.TenantId == CurrentTenantId && r.PipelineId == pipelineId)
                .OrderByDescending(r => r.Id)
                .ToListAsync();
        }

        /// <inheritdoc/>
        public void AddRun(PipelineRun run)
        {
            run.TenantId = CurrentTenantId;
            context.PipelineRuns.Add(run);
        }

        /// <inheritdoc/>
        public void AddNode(PipelineNode node)
        {
            node.TenantId = CurrentTenantId;
            context.PipelineNodes.Add(node);
        }

        /// <inheritdoc/>
        public void AddEdge(PipelineEdge edge)
        {
            edge.TenantId = CurrentTenantId;
            context.PipelineEdges.Add(edge);
        }

        /// <inheritdoc/>
        public void AddRunStep(PipelineRunStep step)
        {
            step.TenantId = CurrentTenantId;
            context.PipelineRunSteps.Add(step);
        }

        /// <inheritdoc/>
        public void DeleteNodes(long pipelineId)
        {
            var nodes = context.PipelineNodes
                .Where(n => n.TenantId == CurrentTenantId && n.PipelineId == pipelineId);
            context.PipelineNodes.RemoveRange(nodes);
        }

        /// <inheritdoc/>
        public void DeleteEdges(long pipelineId)
        {
            var edges = context.PipelineEdges
                .Where(e => e.TenantId == CurrentTenantId && e.PipelineId == pipelineId);
            context.PipelineEdges.RemoveRange(edges);
        }
    }
}
