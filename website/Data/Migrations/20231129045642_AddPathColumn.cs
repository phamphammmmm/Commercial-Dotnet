using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace website.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddPathColumn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Path",
                table: "Users",
                type: "text",  // Set the appropriate data type for your path
                nullable: true,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Path",
                table: "Users");
        }
    }
}
