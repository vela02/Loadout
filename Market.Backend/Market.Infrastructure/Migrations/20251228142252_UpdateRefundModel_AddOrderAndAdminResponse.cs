using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Market.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateRefundModel_AddOrderAndAdminResponse : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AdminResponse",
                table: "Refund",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "OrderId",
                table: "Refund",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "UserId",
                table: "Refund",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Refund_OrderId",
                table: "Refund",
                column: "OrderId");

            migrationBuilder.AddForeignKey(
                name: "FK_Refund_Order_OrderId",
                table: "Refund",
                column: "OrderId",
                principalTable: "Order",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Refund_Order_OrderId",
                table: "Refund");

            migrationBuilder.DropIndex(
                name: "IX_Refund_OrderId",
                table: "Refund");

            migrationBuilder.DropColumn(
                name: "AdminResponse",
                table: "Refund");

            migrationBuilder.DropColumn(
                name: "OrderId",
                table: "Refund");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Refund");
        }
    }
}
