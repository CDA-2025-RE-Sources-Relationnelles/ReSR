using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ReSR.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class _101 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Resource_Users_VerifyingUserId",
                table: "Resource");

            migrationBuilder.DropIndex(
                name: "IX_Resource_VerifyingUserId",
                table: "Resource");

            migrationBuilder.DropColumn(
                name: "VerifyingUserId",
                table: "Resource");

            migrationBuilder.AlterColumn<byte>(
                name: "Permissions",
                table: "Users",
                type: "smallint",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<byte>(
                name: "Permissions",
                table: "Managers",
                type: "smallint",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "Permissions",
                table: "Users",
                type: "integer",
                nullable: false,
                oldClrType: typeof(byte),
                oldType: "smallint");

            migrationBuilder.AddColumn<long>(
                name: "VerifyingUserId",
                table: "Resource",
                type: "bigint",
                nullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "Permissions",
                table: "Managers",
                type: "integer",
                nullable: false,
                oldClrType: typeof(byte),
                oldType: "smallint");

            migrationBuilder.CreateIndex(
                name: "IX_Resource_VerifyingUserId",
                table: "Resource",
                column: "VerifyingUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Resource_Users_VerifyingUserId",
                table: "Resource",
                column: "VerifyingUserId",
                principalTable: "Users",
                principalColumn: "Id");
        }
    }
}
