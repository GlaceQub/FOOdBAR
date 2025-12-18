using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Restaurant.Migrations
{
    /// <inheritdoc />
    public partial class AddQuizEigenschappen : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Naam",
                table: "Categorie",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.CreateTable(
                name: "QuizEigenschappen",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProductId = table.Column<int>(type: "int", nullable: false),
                    IsZoet = table.Column<int>(type: "int", nullable: false),
                    IsZout = table.Column<int>(type: "int", nullable: false),
                    IsBitter = table.Column<int>(type: "int", nullable: false),
                    IsFris = table.Column<int>(type: "int", nullable: false),
                    IsPikant = table.Column<int>(type: "int", nullable: false),
                    IsAlcoholisch = table.Column<int>(type: "int", nullable: false),
                    IsWarm = table.Column<int>(type: "int", nullable: false),
                    IsKoud = table.Column<int>(type: "int", nullable: false),
                    IsLicht = table.Column<int>(type: "int", nullable: false),
                    IsZwaar = table.Column<int>(type: "int", nullable: false),
                    IsRomig = table.Column<int>(type: "int", nullable: false),
                    IsFruitig = table.Column<int>(type: "int", nullable: false),
                    IsKruidig = table.Column<int>(type: "int", nullable: false),
                    IsExotisch = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QuizEigenschappen", x => x.Id);
                    table.ForeignKey(
                        name: "FK_QuizEigenschappen_Product_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Product",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_QuizEigenschappen_ProductId",
                table: "QuizEigenschappen",
                column: "ProductId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "QuizEigenschappen");

            migrationBuilder.AlterColumn<string>(
                name: "Naam",
                table: "Categorie",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");
        }
    }
}
