using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace Nssol.Platypus.Migrations
{
    public partial class AddPipeline : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Pipelines",
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
                    Memo = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Pipelines", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Pipelines_Tenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "Tenants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PipelineNodes",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    CreatedBy = table.Column<string>(nullable: false),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    ModifiedBy = table.Column<string>(nullable: false),
                    ModifiedAt = table.Column<DateTime>(nullable: false),
                    TenantId = table.Column<long>(nullable: false),
                    PipelineId = table.Column<long>(nullable: false),
                    Name = table.Column<string>(nullable: false),
                    JobType = table.Column<string>(nullable: false),
                    PositionX = table.Column<int>(nullable: false),
                    PositionY = table.Column<int>(nullable: false),
                    JobParams = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PipelineNodes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PipelineNodes_Pipelines_PipelineId",
                        column: x => x.PipelineId,
                        principalTable: "Pipelines",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PipelineNodes_Tenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "Tenants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PipelineEdges",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    CreatedBy = table.Column<string>(nullable: false),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    ModifiedBy = table.Column<string>(nullable: false),
                    ModifiedAt = table.Column<DateTime>(nullable: false),
                    TenantId = table.Column<long>(nullable: false),
                    PipelineId = table.Column<long>(nullable: false),
                    SourceNodeId = table.Column<long>(nullable: false),
                    TargetNodeId = table.Column<long>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PipelineEdges", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PipelineEdges_Pipelines_PipelineId",
                        column: x => x.PipelineId,
                        principalTable: "Pipelines",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PipelineEdges_PipelineNodes_SourceNodeId",
                        column: x => x.SourceNodeId,
                        principalTable: "PipelineNodes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PipelineEdges_PipelineNodes_TargetNodeId",
                        column: x => x.TargetNodeId,
                        principalTable: "PipelineNodes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PipelineEdges_Tenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "Tenants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PipelineRuns",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    CreatedBy = table.Column<string>(nullable: false),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    ModifiedBy = table.Column<string>(nullable: false),
                    ModifiedAt = table.Column<DateTime>(nullable: false),
                    TenantId = table.Column<long>(nullable: false),
                    PipelineId = table.Column<long>(nullable: false),
                    Status = table.Column<string>(nullable: false),
                    StartedAt = table.Column<DateTime>(nullable: true),
                    CompletedAt = table.Column<DateTime>(nullable: true),
                    Memo = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PipelineRuns", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PipelineRuns_Pipelines_PipelineId",
                        column: x => x.PipelineId,
                        principalTable: "Pipelines",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PipelineRuns_Tenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "Tenants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PipelineRunSteps",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    CreatedBy = table.Column<string>(nullable: false),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    ModifiedBy = table.Column<string>(nullable: false),
                    ModifiedAt = table.Column<DateTime>(nullable: false),
                    TenantId = table.Column<long>(nullable: false),
                    PipelineRunId = table.Column<long>(nullable: false),
                    PipelineNodeId = table.Column<long>(nullable: false),
                    Status = table.Column<string>(nullable: false),
                    TrainingHistoryId = table.Column<long>(nullable: true),
                    InferenceHistoryId = table.Column<long>(nullable: true),
                    PreprocessHistoryId = table.Column<long>(nullable: true),
                    StartedAt = table.Column<DateTime>(nullable: true),
                    CompletedAt = table.Column<DateTime>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PipelineRunSteps", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PipelineRunSteps_InferenceHistories_InferenceHistoryId",
                        column: x => x.InferenceHistoryId,
                        principalTable: "InferenceHistories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PipelineRunSteps_PipelineNodes_PipelineNodeId",
                        column: x => x.PipelineNodeId,
                        principalTable: "PipelineNodes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PipelineRunSteps_PipelineRuns_PipelineRunId",
                        column: x => x.PipelineRunId,
                        principalTable: "PipelineRuns",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PipelineRunSteps_PreprocessHistories_PreprocessHistoryId",
                        column: x => x.PreprocessHistoryId,
                        principalTable: "PreprocessHistories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PipelineRunSteps_Tenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "Tenants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PipelineRunSteps_TrainingHistories_TrainingHistoryId",
                        column: x => x.TrainingHistoryId,
                        principalTable: "TrainingHistories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Pipelines_TenantId",
                table: "Pipelines",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_PipelineNodes_PipelineId",
                table: "PipelineNodes",
                column: "PipelineId");

            migrationBuilder.CreateIndex(
                name: "IX_PipelineNodes_TenantId",
                table: "PipelineNodes",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_PipelineEdges_PipelineId",
                table: "PipelineEdges",
                column: "PipelineId");

            migrationBuilder.CreateIndex(
                name: "IX_PipelineEdges_SourceNodeId",
                table: "PipelineEdges",
                column: "SourceNodeId");

            migrationBuilder.CreateIndex(
                name: "IX_PipelineEdges_TargetNodeId",
                table: "PipelineEdges",
                column: "TargetNodeId");

            migrationBuilder.CreateIndex(
                name: "IX_PipelineEdges_TenantId",
                table: "PipelineEdges",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_PipelineRuns_PipelineId",
                table: "PipelineRuns",
                column: "PipelineId");

            migrationBuilder.CreateIndex(
                name: "IX_PipelineRuns_TenantId",
                table: "PipelineRuns",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_PipelineRunSteps_InferenceHistoryId",
                table: "PipelineRunSteps",
                column: "InferenceHistoryId");

            migrationBuilder.CreateIndex(
                name: "IX_PipelineRunSteps_PipelineNodeId",
                table: "PipelineRunSteps",
                column: "PipelineNodeId");

            migrationBuilder.CreateIndex(
                name: "IX_PipelineRunSteps_PipelineRunId",
                table: "PipelineRunSteps",
                column: "PipelineRunId");

            migrationBuilder.CreateIndex(
                name: "IX_PipelineRunSteps_PreprocessHistoryId",
                table: "PipelineRunSteps",
                column: "PreprocessHistoryId");

            migrationBuilder.CreateIndex(
                name: "IX_PipelineRunSteps_TenantId",
                table: "PipelineRunSteps",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_PipelineRunSteps_TrainingHistoryId",
                table: "PipelineRunSteps",
                column: "TrainingHistoryId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(name: "PipelineRunSteps");
            migrationBuilder.DropTable(name: "PipelineEdges");
            migrationBuilder.DropTable(name: "PipelineRuns");
            migrationBuilder.DropTable(name: "PipelineNodes");
            migrationBuilder.DropTable(name: "Pipelines");
        }
    }
}
