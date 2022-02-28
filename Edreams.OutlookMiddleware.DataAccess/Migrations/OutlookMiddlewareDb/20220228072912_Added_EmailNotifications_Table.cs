using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Edreams.OutlookMiddleware.DataAccess.Migrations.OutlookMiddlewareDb
{
    public partial class Added_EmailNotifications_Table : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "EmailNotifications",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    SysId = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    InsertedOn = table.Column<DateTime>(nullable: false),
                    UpdatedOn = table.Column<DateTime>(nullable: true),
                    InsertedBy = table.Column<string>(maxLength: 100, nullable: false),
                    UpdatedBy = table.Column<string>(maxLength: 100, nullable: true),
                    NotificationSent = table.Column<bool>(nullable: true),
                    BatchId = table.Column<Guid>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmailNotifications", x => x.Id)
                        .Annotation("SqlServer:Clustered", false);
                    table.ForeignKey(
                        name: "FK_EmailNotifications_Batches_BatchId",
                        column: x => x.BatchId,
                        principalTable: "Batches",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_EmailNotifications_BatchId",
                table: "EmailNotifications",
                column: "BatchId");

            migrationBuilder.CreateIndex(
                name: "IX_EmailNotifications_NotificationSent",
                table: "EmailNotifications",
                column: "NotificationSent");

            migrationBuilder.CreateIndex(
                name: "IX_EmailNotifications_SysId",
                table: "EmailNotifications",
                column: "SysId",
                unique: true)
                .Annotation("SqlServer:Clustered", true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EmailNotifications");
        }
    }
}
