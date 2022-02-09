using Microsoft.EntityFrameworkCore.Migrations;

namespace Edreams.OutlookMiddleware.DataAccess.Migrations.OutlookMiddlewareDb
{
    public partial class AddedFileExtension : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Extension",
                table: "Files",
                maxLength: 100,
                nullable: true);

            migrationBuilder.DropIndex(
                name: "IX_CategorizationRequest_SysId",
                table: "CategorizationRequest");

            migrationBuilder.DropColumn(
                name: "SysId",
                table: "CategorizationRequest");

            migrationBuilder.AddColumn<long>(
                name: "SysId",
                table: "CategorizationRequest",
                nullable: false)
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.CreateIndex(
                    name: "IX_CategorizationRequest_SysId",
                    table: "CategorizationRequest",
                    column: "SysId",
                    unique: true)
                .Annotation("SqlServer:Clustered", true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Extension",
                table: "Files");

            migrationBuilder.AlterColumn<long>(
                name: "SysId",
                table: "CategorizationRequest",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(long))
                .OldAnnotation("SqlServer:Identity", "1, 1");
        }
    }
}