using Microsoft.EntityFrameworkCore.Migrations;

namespace Edreams.OutlookMiddleware.DataAccess.Migrations.OutlookMiddlewarePreloadDb
{
    public partial class Add_Principalname_PreloadedFiles_Table : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "PrincipalName",
                table: "PreloadedFiles",
                maxLength: 100,
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PrincipalName",
                table: "PreloadedFiles");
        }
    }
}
