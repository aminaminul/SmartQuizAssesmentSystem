using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QuizSystemRepository.Migrations
{
    /// <inheritdoc />
    public partial class QuestionUpdate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "SubjectId",
                table: "Instructor",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Instructor_SubjectId",
                table: "Instructor",
                column: "SubjectId");

            migrationBuilder.AddForeignKey(
                name: "FK_Instructor_Subject_SubjectId",
                table: "Instructor",
                column: "SubjectId",
                principalTable: "Subject",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Instructor_Subject_SubjectId",
                table: "Instructor");

            migrationBuilder.DropIndex(
                name: "IX_Instructor_SubjectId",
                table: "Instructor");

            migrationBuilder.DropColumn(
                name: "SubjectId",
                table: "Instructor");
        }
    }
}
