using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QuizSystemRepository.Migrations
{
    
    public partial class quizup : Migration
    {
        
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "NegativeMarking",
                table: "Quiz",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);
        }

        
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "NegativeMarking",
                table: "Quiz");
        }
    }
}
