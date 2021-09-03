using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Edreams.OutlookMiddleware.DataAccess.Migrations.OutlookMiddlewarePreloadDb
{
    public partial class Renamed_EmailReferenceId_To_EdreamsReferenceId : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EmailReferenceId",
                table: "PreloadedFiles");

            migrationBuilder.AddColumn<Guid>(
                name: "EdreamsReferenceId",
                table: "PreloadedFiles",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EdreamsReferenceId",
                table: "PreloadedFiles");

            migrationBuilder.AddColumn<Guid>(
                name: "EmailReferenceId",
                table: "PreloadedFiles",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));
        }
    }
}
