using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TalentShowCase.API.Migrations
{
    /// <inheritdoc />
    public partial class RemoveTalentCategoryUsersNav : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TalentCategoryUser");

            migrationBuilder.DropColumn(
                name: "Skill",
                table: "Users");

            migrationBuilder.AddColumn<int>(
                name: "UserId",
                table: "TalentCategories",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_TalentCategories_UserId",
                table: "TalentCategories",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_TalentCategories_Users_UserId",
                table: "TalentCategories",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TalentCategories_Users_UserId",
                table: "TalentCategories");

            migrationBuilder.DropIndex(
                name: "IX_TalentCategories_UserId",
                table: "TalentCategories");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "TalentCategories");

            migrationBuilder.AddColumn<string>(
                name: "Skill",
                table: "Users",
                type: "character varying(255)",
                maxLength: 255,
                nullable: true);

            migrationBuilder.CreateTable(
                name: "TalentCategoryUser",
                columns: table => new
                {
                    TalentCategoriesCategoryId = table.Column<int>(type: "integer", nullable: false),
                    UsersUserId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TalentCategoryUser", x => new { x.TalentCategoriesCategoryId, x.UsersUserId });
                    table.ForeignKey(
                        name: "FK_TalentCategoryUser_TalentCategories_TalentCategoriesCategor~",
                        column: x => x.TalentCategoriesCategoryId,
                        principalTable: "TalentCategories",
                        principalColumn: "CategoryId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TalentCategoryUser_Users_UsersUserId",
                        column: x => x.UsersUserId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TalentCategoryUser_UsersUserId",
                table: "TalentCategoryUser",
                column: "UsersUserId");
        }
    }
}
