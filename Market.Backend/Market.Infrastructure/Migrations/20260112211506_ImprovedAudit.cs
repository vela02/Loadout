using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Market.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ImprovedAudit : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Action",
                table: "LogActions",
                newName: "Message");

            migrationBuilder.AddColumn<string>(
                name: "ActionType",
                table: "LogActions",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "EntityId",
                table: "LogActions",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "EntityName",
                table: "LogActions",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "IpAddress",
                table: "LogActions",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ActionType",
                table: "LogActions");

            migrationBuilder.DropColumn(
                name: "EntityId",
                table: "LogActions");

            migrationBuilder.DropColumn(
                name: "EntityName",
                table: "LogActions");

            migrationBuilder.DropColumn(
                name: "IpAddress",
                table: "LogActions");

            migrationBuilder.RenameColumn(
                name: "Message",
                table: "LogActions",
                newName: "Action");
        }
    }
}
