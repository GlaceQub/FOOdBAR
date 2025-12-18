using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Restaurant.Migrations
{
    /// <inheritdoc />
    public partial class Change_Colum_Names : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "IsZwaar",
                table: "QuizEigenschappen",
                newName: "Zwaar");

            migrationBuilder.RenameColumn(
                name: "IsZout",
                table: "QuizEigenschappen",
                newName: "Zout");

            migrationBuilder.RenameColumn(
                name: "IsZoet",
                table: "QuizEigenschappen",
                newName: "Zoet");

            migrationBuilder.RenameColumn(
                name: "IsWarm",
                table: "QuizEigenschappen",
                newName: "Warm");

            migrationBuilder.RenameColumn(
                name: "IsRomig",
                table: "QuizEigenschappen",
                newName: "Romig");

            migrationBuilder.RenameColumn(
                name: "IsPikant",
                table: "QuizEigenschappen",
                newName: "Pikant");

            migrationBuilder.RenameColumn(
                name: "IsLicht",
                table: "QuizEigenschappen",
                newName: "Licht");

            migrationBuilder.RenameColumn(
                name: "IsKruidig",
                table: "QuizEigenschappen",
                newName: "Kruidig");

            migrationBuilder.RenameColumn(
                name: "IsKoud",
                table: "QuizEigenschappen",
                newName: "Koud");

            migrationBuilder.RenameColumn(
                name: "IsFruitig",
                table: "QuizEigenschappen",
                newName: "Fruitig");

            migrationBuilder.RenameColumn(
                name: "IsFris",
                table: "QuizEigenschappen",
                newName: "Fris");

            migrationBuilder.RenameColumn(
                name: "IsExotisch",
                table: "QuizEigenschappen",
                newName: "Exotisch");

            migrationBuilder.RenameColumn(
                name: "IsBitter",
                table: "QuizEigenschappen",
                newName: "Bitter");

            migrationBuilder.RenameColumn(
                name: "IsAlcoholisch",
                table: "QuizEigenschappen",
                newName: "Alcoholisch");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Zwaar",
                table: "QuizEigenschappen",
                newName: "IsZwaar");

            migrationBuilder.RenameColumn(
                name: "Zout",
                table: "QuizEigenschappen",
                newName: "IsZout");

            migrationBuilder.RenameColumn(
                name: "Zoet",
                table: "QuizEigenschappen",
                newName: "IsZoet");

            migrationBuilder.RenameColumn(
                name: "Warm",
                table: "QuizEigenschappen",
                newName: "IsWarm");

            migrationBuilder.RenameColumn(
                name: "Romig",
                table: "QuizEigenschappen",
                newName: "IsRomig");

            migrationBuilder.RenameColumn(
                name: "Pikant",
                table: "QuizEigenschappen",
                newName: "IsPikant");

            migrationBuilder.RenameColumn(
                name: "Licht",
                table: "QuizEigenschappen",
                newName: "IsLicht");

            migrationBuilder.RenameColumn(
                name: "Kruidig",
                table: "QuizEigenschappen",
                newName: "IsKruidig");

            migrationBuilder.RenameColumn(
                name: "Koud",
                table: "QuizEigenschappen",
                newName: "IsKoud");

            migrationBuilder.RenameColumn(
                name: "Fruitig",
                table: "QuizEigenschappen",
                newName: "IsFruitig");

            migrationBuilder.RenameColumn(
                name: "Fris",
                table: "QuizEigenschappen",
                newName: "IsFris");

            migrationBuilder.RenameColumn(
                name: "Exotisch",
                table: "QuizEigenschappen",
                newName: "IsExotisch");

            migrationBuilder.RenameColumn(
                name: "Bitter",
                table: "QuizEigenschappen",
                newName: "IsBitter");

            migrationBuilder.RenameColumn(
                name: "Alcoholisch",
                table: "QuizEigenschappen",
                newName: "IsAlcoholisch");
        }
    }
}
