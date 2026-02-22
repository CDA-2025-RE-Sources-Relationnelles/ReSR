using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ReSR.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class _105 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Comments_Comments_AnsweredCommentId",
                table: "Comments");

            migrationBuilder.AddForeignKey(
                name: "FK_Comments_Comments_AnsweredCommentId",
                table: "Comments",
                column: "AnsweredCommentId",
                principalTable: "Comments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Comments_Comments_AnsweredCommentId",
                table: "Comments");

            migrationBuilder.AddForeignKey(
                name: "FK_Comments_Comments_AnsweredCommentId",
                table: "Comments",
                column: "AnsweredCommentId",
                principalTable: "Comments",
                principalColumn: "Id");
        }
    }
}
