using Microsoft.EntityFrameworkCore.Migrations;

namespace Edreams.OutlookMiddleware.DataAccess.Migrations.OutlookMiddlewareDb
{
    public partial class Additional_Migration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AttachmentId",
                table: "Files",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "EmailSubject",
                table: "Files",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AttachmentId",
                table: "Files");

            migrationBuilder.DropColumn(
                name: "EmailSubject",
                table: "Files");
        }
    }
}
