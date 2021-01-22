using Microsoft.EntityFrameworkCore.Migrations;

namespace Edreams.OutlookMiddleware.DataAccess.Migrations.OutlookMiddlewareDb
{
    public partial class Rename_Transactions_To_TransactionQueue : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_Transactions",
                table: "Transactions");

            migrationBuilder.RenameTable(
                name: "Transactions",
                newName: "TransactionQueue");

            migrationBuilder.RenameIndex(
                name: "IX_Transactions_SysId",
                table: "TransactionQueue",
                newName: "IX_TransactionQueue_SysId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_TransactionQueue",
                table: "TransactionQueue",
                column: "Id")
                .Annotation("SqlServer:Clustered", false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_TransactionQueue",
                table: "TransactionQueue");

            migrationBuilder.RenameTable(
                name: "TransactionQueue",
                newName: "Transactions");

            migrationBuilder.RenameIndex(
                name: "IX_TransactionQueue_SysId",
                table: "Transactions",
                newName: "IX_Transactions_SysId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Transactions",
                table: "Transactions",
                column: "Id")
                .Annotation("SqlServer:Clustered", false);
        }
    }
}
