using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Edreams.OutlookMiddleware.DataAccess.Migrations.OutlookMiddlewarePreloadDb
{
    public partial class Added_InternetMessageId_AuditFields : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "InsertedBy",
                table: "PreloadedFiles",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "InsertedOn",
                table: "PreloadedFiles",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "InternetMessageId",
                table: "PreloadedFiles",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UpdatedBy",
                table: "PreloadedFiles",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedOn",
                table: "PreloadedFiles",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "InsertedBy",
                table: "PreloadedFiles");

            migrationBuilder.DropColumn(
                name: "InsertedOn",
                table: "PreloadedFiles");

            migrationBuilder.DropColumn(
                name: "InternetMessageId",
                table: "PreloadedFiles");

            migrationBuilder.DropColumn(
                name: "UpdatedBy",
                table: "PreloadedFiles");

            migrationBuilder.DropColumn(
                name: "UpdatedOn",
                table: "PreloadedFiles");
        }
    }
}
