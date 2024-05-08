using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BookLibrary.Migrations
{
    /// <inheritdoc />
    public partial class libraryRefactored : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LibraryRecord_BooksList_BooksId",
                table: "LibraryRecord");

            migrationBuilder.DropIndex(
                name: "IX_LibraryRecord_BooksId",
                table: "LibraryRecord");

            migrationBuilder.DropColumn(
                name: "BooksId",
                table: "LibraryRecord");

            migrationBuilder.CreateIndex(
                name: "IX_LibraryRecord_BookId",
                table: "LibraryRecord",
                column: "BookId");

            migrationBuilder.AddForeignKey(
                name: "FK_LibraryRecord_BooksList_BookId",
                table: "LibraryRecord",
                column: "BookId",
                principalTable: "BooksList",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LibraryRecord_BooksList_BookId",
                table: "LibraryRecord");

            migrationBuilder.DropIndex(
                name: "IX_LibraryRecord_BookId",
                table: "LibraryRecord");

            migrationBuilder.AddColumn<int>(
                name: "BooksId",
                table: "LibraryRecord",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_LibraryRecord_BooksId",
                table: "LibraryRecord",
                column: "BooksId");

            migrationBuilder.AddForeignKey(
                name: "FK_LibraryRecord_BooksList_BooksId",
                table: "LibraryRecord",
                column: "BooksId",
                principalTable: "BooksList",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
