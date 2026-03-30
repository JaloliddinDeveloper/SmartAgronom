using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AqlliAgronom.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class NormalizeDifficultyToSubjectOnly : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Normalize difficulty to subject name only: "math-grade4" → "math"
            migrationBuilder.Sql("UPDATE math_scores SET difficulty = 'math' WHERE difficulty LIKE 'math%';");
            migrationBuilder.Sql("UPDATE math_scores SET difficulty = 'geo'  WHERE difficulty LIKE 'geo%';");
            migrationBuilder.Sql("UPDATE math_scores SET difficulty = 'hist' WHERE difficulty LIKE 'hist%';");
            migrationBuilder.Sql("UPDATE math_scores SET difficulty = 'eng'  WHERE difficulty LIKE 'eng%';");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
