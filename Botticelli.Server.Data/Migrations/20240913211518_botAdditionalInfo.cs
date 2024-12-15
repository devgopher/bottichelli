using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Botticelli.Server.Data.Migrations
{
    /// <inheritdoc />
    public partial class botAdditionalInfo : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "BotName",
                table: "BotInfo",
                type: "TEXT",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AdditionalInfoBotId",
                table: "BotInfo",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "BotAdditionalInfo",
                columns: table => new
                {
                    BotId = table.Column<string>(type: "TEXT", nullable: false),
                    ItemName = table.Column<string>(type: "TEXT", nullable: false),
                    ItemValue = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BotAdditionalInfo", x => x.BotId);
                });

            migrationBuilder.InsertData(
                table: "ApplicationRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "11594aea-0910-40f8-8e37-53efc5e1a80f", "09/13/2024 21:15:18", "bot_manager", "BOT_MANAGER" },
                    { "2cb59f48-3d78-4056-b43a-5042b67d8b8d", "09/13/2024 21:15:18", "admin", "ADMIN" },
                    { "e8bcf27d-e87d-462c-bbbd-3fc1a42c5701", "09/13/2024 21:15:18", "viewer", "VIEWER" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_BotInfo_AdditionalInfoBotId",
                table: "BotInfo",
                column: "AdditionalInfoBotId");

            migrationBuilder.AddForeignKey(
                name: "FK_BotInfo_BotAdditionalInfo_AdditionalInfoBotId",
                table: "BotInfo",
                column: "AdditionalInfoBotId",
                principalTable: "BotAdditionalInfo",
                principalColumn: "BotId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BotInfo_BotAdditionalInfo_AdditionalInfoBotId",
                table: "BotInfo");

            migrationBuilder.DropTable(
                name: "BotAdditionalInfo");

            migrationBuilder.DropIndex(
                name: "IX_BotInfo_AdditionalInfoBotId",
                table: "BotInfo");

            migrationBuilder.DropColumn(
                name: "AdditionalInfoBotId",
                table: "BotInfo");

            migrationBuilder.AlterColumn<string>(
                name: "BotName",
                table: "BotInfo",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.InsertData(
                table: "ApplicationRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "4997b014-6ca0-4736-aa14-db77741c9fbb", "09/13/2024 21:12:15", "viewer", "VIEWER" },
                    { "4b585771-7feb-4e6a-aa05-09076415edf1", "09/13/2024 21:12:15", "bot_manager", "BOT_MANAGER" },
                    { "91f76d24-9f30-446c-b3db-42a6cff5c1a6", "09/13/2024 21:12:15", "admin", "ADMIN" }
                });
        }
    }
}
