using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QuizSystemRepository.Migrations
{
    /// <inheritdoc />
    public partial class AttemptedQuizAnswer : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AttemptedQuizAnswers",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    QuizAttemptId = table.Column<long>(type: "bigint", nullable: true),
                    QuestionBankId = table.Column<long>(type: "bigint", nullable: true),
                    SelectedOption = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Score = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AttemptedQuizAnswers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AttemptedQuizAnswers_QuestionBanks_QuestionBankId",
                        column: x => x.QuestionBankId,
                        principalTable: "QuestionBanks",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_AttemptedQuizAnswers_QuizAttempts_QuizAttemptId",
                        column: x => x.QuizAttemptId,
                        principalTable: "QuizAttempts",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_AttemptedQuizAnswers_QuestionBankId",
                table: "AttemptedQuizAnswers",
                column: "QuestionBankId");

            migrationBuilder.CreateIndex(
                name: "IX_AttemptedQuizAnswers_QuizAttemptId",
                table: "AttemptedQuizAnswers",
                column: "QuizAttemptId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AttemptedQuizAnswers");
        }
    }
}
