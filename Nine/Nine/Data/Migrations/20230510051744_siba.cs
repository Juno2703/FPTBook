using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Nine.Data.Migrations
{
    /// <inheritdoc />
    public partial class siba : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "OwnerAddId",
                table: "Books",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Books_OwnerAddId",
                table: "Books",
                column: "OwnerAddId");

            migrationBuilder.AddForeignKey(
                name: "FK_Books_AspNetUsers_OwnerAddId",
                table: "Books",
                column: "OwnerAddId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Books_AspNetUsers_OwnerAddId",
                table: "Books");

            migrationBuilder.DropIndex(
                name: "IX_Books_OwnerAddId",
                table: "Books");

            migrationBuilder.DropColumn(
                name: "OwnerAddId",
                table: "Books");
        }
    }
}
