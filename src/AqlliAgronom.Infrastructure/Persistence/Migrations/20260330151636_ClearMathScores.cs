using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AqlliAgronom.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class ClearMathScores : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Reset leaderboard — delete all previous scores so the board starts fresh
            migrationBuilder.Sql("DELETE FROM math_scores;");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
