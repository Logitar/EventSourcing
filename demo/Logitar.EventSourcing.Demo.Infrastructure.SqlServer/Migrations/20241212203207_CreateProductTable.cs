using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Logitar.EventSourcing.Demo.Infrastructure.SqlServer.Migrations
{
    /// <inheritdoc />
    public partial class CreateProductTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Products",
                columns: table => new
                {
                    ProductId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Sku = table.Column<string>(type: "nvarchar(32)", maxLength: 32, nullable: false),
                    SkuNormalized = table.Column<string>(type: "nvarchar(32)", maxLength: 32, nullable: false),
                    DisplayName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", maxLength: 65535, nullable: true),
                    Price = table.Column<decimal>(type: "money", nullable: true),
                    PictureUrl = table.Column<string>(type: "nvarchar(2048)", maxLength: 2048, nullable: true),
                    AggregateId = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Version = table.Column<long>(type: "bigint", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    UpdatedOn = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Products", x => x.ProductId);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Products_AggregateId",
                table: "Products",
                column: "AggregateId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Products_CreatedBy",
                table: "Products",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_Products_CreatedOn",
                table: "Products",
                column: "CreatedOn");

            migrationBuilder.CreateIndex(
                name: "IX_Products_DisplayName",
                table: "Products",
                column: "DisplayName");

            migrationBuilder.CreateIndex(
                name: "IX_Products_Id",
                table: "Products",
                column: "Id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Products_Price",
                table: "Products",
                column: "Price");

            migrationBuilder.CreateIndex(
                name: "IX_Products_Sku",
                table: "Products",
                column: "Sku");

            migrationBuilder.CreateIndex(
                name: "IX_Products_SkuNormalized",
                table: "Products",
                column: "SkuNormalized",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Products_UpdatedBy",
                table: "Products",
                column: "UpdatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_Products_UpdatedOn",
                table: "Products",
                column: "UpdatedOn");

            migrationBuilder.CreateIndex(
                name: "IX_Products_Version",
                table: "Products",
                column: "Version");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Products");
        }
    }
}
