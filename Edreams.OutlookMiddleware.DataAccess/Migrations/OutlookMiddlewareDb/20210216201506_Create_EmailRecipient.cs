using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Edreams.OutlookMiddleware.DataAccess.Migrations.OutlookMiddlewareDb
{
    public partial class Create_EmailRecipient : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_CategorizationRequest_UserPrincipalName",
                table: "CategorizationRequest");

            migrationBuilder.DropIndex(
                name: "IX_CategorizationRequest_UserPrincipalName_Sent_CategorizationRequestType_InternetMessageId",
                table: "CategorizationRequest");

            migrationBuilder.DropColumn(
                name: "IsCompose",
                table: "CategorizationRequest");

            migrationBuilder.DropColumn(
                name: "Sent",
                table: "CategorizationRequest");

            migrationBuilder.DropColumn(
                name: "UserPrincipalName",
                table: "CategorizationRequest");

            migrationBuilder.AddColumn<Guid>(
                name: "EmailRecipientId",
                table: "Emails",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "InternetMessageId",
                table: "Emails",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "EmailAddress",
                table: "CategorizationRequest",
                maxLength: 200,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "CategorizationRequest",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Type",
                table: "CategorizationRequest",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "EmailRecipients",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    SysId = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    InsertedOn = table.Column<DateTime>(nullable: false),
                    UpdatedOn = table.Column<DateTime>(nullable: true),
                    InsertedBy = table.Column<string>(maxLength: 100, nullable: false),
                    UpdatedBy = table.Column<string>(maxLength: 100, nullable: true),
                    Recipient = table.Column<string>(maxLength: 200, nullable: false),
                    Type = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmailRecipients", x => x.Id)
                        .Annotation("SqlServer:Clustered", false);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Emails_EmailRecipientId",
                table: "Emails",
                column: "EmailRecipientId");

            migrationBuilder.CreateIndex(
                name: "IX_Emails_InternetMessageId",
                table: "Emails",
                column: "InternetMessageId");

            migrationBuilder.CreateIndex(
                name: "IX_CategorizationRequest_EmailAddress",
                table: "CategorizationRequest",
                column: "EmailAddress");

            migrationBuilder.CreateIndex(
                name: "IX_CategorizationRequest_EmailAddress_Status_CategorizationRequestType_InternetMessageId",
                table: "CategorizationRequest",
                columns: new[] { "EmailAddress", "Status", "CategorizationRequestType", "InternetMessageId" });

            migrationBuilder.CreateIndex(
                name: "IX_EmailRecipients_SysId",
                table: "EmailRecipients",
                column: "SysId",
                unique: true)
                .Annotation("SqlServer:Clustered", true);

            migrationBuilder.AddForeignKey(
                name: "FK_Emails_EmailRecipients_EmailRecipientId",
                table: "Emails",
                column: "EmailRecipientId",
                principalTable: "EmailRecipients",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Emails_EmailRecipients_EmailRecipientId",
                table: "Emails");

            migrationBuilder.DropTable(
                name: "EmailRecipients");

            migrationBuilder.DropIndex(
                name: "IX_Emails_EmailRecipientId",
                table: "Emails");

            migrationBuilder.DropIndex(
                name: "IX_Emails_InternetMessageId",
                table: "Emails");

            migrationBuilder.DropIndex(
                name: "IX_CategorizationRequest_EmailAddress",
                table: "CategorizationRequest");

            migrationBuilder.DropIndex(
                name: "IX_CategorizationRequest_EmailAddress_Status_CategorizationRequestType_InternetMessageId",
                table: "CategorizationRequest");

            migrationBuilder.DropColumn(
                name: "EmailRecipientId",
                table: "Emails");

            migrationBuilder.DropColumn(
                name: "InternetMessageId",
                table: "Emails");

            migrationBuilder.DropColumn(
                name: "EmailAddress",
                table: "CategorizationRequest");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "CategorizationRequest");

            migrationBuilder.DropColumn(
                name: "Type",
                table: "CategorizationRequest");

            migrationBuilder.AddColumn<bool>(
                name: "IsCompose",
                table: "CategorizationRequest",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "Sent",
                table: "CategorizationRequest",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "UserPrincipalName",
                table: "CategorizationRequest",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_CategorizationRequest_UserPrincipalName",
                table: "CategorizationRequest",
                column: "UserPrincipalName");

            migrationBuilder.CreateIndex(
                name: "IX_CategorizationRequest_UserPrincipalName_Sent_CategorizationRequestType_InternetMessageId",
                table: "CategorizationRequest",
                columns: new[] { "UserPrincipalName", "Sent", "CategorizationRequestType", "InternetMessageId" });
        }
    }
}
