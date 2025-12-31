using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QuizSystemRepository.Migrations
{
    /// <inheritdoc />
    public partial class ModelUpdate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Student_AspNetUsers_UserId",
                table: "Student");

            migrationBuilder.RenameColumn(
                name: "ÈndAt",
                table: "Quiz",
                newName: "EndAt");

            migrationBuilder.RenameColumn(
                name: "Markes",
                table: "QuestionBank",
                newName: "Marks");

            migrationBuilder.AlterColumn<long>(
                name: "UserId",
                table: "Student",
                type: "bigint",
                nullable: true,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AddColumn<string>(
                name: "Subject",
                table: "Quiz",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Subject",
                table: "QuestionBank",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddForeignKey(
                name: "FK_Student_AspNetUsers_UserId",
                table: "Student",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Student_AspNetUsers_UserId",
                table: "Student");

            migrationBuilder.DropColumn(
                name: "Subject",
                table: "Quiz");

            migrationBuilder.DropColumn(
                name: "Subject",
                table: "QuestionBank");

            migrationBuilder.RenameColumn(
                name: "EndAt",
                table: "Quiz",
                newName: "ÈndAt");

            migrationBuilder.RenameColumn(
                name: "Marks",
                table: "QuestionBank",
                newName: "Markes");

            migrationBuilder.AlterColumn<long>(
                name: "UserId",
                table: "Student",
                type: "bigint",
                nullable: false,
                defaultValue: 0L,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Student_AspNetUsers_UserId",
                table: "Student",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
