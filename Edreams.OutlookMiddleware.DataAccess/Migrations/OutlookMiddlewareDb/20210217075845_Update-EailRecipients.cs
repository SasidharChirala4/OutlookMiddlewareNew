using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Edreams.OutlookMiddleware.DataAccess.Migrations.OutlookMiddlewareDb
{
    public partial class UpdateEailRecipients : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Emails_EmailRecipients_EmailRecipientId",
                table: "Emails");

            migrationBuilder.DropIndex(
                name: "IX_Emails_EmailRecipientId",
                table: "Emails");

            migrationBuilder.DropColumn(
                name: "EmailRecipientId",
                table: "Emails");

            migrationBuilder.AddColumn<Guid>(
                name: "EmailId",
                table: "EmailRecipients",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_EmailRecipients_EmailId",
                table: "EmailRecipients",
                column: "EmailId");

            migrationBuilder.AddForeignKey(
                name: "FK_EmailRecipients_Emails_EmailId",
                table: "EmailRecipients",
                column: "EmailId",
                principalTable: "Emails",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EmailRecipients_Emails_EmailId",
                table: "EmailRecipients");

            migrationBuilder.DropIndex(
                name: "IX_EmailRecipients_EmailId",
                table: "EmailRecipients");

            migrationBuilder.DropColumn(
                name: "EmailId",
                table: "EmailRecipients");

            migrationBuilder.AddColumn<Guid>(
                name: "EmailRecipientId",
                table: "Emails",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Emails_EmailRecipientId",
                table: "Emails",
                column: "EmailRecipientId");

            migrationBuilder.AddForeignKey(
                name: "FK_Emails_EmailRecipients_EmailRecipientId",
                table: "Emails",
                column: "EmailRecipientId",
                principalTable: "EmailRecipients",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
