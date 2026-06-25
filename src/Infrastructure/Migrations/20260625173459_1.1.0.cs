using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ReSR.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class _110 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Resource_Users_OwnerId",
                table: "Resource");

            migrationBuilder.AddColumn<int>(
                name: "AuthFailedAttemptCount",
                table: "Users",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddForeignKey(
                name: "FK_Resource_Users_OwnerId",
                table: "Resource",
                column: "OwnerId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Resource_Users_OwnerId",
                table: "Resource");

            migrationBuilder.DropColumn(
                name: "AuthFailedAttemptCount",
                table: "Users");

            migrationBuilder.AddForeignKey(
                name: "FK_Resource_Users_OwnerId",
                table: "Resource",
                column: "OwnerId",
                principalTable: "Users",
                principalColumn: "Id");
        }
    }
}
