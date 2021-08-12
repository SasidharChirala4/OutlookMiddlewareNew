using Microsoft.EntityFrameworkCore.Migrations;

namespace Edreams.OutlookMiddleware.DataAccess.Migrations.OutlookMiddlewareDb
{
    public partial class Vertioncomment_DeclareAsRecords_Batch_Table : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "DeclareAsRecord",
                table: "Batches",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "VersionComment",
                table: "Batches",
                maxLength: 255,
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DeclareAsRecord",
                table: "Batches");

            migrationBuilder.DropColumn(
                name: "VersionComment",
                table: "Batches");
        }
    }
}
