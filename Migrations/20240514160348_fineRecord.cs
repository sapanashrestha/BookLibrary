using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BookLibrary.Migrations
{
    /// <inheritdoc />
    public partial class fineRecord : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FineAmount",
                table: "LibraryRecord");

            migrationBuilder.CreateTable(
                name: "FineRecords",
                columns: table => new
                {
                    FineId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FineAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    LibraryRecordId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FineRecords", x => x.FineId);
                    table.ForeignKey(
                        name: "FK_FineRecords_LibraryRecord_LibraryRecordId",
                        column: x => x.LibraryRecordId,
                        principalTable: "LibraryRecord",
                        principalColumn: "LibraryRecordId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_FineRecords_LibraryRecordId",
                table: "FineRecords",
                column: "LibraryRecordId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FineRecords");

            migrationBuilder.AddColumn<decimal>(
                name: "FineAmount",
                table: "LibraryRecord",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);
        }
    }
}
