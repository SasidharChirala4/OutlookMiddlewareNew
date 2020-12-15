using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Edreams.OutlookMiddleware.DataAccess.Migrations.OutlookMiddlewareDb
{
    public partial class Added_CategorizationRequests : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "InsertedBy",
                table: "Files",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "InsertedOn",
                table: "Files",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "UpdatedBy",
                table: "Files",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedOn",
                table: "Files",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "InsertedBy",
                table: "Emails",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "InsertedOn",
                table: "Emails",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "UpdatedBy",
                table: "Emails",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedOn",
                table: "Emails",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "InsertedBy",
                table: "Batches",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "InsertedOn",
                table: "Batches",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "UpdatedBy",
                table: "Batches",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedOn",
                table: "Batches",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "CategorizationRequest",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    SysId = table.Column<long>(nullable: false),
                    InsertedOn = table.Column<DateTime>(nullable: false),
                    UpdatedOn = table.Column<DateTime>(nullable: true),
                    InsertedBy = table.Column<string>(maxLength: 100, nullable: false),
                    UpdatedBy = table.Column<string>(maxLength: 100, nullable: false),
                    UserPrincipalName = table.Column<string>(maxLength: 200, nullable: false),
                    InternetMessageId = table.Column<string>(maxLength: 200, nullable: false),
                    IsCompose = table.Column<bool>(nullable: false),
                    Sent = table.Column<bool>(nullable: false),
                    CategorizationRequestType = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CategorizationRequest", x => x.Id)
                        .Annotation("SqlServer:Clustered", false);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CategorizationRequest_SysId",
                table: "CategorizationRequest",
                column: "SysId",
                unique: true)
                .Annotation("SqlServer:Clustered", true);

            migrationBuilder.CreateIndex(
                name: "IX_CategorizationRequest_UserPrincipalName",
                table: "CategorizationRequest",
                column: "UserPrincipalName");

            migrationBuilder.CreateIndex(
                name: "IX_CategorizationRequest_UserPrincipalName_Sent_CategorizationRequestType_InternetMessageId",
                table: "CategorizationRequest",
                columns: new[] { "UserPrincipalName", "Sent", "CategorizationRequestType", "InternetMessageId" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CategorizationRequest");

            migrationBuilder.DropColumn(
                name: "InsertedBy",
                table: "Files");

            migrationBuilder.DropColumn(
                name: "InsertedOn",
                table: "Files");

            migrationBuilder.DropColumn(
                name: "UpdatedBy",
                table: "Files");

            migrationBuilder.DropColumn(
                name: "UpdatedOn",
                table: "Files");

            migrationBuilder.DropColumn(
                name: "InsertedBy",
                table: "Emails");

            migrationBuilder.DropColumn(
                name: "InsertedOn",
                table: "Emails");

            migrationBuilder.DropColumn(
                name: "UpdatedBy",
                table: "Emails");

            migrationBuilder.DropColumn(
                name: "UpdatedOn",
                table: "Emails");

            migrationBuilder.DropColumn(
                name: "InsertedBy",
                table: "Batches");

            migrationBuilder.DropColumn(
                name: "InsertedOn",
                table: "Batches");

            migrationBuilder.DropColumn(
                name: "UpdatedBy",
                table: "Batches");

            migrationBuilder.DropColumn(
                name: "UpdatedOn",
                table: "Batches");
        }
    }
}
