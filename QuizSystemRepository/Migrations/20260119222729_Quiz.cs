using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QuizSystemRepository.Migrations
{
    
    public partial class Quiz : Migration
    {
        
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Subject",
                table: "Quiz");

            migrationBuilder.AddColumn<long>(
                name: "ClassId",
                table: "Quiz",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "EducationMediumId",
                table: "Quiz",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "SubjectId",
                table: "Quiz",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Quiz_ClassId",
                table: "Quiz",
                column: "ClassId");

            migrationBuilder.CreateIndex(
                name: "IX_Quiz_EducationMediumId",
                table: "Quiz",
                column: "EducationMediumId");

            migrationBuilder.CreateIndex(
                name: "IX_Quiz_SubjectId",
                table: "Quiz",
                column: "SubjectId");

            migrationBuilder.AddForeignKey(
                name: "FK_Quiz_Class_ClassId",
                table: "Quiz",
                column: "ClassId",
                principalTable: "Class",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Quiz_EducationMedium_EducationMediumId",
                table: "Quiz",
                column: "EducationMediumId",
                principalTable: "EducationMedium",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Quiz_Subject_SubjectId",
                table: "Quiz",
                column: "SubjectId",
                principalTable: "Subject",
                principalColumn: "Id");
        }

        
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Quiz_Class_ClassId",
                table: "Quiz");

            migrationBuilder.DropForeignKey(
                name: "FK_Quiz_EducationMedium_EducationMediumId",
                table: "Quiz");

            migrationBuilder.DropForeignKey(
                name: "FK_Quiz_Subject_SubjectId",
                table: "Quiz");

            migrationBuilder.DropIndex(
                name: "IX_Quiz_ClassId",
                table: "Quiz");

            migrationBuilder.DropIndex(
                name: "IX_Quiz_EducationMediumId",
                table: "Quiz");

            migrationBuilder.DropIndex(
                name: "IX_Quiz_SubjectId",
                table: "Quiz");

            migrationBuilder.DropColumn(
                name: "ClassId",
                table: "Quiz");

            migrationBuilder.DropColumn(
                name: "EducationMediumId",
                table: "Quiz");

            migrationBuilder.DropColumn(
                name: "SubjectId",
                table: "Quiz");

            migrationBuilder.AddColumn<string>(
                name: "Subject",
                table: "Quiz",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
