using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QuizSystemRepository.Migrations
{
    /// <inheritdoc />
    public partial class Updatemodelss : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Name",
                table: "Student");

            migrationBuilder.DropColumn(
                name: "Name",
                table: "Instructor");

            migrationBuilder.AddColumn<string>(
                name: "FirstName",
                table: "Student",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LastName",
                table: "Student",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FirstName",
                table: "Instructor",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LastName",
                table: "Instructor",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FirstName",
                table: "Student");

            migrationBuilder.DropColumn(
                name: "LastName",
                table: "Student");

            migrationBuilder.DropColumn(
                name: "FirstName",
                table: "Instructor");

            migrationBuilder.DropColumn(
                name: "LastName",
                table: "Instructor");

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "Student",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "Instructor",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
