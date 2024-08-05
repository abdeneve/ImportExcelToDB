using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateSale : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Sale_Country_CountryId1",
                table: "Sale");

            migrationBuilder.DropForeignKey(
                name: "FK_Sale_Customer_CustomerId1",
                table: "Sale");

            migrationBuilder.DropIndex(
                name: "IX_Sale_CountryId1",
                table: "Sale");

            migrationBuilder.DropIndex(
                name: "IX_Sale_CustomerId1",
                table: "Sale");

            migrationBuilder.DropColumn(
                name: "CountryId1",
                table: "Sale");

            migrationBuilder.DropColumn(
                name: "CustomerId1",
                table: "Sale");

            migrationBuilder.AlterColumn<int>(
                name: "CustomerId",
                table: "Sale",
                type: "int",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AlterColumn<int>(
                name: "CountryId",
                table: "Sale",
                type: "int",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.CreateIndex(
                name: "IX_Sale_CountryId",
                table: "Sale",
                column: "CountryId");

            migrationBuilder.CreateIndex(
                name: "IX_Sale_CustomerId",
                table: "Sale",
                column: "CustomerId");

            migrationBuilder.AddForeignKey(
                name: "FK_Country_Sale",
                table: "Sale",
                column: "CountryId",
                principalTable: "Country",
                principalColumn: "CountryId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Customer_Sale",
                table: "Sale",
                column: "CustomerId",
                principalTable: "Customer",
                principalColumn: "CustomerId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Country_Sale",
                table: "Sale");

            migrationBuilder.DropForeignKey(
                name: "FK_Customer_Sale",
                table: "Sale");

            migrationBuilder.DropIndex(
                name: "IX_Sale_CountryId",
                table: "Sale");

            migrationBuilder.DropIndex(
                name: "IX_Sale_CustomerId",
                table: "Sale");

            migrationBuilder.AlterColumn<long>(
                name: "CustomerId",
                table: "Sale",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<long>(
                name: "CountryId",
                table: "Sale",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<int>(
                name: "CountryId1",
                table: "Sale",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "CustomerId1",
                table: "Sale",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Sale_CountryId1",
                table: "Sale",
                column: "CountryId1");

            migrationBuilder.CreateIndex(
                name: "IX_Sale_CustomerId1",
                table: "Sale",
                column: "CustomerId1");

            migrationBuilder.AddForeignKey(
                name: "FK_Sale_Country_CountryId1",
                table: "Sale",
                column: "CountryId1",
                principalTable: "Country",
                principalColumn: "CountryId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Sale_Customer_CustomerId1",
                table: "Sale",
                column: "CustomerId1",
                principalTable: "Customer",
                principalColumn: "CustomerId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
