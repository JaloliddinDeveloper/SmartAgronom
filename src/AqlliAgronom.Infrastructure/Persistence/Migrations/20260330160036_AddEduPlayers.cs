using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AqlliAgronom.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddEduPlayers : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "edu_players",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_edu_players", x => x.id);
                });

            migrationBuilder.CreateIndex(
                name: "ix_edu_players_name",
                table: "edu_players",
                column: "name",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "edu_players");
        }
    }
}
