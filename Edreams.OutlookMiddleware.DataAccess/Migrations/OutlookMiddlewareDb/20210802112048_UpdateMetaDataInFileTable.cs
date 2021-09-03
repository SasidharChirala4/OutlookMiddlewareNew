using Microsoft.EntityFrameworkCore.Migrations;

namespace Edreams.OutlookMiddleware.DataAccess.Migrations.OutlookMiddlewareDb
{
    public partial class UpdateMetaDataInFileTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Metadata_FileId",
                table: "Metadata");

            migrationBuilder.CreateIndex(
                name: "IX_Metadata_FileId",
                table: "Metadata",
                column: "FileId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Metadata_FileId",
                table: "Metadata");

            migrationBuilder.CreateIndex(
                name: "IX_Metadata_FileId",
                table: "Metadata",
                column: "FileId",
                unique: true);
        }
    }
}
