using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace Nssol.Platypus.Migrations
{
    public partial class AddHpoTables : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "HpoJobs",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    CreatedBy = table.Column<string>(nullable: true),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    ModifiedBy = table.Column<string>(nullable: true),
                    ModifiedAt = table.Column<DateTime>(nullable: false),
                    TenantId = table.Column<long>(nullable: false),
                    DisplayId = table.Column<long>(nullable: true),
                    Name = table.Column<string>(nullable: false),
                    Algorithm = table.Column<string>(nullable: false),
                    SearchSpace = table.Column<string>(nullable: false),
                    MaxTrials = table.Column<int>(nullable: false),
                    ObjectiveMetric = table.Column<string>(nullable: false),
                    OptimizationDirection = table.Column<string>(nullable: false),
                    DataSetId = table.Column<long>(nullable: false),
                    ModelGitId = table.Column<long>(nullable: false),
                    ModelRepository = table.Column<string>(nullable: false),
                    ModelRepositoryOwner = table.Column<string>(nullable: false),
                    ModelBranch = table.Column<string>(nullable: true),
                    ModelCommitId = table.Column<string>(nullable: false),
                    EntryPoint = table.Column<string>(nullable: false),
                    ContainerRegistryId = table.Column<long>(nullable: true),
                    ContainerImage = table.Column<string>(nullable: false),
                    ContainerTag = table.Column<string>(nullable: false),
                    Cpu = table.Column<int>(nullable: false),
                    Memory = table.Column<int>(nullable: false),
                    Gpu = table.Column<int>(nullable: false),
                    Partition = table.Column<string>(nullable: true),
                    Status = table.Column<string>(nullable: false),
                    StartedAt = table.Column<DateTime>(nullable: true),
                    CompletedAt = table.Column<DateTime>(nullable: true),
                    Memo = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HpoJobs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HpoJobs_DataSets_DataSetId",
                        column: x => x.DataSetId,
                        principalTable: "DataSets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_HpoJobs_Tenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "Tenants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "HpoTrials",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    CreatedBy = table.Column<string>(nullable: true),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    ModifiedBy = table.Column<string>(nullable: true),
                    ModifiedAt = table.Column<DateTime>(nullable: false),
                    TenantId = table.Column<long>(nullable: false),
                    TrialNo = table.Column<int>(nullable: false),
                    HpoJobId = table.Column<long>(nullable: false),
                    TrainingHistoryId = table.Column<long>(nullable: true),
                    Parameters = table.Column<string>(nullable: false),
                    MetricValue = table.Column<double>(nullable: true),
                    Status = table.Column<string>(nullable: false),
                    StartedAt = table.Column<DateTime>(nullable: true),
                    CompletedAt = table.Column<DateTime>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HpoTrials", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HpoTrials_HpoJobs_HpoJobId",
                        column: x => x.HpoJobId,
                        principalTable: "HpoJobs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_HpoTrials_TrainingHistories_TrainingHistoryId",
                        column: x => x.TrainingHistoryId,
                        principalTable: "TrainingHistories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_HpoTrials_Tenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "Tenants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_HpoJobs_DataSetId",
                table: "HpoJobs",
                column: "DataSetId");

            migrationBuilder.CreateIndex(
                name: "IX_HpoJobs_TenantId",
                table: "HpoJobs",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_HpoTrials_HpoJobId",
                table: "HpoTrials",
                column: "HpoJobId");

            migrationBuilder.CreateIndex(
                name: "IX_HpoTrials_TrainingHistoryId",
                table: "HpoTrials",
                column: "TrainingHistoryId");

            migrationBuilder.CreateIndex(
                name: "IX_HpoTrials_TenantId",
                table: "HpoTrials",
                column: "TenantId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(name: "HpoTrials");
            migrationBuilder.DropTable(name: "HpoJobs");
        }
    }
}
