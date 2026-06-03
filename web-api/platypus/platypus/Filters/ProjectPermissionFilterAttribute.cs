using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Nssol.Platypus.DataAccess.Repositories.Interfaces.TenantRepositories;
using Nssol.Platypus.Infrastructure;
using Nssol.Platypus.Logic.Interfaces;
using System;
using System.Threading.Tasks;

namespace Nssol.Platypus.Filters
{
    /// <summary>
    /// プロジェクト単位の認可フィルタ。
    /// 指定したプロジェクトに対して必要な権限レベルを持つユーザーのみアクセスを許可する。
    /// ルートパラメータから projectId を取得し、ユーザーのプロジェクト内権限をチェックする。
    /// </summary>
    public sealed class ProjectPermissionFilterAttribute : TypeFilterAttribute
    {
        public ProjectRoleType RequiredRole { get; }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="requiredRole">必要な権限レベル</param>
        public ProjectPermissionFilterAttribute(ProjectRoleType requiredRole)
            : base(typeof(ProjectPermissionFilterAttributeImpl))
        {
            RequiredRole = requiredRole;
            Arguments = new object[] { requiredRole };
        }

        private sealed class ProjectPermissionFilterAttributeImpl : Attribute, IAsyncActionFilter
        {
            private readonly IProjectRepository projectRepository;
            private readonly IMultiTenancyLogic multiTenancyLogic;
            private readonly ProjectRoleType requiredRole;

            public ProjectPermissionFilterAttributeImpl(
                IProjectRepository projectRepository,
                IMultiTenancyLogic multiTenancyLogic,
                ProjectRoleType requiredRole)
            {
                this.projectRepository = projectRepository;
                this.multiTenancyLogic = multiTenancyLogic;
                this.requiredRole = requiredRole;
            }

            public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
            {
                if (context.HttpContext?.User?.Identity?.IsAuthenticated ?? false)
                {
                    // ルートパラメータから projectId を取得
                    if (context.ActionArguments.TryGetValue("projectId", out var projectIdObj) && projectIdObj is long projectId)
                    {
                        var userId = multiTenancyLogic.CurrentUserInfo.Id;
                        var member = await projectRepository.GetMemberAsync(projectId, userId);

                        if (member == null || member.RoleType < requiredRole)
                        {
                            context.HttpContext.Response.StatusCode = (int)System.Net.HttpStatusCode.Forbidden;
                            context.HttpContext.Response.Headers.Add("X-Required-ProjectRole", requiredRole.ToString());
                            return;
                        }
                    }
                }

                await next();
            }
        }
    }
}
