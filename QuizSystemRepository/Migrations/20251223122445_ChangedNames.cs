using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QuizSystemRepository.Migrations
{
    /// <inheritdoc />
    public partial class ChangedNames : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AttemptedQuizAnswers");

            migrationBuilder.DropTable(
                name: "Students");

            migrationBuilder.DropTable(
                name: "Subjects");

            migrationBuilder.DropTable(
                name: "QuestionBanks");

            migrationBuilder.DropTable(
                name: "QuizAttempts");

            migrationBuilder.DropTable(
                name: "Classes");

            migrationBuilder.DropTable(
                name: "EducationMediums");

            migrationBuilder.CreateTable(
                name: "EducationMedium",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedById = table.Column<long>(type: "bigint", nullable: true),
                    ModifiedById = table.Column<long>(type: "bigint", nullable: true),
                    IsApproved = table.Column<bool>(type: "bit", nullable: false),
                    ApprovedById = table.Column<long>(type: "bigint", nullable: true),
                    RejectedById = table.Column<long>(type: "bigint", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ApprovedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    RejectedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EducationMedium", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EducationMedium_AspNetUsers_ApprovedById",
                        column: x => x.ApprovedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_EducationMedium_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_EducationMedium_AspNetUsers_ModifiedById",
                        column: x => x.ModifiedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_EducationMedium_AspNetUsers_RejectedById",
                        column: x => x.RejectedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "QuestionBank",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedById = table.Column<long>(type: "bigint", nullable: true),
                    ModifiedById = table.Column<long>(type: "bigint", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false),
                    QuizId = table.Column<long>(type: "bigint", nullable: false),
                    QuestionText = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    OptionA = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    OptionB = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    OptionC = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    OptionD = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RightOption = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Markes = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QuestionBank", x => x.Id);
                    table.ForeignKey(
                        name: "FK_QuestionBank_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_QuestionBank_AspNetUsers_ModifiedById",
                        column: x => x.ModifiedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_QuestionBank_Quiz_QuizId",
                        column: x => x.QuizId,
                        principalTable: "Quiz",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "QuizAttempt",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SubmittedById = table.Column<long>(type: "bigint", nullable: true),
                    SubmittedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    QuizId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QuizAttempt", x => x.Id);
                    table.ForeignKey(
                        name: "FK_QuizAttempt_AspNetUsers_SubmittedById",
                        column: x => x.SubmittedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_QuizAttempt_Quiz_QuizId",
                        column: x => x.QuizId,
                        principalTable: "Quiz",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Classe",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedById = table.Column<long>(type: "bigint", nullable: true),
                    ModifiedById = table.Column<long>(type: "bigint", nullable: true),
                    IsApproved = table.Column<bool>(type: "bit", nullable: false),
                    ApprovedById = table.Column<long>(type: "bigint", nullable: true),
                    RejectedById = table.Column<long>(type: "bigint", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ApprovedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    RejectedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false),
                    EducationMediumId = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Classe", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Classe_AspNetUsers_ApprovedById",
                        column: x => x.ApprovedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Classe_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Classe_AspNetUsers_ModifiedById",
                        column: x => x.ModifiedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Classe_AspNetUsers_RejectedById",
                        column: x => x.RejectedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Classe_EducationMedium_EducationMediumId",
                        column: x => x.EducationMediumId,
                        principalTable: "EducationMedium",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Instructor",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedById = table.Column<long>(type: "bigint", nullable: true),
                    ModifiedById = table.Column<long>(type: "bigint", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    HscPassingInstrutute = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    HscPassingYear = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    HscGrade = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserId = table.Column<long>(type: "bigint", nullable: true),
                    EducationMediumId = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Instructor", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Instructor_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Instructor_AspNetUsers_ModifiedById",
                        column: x => x.ModifiedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Instructor_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Instructor_EducationMedium_EducationMediumId",
                        column: x => x.EducationMediumId,
                        principalTable: "EducationMedium",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "AttemptedQuizAnswer",
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
                    table.PrimaryKey("PK_AttemptedQuizAnswer", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AttemptedQuizAnswer_QuestionBank_QuestionBankId",
                        column: x => x.QuestionBankId,
                        principalTable: "QuestionBank",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_AttemptedQuizAnswer_QuizAttempt_QuizAttemptId",
                        column: x => x.QuizAttemptId,
                        principalTable: "QuizAttempt",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Student",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedById = table.Column<long>(type: "bigint", nullable: true),
                    ModifiedById = table.Column<long>(type: "bigint", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserId = table.Column<long>(type: "bigint", nullable: true),
                    EducationMediumId = table.Column<long>(type: "bigint", nullable: true),
                    ClassId = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Student", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Student_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Student_AspNetUsers_ModifiedById",
                        column: x => x.ModifiedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Student_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Student_Classe_ClassId",
                        column: x => x.ClassId,
                        principalTable: "Classe",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Student_EducationMedium_EducationMediumId",
                        column: x => x.EducationMediumId,
                        principalTable: "EducationMedium",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Subject",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedById = table.Column<long>(type: "bigint", nullable: true),
                    ModifiedById = table.Column<long>(type: "bigint", nullable: true),
                    IsApproved = table.Column<bool>(type: "bit", nullable: false),
                    ApprovedById = table.Column<long>(type: "bigint", nullable: true),
                    RejectedById = table.Column<long>(type: "bigint", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ApprovedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    RejectedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false),
                    ClassId = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Subject", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Subject_AspNetUsers_ApprovedById",
                        column: x => x.ApprovedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Subject_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Subject_AspNetUsers_ModifiedById",
                        column: x => x.ModifiedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Subject_AspNetUsers_RejectedById",
                        column: x => x.RejectedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Subject_Classe_ClassId",
                        column: x => x.ClassId,
                        principalTable: "Classe",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_AttemptedQuizAnswer_QuestionBankId",
                table: "AttemptedQuizAnswer",
                column: "QuestionBankId");

            migrationBuilder.CreateIndex(
                name: "IX_AttemptedQuizAnswer_QuizAttemptId",
                table: "AttemptedQuizAnswer",
                column: "QuizAttemptId");

            migrationBuilder.CreateIndex(
                name: "IX_Classe_ApprovedById",
                table: "Classe",
                column: "ApprovedById");

            migrationBuilder.CreateIndex(
                name: "IX_Classe_CreatedById",
                table: "Classe",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_Classe_EducationMediumId",
                table: "Classe",
                column: "EducationMediumId");

            migrationBuilder.CreateIndex(
                name: "IX_Classe_ModifiedById",
                table: "Classe",
                column: "ModifiedById");

            migrationBuilder.CreateIndex(
                name: "IX_Classe_RejectedById",
                table: "Classe",
                column: "RejectedById");

            migrationBuilder.CreateIndex(
                name: "IX_EducationMedium_ApprovedById",
                table: "EducationMedium",
                column: "ApprovedById");

            migrationBuilder.CreateIndex(
                name: "IX_EducationMedium_CreatedById",
                table: "EducationMedium",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_EducationMedium_ModifiedById",
                table: "EducationMedium",
                column: "ModifiedById");

            migrationBuilder.CreateIndex(
                name: "IX_EducationMedium_RejectedById",
                table: "EducationMedium",
                column: "RejectedById");

            migrationBuilder.CreateIndex(
                name: "IX_Instructor_CreatedById",
                table: "Instructor",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_Instructor_EducationMediumId",
                table: "Instructor",
                column: "EducationMediumId");

            migrationBuilder.CreateIndex(
                name: "IX_Instructor_ModifiedById",
                table: "Instructor",
                column: "ModifiedById");

            migrationBuilder.CreateIndex(
                name: "IX_Instructor_UserId",
                table: "Instructor",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_QuestionBank_CreatedById",
                table: "QuestionBank",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_QuestionBank_ModifiedById",
                table: "QuestionBank",
                column: "ModifiedById");

            migrationBuilder.CreateIndex(
                name: "IX_QuestionBank_QuizId",
                table: "QuestionBank",
                column: "QuizId");

            migrationBuilder.CreateIndex(
                name: "IX_QuizAttempt_QuizId",
                table: "QuizAttempt",
                column: "QuizId");

            migrationBuilder.CreateIndex(
                name: "IX_QuizAttempt_SubmittedById",
                table: "QuizAttempt",
                column: "SubmittedById");

            migrationBuilder.CreateIndex(
                name: "IX_Student_ClassId",
                table: "Student",
                column: "ClassId");

            migrationBuilder.CreateIndex(
                name: "IX_Student_CreatedById",
                table: "Student",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_Student_EducationMediumId",
                table: "Student",
                column: "EducationMediumId");

            migrationBuilder.CreateIndex(
                name: "IX_Student_ModifiedById",
                table: "Student",
                column: "ModifiedById");

            migrationBuilder.CreateIndex(
                name: "IX_Student_UserId",
                table: "Student",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Subject_ApprovedById",
                table: "Subject",
                column: "ApprovedById");

            migrationBuilder.CreateIndex(
                name: "IX_Subject_ClassId",
                table: "Subject",
                column: "ClassId");

            migrationBuilder.CreateIndex(
                name: "IX_Subject_CreatedById",
                table: "Subject",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_Subject_ModifiedById",
                table: "Subject",
                column: "ModifiedById");

            migrationBuilder.CreateIndex(
                name: "IX_Subject_RejectedById",
                table: "Subject",
                column: "RejectedById");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AttemptedQuizAnswer");

            migrationBuilder.DropTable(
                name: "Instructor");

            migrationBuilder.DropTable(
                name: "Student");

            migrationBuilder.DropTable(
                name: "Subject");

            migrationBuilder.DropTable(
                name: "QuestionBank");

            migrationBuilder.DropTable(
                name: "QuizAttempt");

            migrationBuilder.DropTable(
                name: "Classe");

            migrationBuilder.DropTable(
                name: "EducationMedium");

            migrationBuilder.CreateTable(
                name: "EducationMediums",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ApprovedById = table.Column<long>(type: "bigint", nullable: true),
                    CreatedById = table.Column<long>(type: "bigint", nullable: true),
                    ModifiedById = table.Column<long>(type: "bigint", nullable: true),
                    RejectedById = table.Column<long>(type: "bigint", nullable: true),
                    ApprovedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsApproved = table.Column<bool>(type: "bit", nullable: false),
                    ModifiedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RejectedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EducationMediums", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EducationMediums_AspNetUsers_ApprovedById",
                        column: x => x.ApprovedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_EducationMediums_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_EducationMediums_AspNetUsers_ModifiedById",
                        column: x => x.ModifiedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_EducationMediums_AspNetUsers_RejectedById",
                        column: x => x.RejectedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "QuestionBanks",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreatedById = table.Column<long>(type: "bigint", nullable: true),
                    ModifiedById = table.Column<long>(type: "bigint", nullable: true),
                    QuizId = table.Column<long>(type: "bigint", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Markes = table.Column<int>(type: "int", nullable: false),
                    ModifiedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    OptionA = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    OptionB = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    OptionC = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    OptionD = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    QuestionText = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RightOption = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QuestionBanks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_QuestionBanks_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_QuestionBanks_AspNetUsers_ModifiedById",
                        column: x => x.ModifiedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_QuestionBanks_Quiz_QuizId",
                        column: x => x.QuizId,
                        principalTable: "Quiz",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "QuizAttempts",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    QuizId = table.Column<long>(type: "bigint", nullable: false),
                    SubmittedById = table.Column<long>(type: "bigint", nullable: true),
                    SubmittedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QuizAttempts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_QuizAttempts_AspNetUsers_SubmittedById",
                        column: x => x.SubmittedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_QuizAttempts_Quiz_QuizId",
                        column: x => x.QuizId,
                        principalTable: "Quiz",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Classes",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ApprovedById = table.Column<long>(type: "bigint", nullable: true),
                    CreatedById = table.Column<long>(type: "bigint", nullable: true),
                    EducationMediumId = table.Column<long>(type: "bigint", nullable: true),
                    ModifiedById = table.Column<long>(type: "bigint", nullable: true),
                    RejectedById = table.Column<long>(type: "bigint", nullable: true),
                    ApprovedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsApproved = table.Column<bool>(type: "bit", nullable: false),
                    ModifiedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RejectedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Classes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Classes_AspNetUsers_ApprovedById",
                        column: x => x.ApprovedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Classes_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Classes_AspNetUsers_ModifiedById",
                        column: x => x.ModifiedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Classes_AspNetUsers_RejectedById",
                        column: x => x.RejectedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Classes_EducationMediums_EducationMediumId",
                        column: x => x.EducationMediumId,
                        principalTable: "EducationMediums",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "AttemptedQuizAnswers",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    QuestionBankId = table.Column<long>(type: "bigint", nullable: true),
                    QuizAttemptId = table.Column<long>(type: "bigint", nullable: true),
                    Score = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    SelectedOption = table.Column<string>(type: "nvarchar(max)", nullable: true)
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

            migrationBuilder.CreateTable(
                name: "Students",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ClassId = table.Column<long>(type: "bigint", nullable: true),
                    CreatedById = table.Column<long>(type: "bigint", nullable: true),
                    EducationMediumId = table.Column<long>(type: "bigint", nullable: true),
                    ModifiedById = table.Column<long>(type: "bigint", nullable: true),
                    UserId = table.Column<long>(type: "bigint", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ModifiedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Students", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Students_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Students_AspNetUsers_ModifiedById",
                        column: x => x.ModifiedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Students_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Students_Classes_ClassId",
                        column: x => x.ClassId,
                        principalTable: "Classes",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Students_EducationMediums_EducationMediumId",
                        column: x => x.EducationMediumId,
                        principalTable: "EducationMediums",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Subjects",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ApprovedById = table.Column<long>(type: "bigint", nullable: true),
                    ClassId = table.Column<long>(type: "bigint", nullable: true),
                    CreatedById = table.Column<long>(type: "bigint", nullable: true),
                    ModifiedById = table.Column<long>(type: "bigint", nullable: true),
                    RejectedById = table.Column<long>(type: "bigint", nullable: true),
                    ApprovedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsApproved = table.Column<bool>(type: "bit", nullable: false),
                    ModifiedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RejectedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Subjects", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Subjects_AspNetUsers_ApprovedById",
                        column: x => x.ApprovedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Subjects_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Subjects_AspNetUsers_ModifiedById",
                        column: x => x.ModifiedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Subjects_AspNetUsers_RejectedById",
                        column: x => x.RejectedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Subjects_Classes_ClassId",
                        column: x => x.ClassId,
                        principalTable: "Classes",
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

            migrationBuilder.CreateIndex(
                name: "IX_Classes_ApprovedById",
                table: "Classes",
                column: "ApprovedById");

            migrationBuilder.CreateIndex(
                name: "IX_Classes_CreatedById",
                table: "Classes",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_Classes_EducationMediumId",
                table: "Classes",
                column: "EducationMediumId");

            migrationBuilder.CreateIndex(
                name: "IX_Classes_ModifiedById",
                table: "Classes",
                column: "ModifiedById");

            migrationBuilder.CreateIndex(
                name: "IX_Classes_RejectedById",
                table: "Classes",
                column: "RejectedById");

            migrationBuilder.CreateIndex(
                name: "IX_EducationMediums_ApprovedById",
                table: "EducationMediums",
                column: "ApprovedById");

            migrationBuilder.CreateIndex(
                name: "IX_EducationMediums_CreatedById",
                table: "EducationMediums",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_EducationMediums_ModifiedById",
                table: "EducationMediums",
                column: "ModifiedById");

            migrationBuilder.CreateIndex(
                name: "IX_EducationMediums_RejectedById",
                table: "EducationMediums",
                column: "RejectedById");

            migrationBuilder.CreateIndex(
                name: "IX_QuestionBanks_CreatedById",
                table: "QuestionBanks",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_QuestionBanks_ModifiedById",
                table: "QuestionBanks",
                column: "ModifiedById");

            migrationBuilder.CreateIndex(
                name: "IX_QuestionBanks_QuizId",
                table: "QuestionBanks",
                column: "QuizId");

            migrationBuilder.CreateIndex(
                name: "IX_QuizAttempts_QuizId",
                table: "QuizAttempts",
                column: "QuizId");

            migrationBuilder.CreateIndex(
                name: "IX_QuizAttempts_SubmittedById",
                table: "QuizAttempts",
                column: "SubmittedById");

            migrationBuilder.CreateIndex(
                name: "IX_Students_ClassId",
                table: "Students",
                column: "ClassId");

            migrationBuilder.CreateIndex(
                name: "IX_Students_CreatedById",
                table: "Students",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_Students_EducationMediumId",
                table: "Students",
                column: "EducationMediumId");

            migrationBuilder.CreateIndex(
                name: "IX_Students_ModifiedById",
                table: "Students",
                column: "ModifiedById");

            migrationBuilder.CreateIndex(
                name: "IX_Students_UserId",
                table: "Students",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Subjects_ApprovedById",
                table: "Subjects",
                column: "ApprovedById");

            migrationBuilder.CreateIndex(
                name: "IX_Subjects_ClassId",
                table: "Subjects",
                column: "ClassId");

            migrationBuilder.CreateIndex(
                name: "IX_Subjects_CreatedById",
                table: "Subjects",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_Subjects_ModifiedById",
                table: "Subjects",
                column: "ModifiedById");

            migrationBuilder.CreateIndex(
                name: "IX_Subjects_RejectedById",
                table: "Subjects",
                column: "RejectedById");
        }
    }
}
