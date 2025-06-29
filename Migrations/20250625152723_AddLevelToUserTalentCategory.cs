using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TalentShowCase.API.Migrations
{
    /// <inheritdoc />
    public partial class AddLevelToUserTalentCategory : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserTalentCategories_TalentCategories_TalentCategoriesCateg~",
                table: "UserTalentCategories");

            migrationBuilder.DropForeignKey(
                name: "FK_UserTalentCategories_Users_UsersUserId",
                table: "UserTalentCategories");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UserTalentCategories",
                table: "UserTalentCategories");

            migrationBuilder.DropIndex(
                name: "IX_UserTalentCategories_UsersUserId",
                table: "UserTalentCategories");

            migrationBuilder.RenameColumn(
                name: "UsersUserId",
                table: "UserTalentCategories",
                newName: "Level");

            migrationBuilder.RenameColumn(
                name: "TalentCategoriesCategoryId",
                table: "UserTalentCategories",
                newName: "TalentCategoryId");

            migrationBuilder.AddColumn<int>(
                name: "UserId",
                table: "UserTalentCategories",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserTalentCategories",
                table: "UserTalentCategories",
                columns: new[] { "UserId", "TalentCategoryId" });

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
                name: "IX_UserTalentCategories_TalentCategoryId",
                table: "UserTalentCategories",
                column: "TalentCategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_TalentCategoryUser_UsersUserId",
                table: "TalentCategoryUser",
                column: "UsersUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_UserTalentCategories_TalentCategories_TalentCategoryId",
                table: "UserTalentCategories",
                column: "TalentCategoryId",
                principalTable: "TalentCategories",
                principalColumn: "CategoryId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserTalentCategories_Users_UserId",
                table: "UserTalentCategories",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserTalentCategories_TalentCategories_TalentCategoryId",
                table: "UserTalentCategories");

            migrationBuilder.DropForeignKey(
                name: "FK_UserTalentCategories_Users_UserId",
                table: "UserTalentCategories");

            migrationBuilder.DropTable(
                name: "TalentCategoryUser");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UserTalentCategories",
                table: "UserTalentCategories");

            migrationBuilder.DropIndex(
                name: "IX_UserTalentCategories_TalentCategoryId",
                table: "UserTalentCategories");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "UserTalentCategories");

            migrationBuilder.RenameColumn(
                name: "Level",
                table: "UserTalentCategories",
                newName: "UsersUserId");

            migrationBuilder.RenameColumn(
                name: "TalentCategoryId",
                table: "UserTalentCategories",
                newName: "TalentCategoriesCategoryId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserTalentCategories",
                table: "UserTalentCategories",
                columns: new[] { "TalentCategoriesCategoryId", "UsersUserId" });

            migrationBuilder.CreateIndex(
                name: "IX_UserTalentCategories_UsersUserId",
                table: "UserTalentCategories",
                column: "UsersUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_UserTalentCategories_TalentCategories_TalentCategoriesCateg~",
                table: "UserTalentCategories",
                column: "TalentCategoriesCategoryId",
                principalTable: "TalentCategories",
                principalColumn: "CategoryId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserTalentCategories_Users_UsersUserId",
                table: "UserTalentCategories",
                column: "UsersUserId",
                principalTable: "Users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
