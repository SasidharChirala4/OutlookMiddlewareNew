using Microsoft.EntityFrameworkCore.Migrations;

namespace Edreams.OutlookMiddleware.DataAccess.Migrations.OutlookMiddlewareDb
{
    public partial class Added_TransactionType_To_Transactions : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_CategorizationRequest_EmailAddress_Status_CategorizationRequestType_InternetMessageId",
                table: "CategorizationRequest");

            migrationBuilder.DropColumn(
                name: "CategorizationRequestType",
                table: "CategorizationRequest");

            migrationBuilder.AddColumn<string>(
                name: "Type",
                table: "TransactionQueue",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Type",
                table: "TransactionHistory",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<string>(
                name: "Type",
                table: "CategorizationRequest",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "CategorizationRequest",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Type",
                table: "TransactionQueue");

            migrationBuilder.DropColumn(
                name: "Type",
                table: "TransactionHistory");

            migrationBuilder.AlterColumn<int>(
                name: "Type",
                table: "CategorizationRequest",
                type: "int",
                nullable: false,
                oldClrType: typeof(string));

            migrationBuilder.AlterColumn<int>(
                name: "Status",
                table: "CategorizationRequest",
                type: "int",
                nullable: false,
                oldClrType: typeof(string));

            migrationBuilder.AddColumn<int>(
                name: "CategorizationRequestType",
                table: "CategorizationRequest",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_CategorizationRequest_EmailAddress_Status_CategorizationRequestType_InternetMessageId",
                table: "CategorizationRequest",
                columns: new[] { "EmailAddress", "Status", "CategorizationRequestType", "InternetMessageId" });
        }
    }
}
