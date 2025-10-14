using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccessLayer.Migrations
{
    /// <inheritdoc />
    public partial class AddDealerIdToUsers : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "DealerId",
                table: "Users",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_DealerId",
                table: "Users",
                column: "DealerId");

            migrationBuilder.AddForeignKey(
                name: "FK_Users_Dealer_DealerId",
                table: "Users",
                column: "DealerId",
                principalTable: "Dealer",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Users_Dealer_DealerId",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_Users_DealerId",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "DealerId",
                table: "Users");
        }
    }
}
