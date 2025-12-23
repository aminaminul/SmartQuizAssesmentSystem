using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QuizSystemRepository.Migrations
{
    /// <inheritdoc />
    public partial class ChangedtableNames : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Classe_AspNetUsers_ApprovedById",
                table: "Classe");

            migrationBuilder.DropForeignKey(
                name: "FK_Classe_AspNetUsers_CreatedById",
                table: "Classe");

            migrationBuilder.DropForeignKey(
                name: "FK_Classe_AspNetUsers_ModifiedById",
                table: "Classe");

            migrationBuilder.DropForeignKey(
                name: "FK_Classe_AspNetUsers_RejectedById",
                table: "Classe");

            migrationBuilder.DropForeignKey(
                name: "FK_Classe_EducationMedium_EducationMediumId",
                table: "Classe");

            migrationBuilder.DropForeignKey(
                name: "FK_Student_Classe_ClassId",
                table: "Student");

            migrationBuilder.DropForeignKey(
                name: "FK_Subject_Classe_ClassId",
                table: "Subject");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Classe",
                table: "Classe");

            migrationBuilder.RenameTable(
                name: "Classe",
                newName: "Class");

            migrationBuilder.RenameIndex(
                name: "IX_Classe_RejectedById",
                table: "Class",
                newName: "IX_Class_RejectedById");

            migrationBuilder.RenameIndex(
                name: "IX_Classe_ModifiedById",
                table: "Class",
                newName: "IX_Class_ModifiedById");

            migrationBuilder.RenameIndex(
                name: "IX_Classe_EducationMediumId",
                table: "Class",
                newName: "IX_Class_EducationMediumId");

            migrationBuilder.RenameIndex(
                name: "IX_Classe_CreatedById",
                table: "Class",
                newName: "IX_Class_CreatedById");

            migrationBuilder.RenameIndex(
                name: "IX_Classe_ApprovedById",
                table: "Class",
                newName: "IX_Class_ApprovedById");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Class",
                table: "Class",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Class_AspNetUsers_ApprovedById",
                table: "Class",
                column: "ApprovedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Class_AspNetUsers_CreatedById",
                table: "Class",
                column: "CreatedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Class_AspNetUsers_ModifiedById",
                table: "Class",
                column: "ModifiedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Class_AspNetUsers_RejectedById",
                table: "Class",
                column: "RejectedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Class_EducationMedium_EducationMediumId",
                table: "Class",
                column: "EducationMediumId",
                principalTable: "EducationMedium",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Student_Class_ClassId",
                table: "Student",
                column: "ClassId",
                principalTable: "Class",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Subject_Class_ClassId",
                table: "Subject",
                column: "ClassId",
                principalTable: "Class",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Class_AspNetUsers_ApprovedById",
                table: "Class");

            migrationBuilder.DropForeignKey(
                name: "FK_Class_AspNetUsers_CreatedById",
                table: "Class");

            migrationBuilder.DropForeignKey(
                name: "FK_Class_AspNetUsers_ModifiedById",
                table: "Class");

            migrationBuilder.DropForeignKey(
                name: "FK_Class_AspNetUsers_RejectedById",
                table: "Class");

            migrationBuilder.DropForeignKey(
                name: "FK_Class_EducationMedium_EducationMediumId",
                table: "Class");

            migrationBuilder.DropForeignKey(
                name: "FK_Student_Class_ClassId",
                table: "Student");

            migrationBuilder.DropForeignKey(
                name: "FK_Subject_Class_ClassId",
                table: "Subject");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Class",
                table: "Class");

            migrationBuilder.RenameTable(
                name: "Class",
                newName: "Classe");

            migrationBuilder.RenameIndex(
                name: "IX_Class_RejectedById",
                table: "Classe",
                newName: "IX_Classe_RejectedById");

            migrationBuilder.RenameIndex(
                name: "IX_Class_ModifiedById",
                table: "Classe",
                newName: "IX_Classe_ModifiedById");

            migrationBuilder.RenameIndex(
                name: "IX_Class_EducationMediumId",
                table: "Classe",
                newName: "IX_Classe_EducationMediumId");

            migrationBuilder.RenameIndex(
                name: "IX_Class_CreatedById",
                table: "Classe",
                newName: "IX_Classe_CreatedById");

            migrationBuilder.RenameIndex(
                name: "IX_Class_ApprovedById",
                table: "Classe",
                newName: "IX_Classe_ApprovedById");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Classe",
                table: "Classe",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Classe_AspNetUsers_ApprovedById",
                table: "Classe",
                column: "ApprovedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Classe_AspNetUsers_CreatedById",
                table: "Classe",
                column: "CreatedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Classe_AspNetUsers_ModifiedById",
                table: "Classe",
                column: "ModifiedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Classe_AspNetUsers_RejectedById",
                table: "Classe",
                column: "RejectedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Classe_EducationMedium_EducationMediumId",
                table: "Classe",
                column: "EducationMediumId",
                principalTable: "EducationMedium",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Student_Classe_ClassId",
                table: "Student",
                column: "ClassId",
                principalTable: "Classe",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Subject_Classe_ClassId",
                table: "Subject",
                column: "ClassId",
                principalTable: "Classe",
                principalColumn: "Id");
        }
    }
}
