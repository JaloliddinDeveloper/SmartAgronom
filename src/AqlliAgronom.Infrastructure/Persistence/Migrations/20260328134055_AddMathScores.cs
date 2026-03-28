using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AqlliAgronom.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddMathScores : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "math_scores",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    player_name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    score = table.Column<int>(type: "integer", nullable: false),
                    correct_answers = table.Column<int>(type: "integer", nullable: false),
                    best_streak = table.Column<int>(type: "integer", nullable: false),
                    level_reached = table.Column<int>(type: "integer", nullable: false),
                    difficulty = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_math_scores", x => x.id);
                });

            migrationBuilder.CreateIndex(
                name: "ix_math_scores_difficulty_score",
                table: "math_scores",
                columns: new[] { "difficulty", "score" });

            migrationBuilder.CreateIndex(
                name: "ix_math_scores_score",
                table: "math_scores",
                column: "score");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "math_scores");
        }
    }
}
