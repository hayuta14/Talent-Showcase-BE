using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;
using TalentShowCase.API.Models;

#nullable disable

namespace TalentShowCase.API.Migrations
{
    /// <inheritdoc />
    public partial class AddSubCommentsToComment : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "SubComments",
                table: "Comments",
                type: "jsonb",
                nullable: false,
                defaultValueSql: "'[]'");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SubComments",
                table: "Comments");
        }
    }
}
