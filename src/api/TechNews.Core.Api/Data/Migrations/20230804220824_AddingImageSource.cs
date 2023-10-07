using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TechNews.Core.Api.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddingImageSource : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ImageSource",
                table: "News",
                type: "VARCHAR(500)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ImageSource",
                table: "Authors",
                type: "VARCHAR(500)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ImageSource",
                table: "News");

            migrationBuilder.DropColumn(
                name: "ImageSource",
                table: "Authors");
        }
    }
}
