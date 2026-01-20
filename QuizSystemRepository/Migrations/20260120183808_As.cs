using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QuizSystemRepository.Migrations
{
    
    public partial class As : Migration
    {
        
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "ClassId",
                table: "Instructor",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Instructor_ClassId",
                table: "Instructor",
                column: "ClassId");

            migrationBuilder.AddForeignKey(
                name: "FK_Instructor_Class_ClassId",
                table: "Instructor",
                column: "ClassId",
                principalTable: "Class",
                principalColumn: "Id");
        }

        
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Instructor_Class_ClassId",
                table: "Instructor");

            migrationBuilder.DropIndex(
                name: "IX_Instructor_ClassId",
                table: "Instructor");

            migrationBuilder.DropColumn(
                name: "ClassId",
                table: "Instructor");
        }
    }
}
