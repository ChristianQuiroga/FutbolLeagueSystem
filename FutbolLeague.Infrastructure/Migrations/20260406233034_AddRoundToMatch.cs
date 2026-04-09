using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FutbolLeague.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddRoundToMatch : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Round",
                table: "Matches",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Round",
                table: "Matches");
        }
    }
}
