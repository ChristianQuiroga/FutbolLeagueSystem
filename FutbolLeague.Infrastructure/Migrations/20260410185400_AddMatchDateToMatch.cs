using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FutbolLeague.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddMatchDateToMatch : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "MatchDate",
                table: "Matches",
                type: "timestamp with time zone",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MatchDate",
                table: "Matches");
        }
    }
}
