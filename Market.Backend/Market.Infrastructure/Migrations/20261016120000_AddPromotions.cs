using System;
using Market.Application;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Market.Infrastructure.Migrations
{
    /// <inheritdoc />
    [DbContext(typeof(LoadoutDbContext))]
    [Migration("20261016120000_AddPromotions")]
    public partial class AddPromotions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Promotion",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    DiscountPercentage = table.Column<decimal>(type: "decimal(5,2)", nullable: false),
                    StartDate = table.Column<DateTime>(type: "datetime", nullable: false),
                    EndDate = table.Column<DateTime>(type: "datetime", nullable: false),
                    TargetType = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    TargetValue = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Promotion", x => x.Id);
                });

            migrationBuilder.AddColumn<int>(
                name: "PromotionId",
                table: "Discount",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Discount_PromotionId",
                table: "Discount",
                column: "PromotionId");

            migrationBuilder.AddForeignKey(
                name: "FK_Discount_Promotion_PromotionId",
                table: "Discount",
                column: "PromotionId",
                principalTable: "Promotion",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Discount_Promotion_PromotionId",
                table: "Discount");

            migrationBuilder.DropIndex(
                name: "IX_Discount_PromotionId",
                table: "Discount");

            migrationBuilder.DropColumn(
                name: "PromotionId",
                table: "Discount");

            migrationBuilder.DropTable(
                name: "Promotion");
        }
    }
}
