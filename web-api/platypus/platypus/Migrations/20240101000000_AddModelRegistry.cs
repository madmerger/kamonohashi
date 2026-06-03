using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace Nssol.Platypus.Migrations
{
    public partial class AddModelRegistry : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Models",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    CreatedBy = table.Column<string>(nullable: false),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    ModifiedBy = table.Column<string>(nullable: false),
                    ModifiedAt = table.Column<DateTime>(nullable: false),
                    TenantId = table.Column<long>(nullable: false),
                    Name = table.Column<string>(nullable: false),
                    Description = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Models", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Models_Tenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "Tenants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ModelVersions",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    CreatedBy = table.Column<string>(nullable: false),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    ModifiedBy = table.Column<string>(nullable: false),
                    ModifiedAt = table.Column<DateTime>(nullable: false),
                    TenantId = table.Column<long>(nullable: false),
                    ModelId = table.Column<long>(nullable: false),
                    Version = table.Column<int>(nullable: false),
                    TrainingHistoryId = table.Column<long>(nullable: true),
                    Accuracy = table.Column<float>(nullable: true),
                    Status = table.Column<string>(nullable: false),
                    Description = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ModelVersions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ModelVersions_Models_ModelId",
                        column: x => x.ModelId,
                        principalTable: "Models",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ModelVersions_Tenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "Tenants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ModelVersions_TrainingHistories_TrainingHistoryId",
                        column: x => x.TrainingHistoryId,
                        principalTable: "TrainingHistories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Models_TenantId_Name",
                table: "Models",
                columns: new[] { "TenantId", "Name" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ModelVersions_ModelId_Version",
                table: "ModelVersions",
                columns: new[] { "ModelId", "Version" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ModelVersions_TenantId",
                table: "ModelVersions",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_ModelVersions_TrainingHistoryId",
                table: "ModelVersions",
                column: "TrainingHistoryId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ModelVersions");

            migrationBuilder.DropTable(
                name: "Models");
        }
    }
}
