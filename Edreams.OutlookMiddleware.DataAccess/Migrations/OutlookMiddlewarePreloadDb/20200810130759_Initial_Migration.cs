using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Edreams.OutlookMiddleware.DataAccess.Migrations.OutlookMiddlewarePreloadDb
{
    public partial class Initial_Migration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PreloadedFiles",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    SysId = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BatchId = table.Column<Guid>(nullable: false),
                    EmailId = table.Column<Guid>(nullable: false),
                    EntryId = table.Column<string>(nullable: true),
                    EwsId = table.Column<string>(nullable: true),
                    TempPath = table.Column<string>(nullable: true),
                    FileName = table.Column<string>(nullable: true),
                    Size = table.Column<long>(nullable: false),
                    Kind = table.Column<string>(nullable: false),
                    Status = table.Column<string>(nullable: false),
                    FileStatus = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PreloadedFiles", x => x.Id)
                        .Annotation("SqlServer:Clustered", false);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PreloadedFiles_BatchId",
                table: "PreloadedFiles",
                column: "BatchId");

            migrationBuilder.CreateIndex(
                name: "IX_PreloadedFiles_SysId",
                table: "PreloadedFiles",
                column: "SysId",
                unique: true)
                .Annotation("SqlServer:Clustered", true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PreloadedFiles");
        }
    }
}
