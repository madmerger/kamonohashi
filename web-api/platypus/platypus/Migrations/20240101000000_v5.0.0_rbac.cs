using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace Nssol.Platypus.Migrations
{
    public partial class v500_rbac : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // プロジェクトテーブル
            migrationBuilder.CreateTable(
                name: "Projects",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CreatedBy = table.Column<string>(nullable: false),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    ModifiedBy = table.Column<string>(nullable: false),
                    ModifiedAt = table.Column<DateTime>(nullable: false),
                    TenantId = table.Column<long>(nullable: false),
                    Name = table.Column<string>(nullable: false),
                    Description = table.Column<string>(nullable: true),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Projects", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Projects_Tenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "Tenants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Projects_TenantId",
                table: "Projects",
                column: "TenantId");

            // プロジェクトメンバーテーブル
            migrationBuilder.CreateTable(
                name: "ProjectMembers",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CreatedBy = table.Column<string>(nullable: false),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    ModifiedBy = table.Column<string>(nullable: false),
                    ModifiedAt = table.Column<DateTime>(nullable: false),
                    TenantId = table.Column<long>(nullable: false),
                    ProjectId = table.Column<long>(nullable: false),
                    UserId = table.Column<long>(nullable: false),
                    RoleType = table.Column<int>(nullable: false),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProjectMembers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProjectMembers_Projects_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "Projects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ProjectMembers_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ProjectMembers_Tenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "Tenants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ProjectMembers_TenantId_ProjectId_UserId",
                table: "ProjectMembers",
                columns: new[] { "TenantId", "ProjectId", "UserId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ProjectMembers_ProjectId",
                table: "ProjectMembers",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectMembers_UserId",
                table: "ProjectMembers",
                column: "UserId");

            // プロジェクトリソースマッピングテーブル
            migrationBuilder.CreateTable(
                name: "ProjectResourceMaps",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CreatedBy = table.Column<string>(nullable: false),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    ModifiedBy = table.Column<string>(nullable: false),
                    ModifiedAt = table.Column<DateTime>(nullable: false),
                    TenantId = table.Column<long>(nullable: false),
                    ProjectId = table.Column<long>(nullable: false),
                    ResourceType = table.Column<int>(nullable: false),
                    ResourceId = table.Column<long>(nullable: false),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProjectResourceMaps", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProjectResourceMaps_Projects_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "Projects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ProjectResourceMaps_Tenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "Tenants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ProjectResourceMaps_TenantId_ProjectId_ResourceType_ResourceId",
                table: "ProjectResourceMaps",
                columns: new[] { "TenantId", "ProjectId", "ResourceType", "ResourceId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ProjectResourceMaps_ProjectId",
                table: "ProjectResourceMaps",
                column: "ProjectId");

            // リソース権限テーブル
            migrationBuilder.CreateTable(
                name: "ResourcePermissions",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CreatedBy = table.Column<string>(nullable: false),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    ModifiedBy = table.Column<string>(nullable: false),
                    ModifiedAt = table.Column<DateTime>(nullable: false),
                    TenantId = table.Column<long>(nullable: false),
                    ResourceType = table.Column<int>(nullable: false),
                    ResourceId = table.Column<long>(nullable: false),
                    UserId = table.Column<long>(nullable: false),
                    AccessLevel = table.Column<int>(nullable: false),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ResourcePermissions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ResourcePermissions_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ResourcePermissions_Tenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "Tenants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ResourcePermissions_TenantId_ResourceType_ResourceId_UserId",
                table: "ResourcePermissions",
                columns: new[] { "TenantId", "ResourceType", "ResourceId", "UserId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ResourcePermissions_UserId",
                table: "ResourcePermissions",
                column: "UserId");

            // カスタムロールテーブル
            migrationBuilder.CreateTable(
                name: "CustomRoles",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CreatedBy = table.Column<string>(nullable: false),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    ModifiedBy = table.Column<string>(nullable: false),
                    ModifiedAt = table.Column<DateTime>(nullable: false),
                    Name = table.Column<string>(nullable: false),
                    DisplayName = table.Column<string>(nullable: false),
                    TenantId = table.Column<long>(nullable: false),
                    Description = table.Column<string>(nullable: true),
                    CanManageData = table.Column<bool>(nullable: false, defaultValue: false),
                    CanManageDataSet = table.Column<bool>(nullable: false, defaultValue: false),
                    CanRunTraining = table.Column<bool>(nullable: false, defaultValue: false),
                    CanRunInference = table.Column<bool>(nullable: false, defaultValue: false),
                    CanUseNotebook = table.Column<bool>(nullable: false, defaultValue: false),
                    CanManageProject = table.Column<bool>(nullable: false, defaultValue: false),
                    CanEditTenantSetting = table.Column<bool>(nullable: false, defaultValue: false),
                    CanManageResourcePermission = table.Column<bool>(nullable: false, defaultValue: false),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CustomRoles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CustomRoles_Tenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "Tenants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CustomRoles_TenantId_Name",
                table: "CustomRoles",
                columns: new[] { "TenantId", "Name" },
                unique: true);

            // 新メニューコードのMenuRoleMapを追加（managersロールに権限付与）
            string adminUser = "admin";
            DateTime now = DateTime.Now;
            migrationBuilder.Sql($"INSERT INTO \"MenuRoleMaps\" (\"Id\", \"CreatedBy\", \"CreatedAt\", \"ModifiedBy\", \"ModifiedAt\", \"MenuCode\", \"RoleId\") SELECT nextval('\"MenuRoleMaps_Id_seq\"'), '{adminUser}', '{now}', '{adminUser}', '{now}', '{Logic.MenuLogic.ProjectManagementMenu.Code.ToString()}', \"Id\" FROM \"Roles\" WHERE \"Name\" = 'managers';");
            migrationBuilder.Sql($"INSERT INTO \"MenuRoleMaps\" (\"Id\", \"CreatedBy\", \"CreatedAt\", \"ModifiedBy\", \"ModifiedAt\", \"MenuCode\", \"RoleId\") SELECT nextval('\"MenuRoleMaps_Id_seq\"'), '{adminUser}', '{now}', '{adminUser}', '{now}', '{Logic.MenuLogic.ResourcePermissionMenu.Code.ToString()}', \"Id\" FROM \"Roles\" WHERE \"Name\" = 'managers';");
            migrationBuilder.Sql($"INSERT INTO \"MenuRoleMaps\" (\"Id\", \"CreatedBy\", \"CreatedAt\", \"ModifiedBy\", \"ModifiedAt\", \"MenuCode\", \"RoleId\") SELECT nextval('\"MenuRoleMaps_Id_seq\"'), '{adminUser}', '{now}', '{adminUser}', '{now}', '{Logic.MenuLogic.CustomRoleMenu.Code.ToString()}', \"Id\" FROM \"Roles\" WHERE \"Name\" = 'managers';");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql($"DELETE FROM \"MenuRoleMaps\" WHERE \"MenuCode\" = '{Logic.MenuLogic.ProjectManagementMenu.Code.ToString()}';");
            migrationBuilder.Sql($"DELETE FROM \"MenuRoleMaps\" WHERE \"MenuCode\" = '{Logic.MenuLogic.ResourcePermissionMenu.Code.ToString()}';");
            migrationBuilder.Sql($"DELETE FROM \"MenuRoleMaps\" WHERE \"MenuCode\" = '{Logic.MenuLogic.CustomRoleMenu.Code.ToString()}';");
            migrationBuilder.DropTable(name: "CustomRoles");
            migrationBuilder.DropTable(name: "ResourcePermissions");
            migrationBuilder.DropTable(name: "ProjectResourceMaps");
            migrationBuilder.DropTable(name: "ProjectMembers");
            migrationBuilder.DropTable(name: "Projects");
        }
    }
}
