using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TextileCRM.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddFileAttachmentUserRelation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FileAttachments_Users_UploadedByUserId",
                table: "FileAttachments");

            migrationBuilder.DropIndex(
                name: "IX_FileAttachments_UploadedByUserId",
                table: "FileAttachments");

            migrationBuilder.DropColumn(
                name: "UploadedByUserId",
                table: "FileAttachments");

            migrationBuilder.CreateIndex(
                name: "IX_FileAttachments_UploadedBy",
                table: "FileAttachments",
                column: "UploadedBy");

            migrationBuilder.AddForeignKey(
                name: "FK_FileAttachments_Users_UploadedBy",
                table: "FileAttachments",
                column: "UploadedBy",
                principalTable: "Users",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FileAttachments_Users_UploadedBy",
                table: "FileAttachments");

            migrationBuilder.DropIndex(
                name: "IX_FileAttachments_UploadedBy",
                table: "FileAttachments");

            migrationBuilder.AddColumn<int>(
                name: "UploadedByUserId",
                table: "FileAttachments",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_FileAttachments_UploadedByUserId",
                table: "FileAttachments",
                column: "UploadedByUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_FileAttachments_Users_UploadedByUserId",
                table: "FileAttachments",
                column: "UploadedByUserId",
                principalTable: "Users",
                principalColumn: "Id");
        }
    }
}
