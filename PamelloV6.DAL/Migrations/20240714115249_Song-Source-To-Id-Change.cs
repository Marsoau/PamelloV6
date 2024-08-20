using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PamelloV6.DAL.Migrations
{
    /// <inheritdoc />
    public partial class SongSourceToIdChange : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "SourceUrl",
                table: "Songs",
                newName: "YoutubeId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "YoutubeId",
                table: "Songs",
                newName: "SourceUrl");
        }
    }
}
