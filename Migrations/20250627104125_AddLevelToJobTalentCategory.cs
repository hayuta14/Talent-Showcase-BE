using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TalentShowCase.API.Migrations
{
    /// <inheritdoc />
    public partial class AddLevelToJobTalentCategory : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Level",
                table: "JobTalentCategories",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Level",
                table: "JobTalentCategories");
        }
    }
}
