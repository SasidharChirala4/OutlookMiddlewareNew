using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Edreams.OutlookMiddleware.DataAccess.Migrations.OutlookMiddlewarePreloadDb
{
    public partial class Added_PreloadedOn : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "PreloadedOn",
                table: "PreloadedFiles",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PreloadedOn",
                table: "PreloadedFiles");
        }
    }
}
