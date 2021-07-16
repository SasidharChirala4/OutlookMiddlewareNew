using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Edreams.OutlookMiddleware.DataAccess.Migrations.OutlookMiddlewareDb
{
    public partial class Updated_BatchFileProjectTasks_ForFixedMetadata : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FileName",
                table: "Files");

            migrationBuilder.DropColumn(
                name: "UploadLocationProjectId",
                table: "Batches");

            migrationBuilder.AddColumn<Guid>(
                name: "UploadLocationProjectId",
                table: "ProjectTasks",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UploadLocationProjectId",
                table: "ProjectTasks");

            migrationBuilder.AddColumn<string>(
                name: "FileName",
                table: "Files",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "UploadLocationProjectId",
                table: "Batches",
                type: "uniqueidentifier",
                nullable: true);
        }
    }
}
