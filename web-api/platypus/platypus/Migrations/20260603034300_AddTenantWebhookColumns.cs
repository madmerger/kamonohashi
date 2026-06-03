using Microsoft.EntityFrameworkCore.Migrations;

namespace Nssol.Platypus.Migrations
{
    public partial class AddTenantWebhookColumns : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "WebhookUrl",
                table: "Tenants",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SlackNotificationTemplate",
                table: "Tenants",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "WebhookNotificationTemplate",
                table: "Tenants",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "WebhookUrl",
                table: "Tenants");

            migrationBuilder.DropColumn(
                name: "SlackNotificationTemplate",
                table: "Tenants");

            migrationBuilder.DropColumn(
                name: "WebhookNotificationTemplate",
                table: "Tenants");
        }
    }
}
