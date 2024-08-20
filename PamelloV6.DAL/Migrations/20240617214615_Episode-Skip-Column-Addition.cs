using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PamelloV6.DAL.Migrations
{
    /// <inheritdoc />
    public partial class EpisodeSkipColumnAddition : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Skip",
                table: "Episodes",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Skip",
                table: "Episodes");
        }
    }
}
