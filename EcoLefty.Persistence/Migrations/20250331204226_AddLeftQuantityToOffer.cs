using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EcoLefty.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddLeftQuantityToOffer : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "QuantityAvailable",
                schema: "ecolefty",
                table: "Offers",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "QuantityAvailable",
                schema: "ecolefty",
                table: "Offers");
        }
    }
}
