using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Edreams.OutlookMiddleware.DataAccess.Migrations.OutlookMiddlewareDb
{
    public partial class FixedMetadataColumnsToEmailFileTables : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_ProjectTasks_EmailId",
                table: "ProjectTasks");

            migrationBuilder.DropColumn(
                name: "UploadOption",
                table: "Emails");

            migrationBuilder.AddColumn<string>(
                name: "NewName",
                table: "Files",
                maxLength: 255,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OriginalName",
                table: "Files",
                maxLength: 255,
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "ShouldUpload",
                table: "Files",
                nullable: false,
                defaultValue: true);

            migrationBuilder.AddColumn<string>(
                name: "UploadLocationFolder",
                table: "Batches",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "UploadLocationProjectId",
                table: "Batches",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UploadLocationSite",
                table: "Batches",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UploadOption",
                table: "Batches",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectTasks_EmailId",
                table: "ProjectTasks",
                column: "EmailId",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_ProjectTasks_EmailId",
                table: "ProjectTasks");

            migrationBuilder.DropColumn(
                name: "NewName",
                table: "Files");

            migrationBuilder.DropColumn(
                name: "OriginalName",
                table: "Files");

            migrationBuilder.DropColumn(
                name: "ShouldUpload",
                table: "Files");

            migrationBuilder.DropColumn(
                name: "UploadLocationFolder",
                table: "Batches");

            migrationBuilder.DropColumn(
                name: "UploadLocationProjectId",
                table: "Batches");

            migrationBuilder.DropColumn(
                name: "UploadLocationSite",
                table: "Batches");

            migrationBuilder.DropColumn(
                name: "UploadOption",
                table: "Batches");

            migrationBuilder.AddColumn<string>(
                name: "UploadOption",
                table: "Emails",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectTasks_EmailId",
                table: "ProjectTasks",
                column: "EmailId");
        }
    }
}
