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
    /// ルートパラメータからプロジェクトIDを取得し、ユーザーのプロジェクト内権限をチェックする。
    /// パラメータが見つからない場合はアクセスを拒否する（fail closed）。
    /// </summary>
    public sealed class ProjectPermissionFilterAttribute : TypeFilterAttribute
    {
        public ProjectRoleType RequiredRole { get; }

        /// <summary>
        /// コンストラクタ（デフォルトパラメータ名: "id", "projectId"）
        /// </summary>
        /// <param name="requiredRole">必要な権限レベル</param>
        public ProjectPermissionFilterAttribute(ProjectRoleType requiredRole)
            : this(requiredRole, "id", "projectId")
        {
        }

        /// <summary>
        /// コンストラクタ（パラメータ名を指定）
        /// </summary>
        /// <param name="requiredRole">必要な権限レベル</param>
        /// <param name="parameterNames">アクションパラメータ名の候補リスト</param>
        public ProjectPermissionFilterAttribute(ProjectRoleType requiredRole, params string[] parameterNames)
            : base(typeof(ProjectPermissionFilterAttributeImpl))
        {
            RequiredRole = requiredRole;
            Arguments = new object[] { requiredRole, parameterNames };
        }

        private sealed class ProjectPermissionFilterAttributeImpl : Attribute, IAsyncActionFilter
        {
            private readonly IProjectRepository projectRepository;
            private readonly IMultiTenancyLogic multiTenancyLogic;
            private readonly ProjectRoleType requiredRole;
            private readonly string[] parameterNames;

            public ProjectPermissionFilterAttributeImpl(
                IProjectRepository projectRepository,
                IMultiTenancyLogic multiTenancyLogic,
                ProjectRoleType requiredRole,
                string[] parameterNames)
            {
                this.projectRepository = projectRepository;
                this.multiTenancyLogic = multiTenancyLogic;
                this.requiredRole = requiredRole;
                this.parameterNames = parameterNames;
            }

            public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
            {
                if (!(context.HttpContext?.User?.Identity?.IsAuthenticated ?? false))
                {
                    context.HttpContext.Response.StatusCode = (int)System.Net.HttpStatusCode.Unauthorized;
                    return;
                }

                // 候補パラメータ名からプロジェクトIDを取得
                long? projectId = null;
                foreach (var name in parameterNames)
                {
                    if (context.ActionArguments.TryGetValue(name, out var value))
                    {
                        if (value is long longVal)
                        {
                            projectId = longVal;
                            break;
                        }
                        if (value is int intVal)
                        {
                            projectId = intVal;
                            break;
                        }
                    }
                }

                // パラメータが見つからない場合はアクセス拒否（fail closed）
                if (!projectId.HasValue)
                {
                    context.HttpContext.Response.StatusCode = (int)System.Net.HttpStatusCode.Forbidden;
                    return;
                }

                var userId = multiTenancyLogic.CurrentUserInfo.Id;
                var member = await projectRepository.GetMemberAsync(projectId.Value, userId);

                if (member == null || member.RoleType < requiredRole)
                {
                    context.HttpContext.Response.StatusCode = (int)System.Net.HttpStatusCode.Forbidden;
                    context.HttpContext.Response.Headers.Add("X-Required-ProjectRole", requiredRole.ToString());
                    return;
                }

                await next();
            }
        }
    }
}
