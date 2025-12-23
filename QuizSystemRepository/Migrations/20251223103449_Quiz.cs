using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QuizSystemRepository.Migrations
{
    /// <inheritdoc />
    public partial class Quiz : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Quiz",
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
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    StartAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ÈndAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Duration = table.Column<DateTime>(type: "datetime2", nullable: true),
                    TotalMarks = table.Column<int>(type: "int", nullable: false),
                    RequiredPassPercentage = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Quiz", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Quiz_AspNetUsers_ApprovedById",
                        column: x => x.ApprovedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Quiz_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Quiz_AspNetUsers_ModifiedById",
                        column: x => x.ModifiedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Quiz_AspNetUsers_RejectedById",
                        column: x => x.RejectedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Quiz_ApprovedById",
                table: "Quiz",
                column: "ApprovedById");

            migrationBuilder.CreateIndex(
                name: "IX_Quiz_CreatedById",
                table: "Quiz",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_Quiz_ModifiedById",
                table: "Quiz",
                column: "ModifiedById");

            migrationBuilder.CreateIndex(
                name: "IX_Quiz_RejectedById",
                table: "Quiz",
                column: "RejectedById");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Quiz");
        }
    }
}
