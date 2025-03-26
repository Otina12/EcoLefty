using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EcoLefty.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class addPurchaseJointTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Purchases",
                schema: "ecolefty",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    TotalPrice = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    PurchaseDateUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    OfferId = table.Column<int>(type: "int", nullable: false),
                    CustomerId = table.Column<int>(type: "int", nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Purchases", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Purchases_Offers_OfferId",
                        column: x => x.OfferId,
                        principalSchema: "ecolefty",
                        principalTable: "Offers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Purchases_Users_CustomerId",
                        column: x => x.CustomerId,
                        principalSchema: "ecolefty",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Purchases_CustomerId",
                schema: "ecolefty",
                table: "Purchases",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_Purchases_OfferId",
                schema: "ecolefty",
                table: "Purchases",
                column: "OfferId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Purchases",
                schema: "ecolefty");
        }
    }
}
