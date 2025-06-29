using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TalentShowCase.API.Migrations
{
    /// <inheritdoc />
    public partial class UserTalentCategoryManyToMany : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "UserTalentCategories",
                columns: table => new
                {
                    TalentCategoriesCategoryId = table.Column<int>(type: "integer", nullable: false),
                    UsersUserId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserTalentCategories", x => new { x.TalentCategoriesCategoryId, x.UsersUserId });
                    table.ForeignKey(
                        name: "FK_UserTalentCategories_TalentCategories_TalentCategoriesCateg~",
                        column: x => x.TalentCategoriesCategoryId,
                        principalTable: "TalentCategories",
                        principalColumn: "CategoryId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserTalentCategories_Users_UsersUserId",
                        column: x => x.UsersUserId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserTalentCategories_UsersUserId",
                table: "UserTalentCategories",
                column: "UsersUserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserTalentCategories");
        }
    }
}
