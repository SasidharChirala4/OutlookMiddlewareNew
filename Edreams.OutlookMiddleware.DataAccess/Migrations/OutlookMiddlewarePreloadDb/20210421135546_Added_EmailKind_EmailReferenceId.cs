using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Edreams.OutlookMiddleware.DataAccess.Migrations.OutlookMiddlewarePreloadDb
{
    public partial class Added_EmailKind_EmailReferenceId : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "EmailKind",
                table: "PreloadedFiles",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<Guid>(
                name: "EmailReferenceId",
                table: "PreloadedFiles",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EmailKind",
                table: "PreloadedFiles");

            migrationBuilder.DropColumn(
                name: "EmailReferenceId",
                table: "PreloadedFiles");

        }
    }
}
