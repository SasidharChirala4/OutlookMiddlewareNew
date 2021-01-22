using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Edreams.OutlookMiddleware.DataAccess.Migrations.OutlookMiddlewareDb
{
    public partial class Transaction_And_TransactionHistory : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TransactionHistory",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    SysId = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    InsertedOn = table.Column<DateTime>(nullable: false),
                    UpdatedOn = table.Column<DateTime>(nullable: true),
                    InsertedBy = table.Column<string>(maxLength: 100, nullable: false),
                    UpdatedBy = table.Column<string>(maxLength: 100, nullable: true),
                    BatchId = table.Column<Guid>(nullable: false),
                    ReleaseDate = table.Column<DateTime>(nullable: true),
                    Status = table.Column<string>(nullable: false),
                    Scheduled = table.Column<DateTime>(nullable: true),
                    ProcessingStarted = table.Column<DateTime>(nullable: true),
                    ProcessingFinished = table.Column<DateTime>(nullable: true),
                    ProcessingEngine = table.Column<string>(maxLength: 100, nullable: true),
                    CorrelationId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TransactionHistory", x => x.Id)
                        .Annotation("SqlServer:Clustered", false);
                });

            migrationBuilder.CreateTable(
                name: "Transactions",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    SysId = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    InsertedOn = table.Column<DateTime>(nullable: false),
                    UpdatedOn = table.Column<DateTime>(nullable: true),
                    InsertedBy = table.Column<string>(maxLength: 100, nullable: false),
                    UpdatedBy = table.Column<string>(maxLength: 100, nullable: true),
                    BatchId = table.Column<Guid>(nullable: false),
                    ReleaseDate = table.Column<DateTime>(nullable: true),
                    Status = table.Column<string>(nullable: false),
                    Scheduled = table.Column<DateTime>(nullable: true),
                    ProcessingStarted = table.Column<DateTime>(nullable: true),
                    ProcessingFinished = table.Column<DateTime>(nullable: true),
                    ProcessingEngine = table.Column<string>(maxLength: 100, nullable: true),
                    CorrelationId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Transactions", x => x.Id)
                        .Annotation("SqlServer:Clustered", false);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TransactionHistory_SysId",
                table: "TransactionHistory",
                column: "SysId",
                unique: true)
                .Annotation("SqlServer:Clustered", true);

            migrationBuilder.CreateIndex(
                name: "IX_Transactions_SysId",
                table: "Transactions",
                column: "SysId",
                unique: true)
                .Annotation("SqlServer:Clustered", true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TransactionHistory");

            migrationBuilder.DropTable(
                name: "Transactions");
        }
    }
}