using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Botticelli.Server.Data.Migrations
{
    /// <inheritdoc />
    public partial class BroadCastingServer : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Broadcasts",
                columns: table => new
                {
                    BotId = table.Column<string>(type: "TEXT", nullable: false),
                    Body = table.Column<string>(type: "TEXT", nullable: false),
                    Timestamp = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Broadcasts", x => x.BotId);
                });

            migrationBuilder.CreateTable(
                name: "BroadcastAttachments",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    MediaType = table.Column<int>(type: "INTEGER", nullable: false),
                    Filename = table.Column<string>(type: "TEXT", nullable: false),
                    Content = table.Column<byte[]>(type: "BLOB", nullable: false),
                    BroadcastBotId = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BroadcastAttachments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BroadcastAttachments_Broadcasts_BroadcastBotId",
                        column: x => x.BroadcastBotId,
                        principalTable: "Broadcasts",
                        principalColumn: "BotId");
                });

            migrationBuilder.CreateIndex(
                name: "IX_BroadcastAttachments_BroadcastBotId",
                table: "BroadcastAttachments",
                column: "BroadcastBotId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BroadcastAttachments");

            migrationBuilder.DropTable(
                name: "Broadcasts");
        }
    }
}
