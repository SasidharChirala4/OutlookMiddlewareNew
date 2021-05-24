using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Edreams.OutlookMiddleware.DataAccess.Migrations.OutlookMiddlewareDb
{
    public partial class Added_Task_Table : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "UpdatedBy",
                table: "CategorizationRequest",
                maxLength: 100,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            migrationBuilder.CreateTable(
                name: "ProjectTasks",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    SysId = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    InsertedOn = table.Column<DateTime>(nullable: false),
                    UpdatedOn = table.Column<DateTime>(nullable: true),
                    InsertedBy = table.Column<string>(maxLength: 100, nullable: false),
                    UpdatedBy = table.Column<string>(maxLength: 100, nullable: true),
                    TaskName = table.Column<string>(maxLength: 200, nullable: true),
                    DueDate = table.Column<DateTime>(nullable: true),
                    Description = table.Column<string>(maxLength: 300, nullable: true),
                    Priority = table.Column<string>(maxLength: 10, nullable: false),
                    EmailId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProjectTasks", x => x.Id)
                        .Annotation("SqlServer:Clustered", false);
                    table.ForeignKey(
                        name: "FK_ProjectTasks_Emails_EmailId",
                        column: x => x.EmailId,
                        principalTable: "Emails",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProjectTaskUserInvolvements",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    SysId = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    InsertedOn = table.Column<DateTime>(nullable: false),
                    UpdatedOn = table.Column<DateTime>(nullable: true),
                    InsertedBy = table.Column<string>(maxLength: 100, nullable: false),
                    UpdatedBy = table.Column<string>(maxLength: 100, nullable: true),
                    UserId = table.Column<string>(maxLength: 100, nullable: true),
                    PrincipalName = table.Column<string>(maxLength: 100, nullable: true),
                    Type = table.Column<string>(maxLength: 10, nullable: false),
                    ProjectTaskId = table.Column<Guid>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProjectTaskUserInvolvements", x => x.Id)
                        .Annotation("SqlServer:Clustered", false);
                    table.ForeignKey(
                        name: "FK_ProjectTaskUserInvolvements_ProjectTasks_ProjectTaskId",
                        column: x => x.ProjectTaskId,
                        principalTable: "ProjectTasks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ProjectTasks_EmailId",
                table: "ProjectTasks",
                column: "EmailId");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectTasks_SysId",
                table: "ProjectTasks",
                column: "SysId",
                unique: true)
                .Annotation("SqlServer:Clustered", true);

            migrationBuilder.CreateIndex(
                name: "IX_ProjectTaskUserInvolvements_ProjectTaskId",
                table: "ProjectTaskUserInvolvements",
                column: "ProjectTaskId");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectTaskUserInvolvements_SysId",
                table: "ProjectTaskUserInvolvements",
                column: "SysId",
                unique: true)
                .Annotation("SqlServer:Clustered", true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ProjectTaskUserInvolvements");

            migrationBuilder.DropTable(
                name: "ProjectTasks");

            migrationBuilder.AlterColumn<string>(
                name: "UpdatedBy",
                table: "CategorizationRequest",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldMaxLength: 100,
                oldNullable: true);
        }
    }
}
