using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Botticelli.Bot.Data.Migrations
{
    /// <inheritdoc />
    public partial class botdata : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "BotData",
                columns: table => new
                {
                    BotId = table.Column<string>(type: "TEXT", nullable: false),
                    Status = table.Column<int>(type: "INTEGER", nullable: true),
                    Type = table.Column<int>(type: "INTEGER", nullable: true),
                    BotKey = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BotData", x => x.BotId);
                });

            migrationBuilder.CreateTable(
                name: "BotAdditionalInfos",
                columns: table => new
                {
                    BotId = table.Column<string>(type: "TEXT", nullable: false),
                    ItemName = table.Column<string>(type: "TEXT", nullable: false),
                    ItemValue = table.Column<string>(type: "TEXT", nullable: true),
                    BotDataBotId = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BotAdditionalInfos", x => x.BotId);
                    table.ForeignKey(
                        name: "FK_BotAdditionalInfos_BotData_BotDataBotId",
                        column: x => x.BotDataBotId,
                        principalTable: "BotData",
                        principalColumn: "BotId");
                });

            migrationBuilder.CreateIndex(
                name: "IX_BotAdditionalInfos_BotDataBotId",
                table: "BotAdditionalInfos",
                column: "BotDataBotId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BotAdditionalInfos");

            migrationBuilder.DropTable(
                name: "BotData");
        }
    }
}
