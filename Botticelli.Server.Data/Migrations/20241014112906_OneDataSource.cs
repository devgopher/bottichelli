using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Botticelli.Server.Data.Migrations
{
    /// <inheritdoc />
    public partial class OneDataSource : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BotInfo_BotAdditionalInfo_AdditionalInfoBotId",
                table: "BotInfo");

            migrationBuilder.DropIndex(
                name: "IX_BotInfo_AdditionalInfoBotId",
                table: "BotInfo");

            migrationBuilder.DeleteData(
                table: "ApplicationRoles",
                keyColumn: "Id",
                keyValue: "11594aea-0910-40f8-8e37-53efc5e1a80f");

            migrationBuilder.DeleteData(
                table: "ApplicationRoles",
                keyColumn: "Id",
                keyValue: "2cb59f48-3d78-4056-b43a-5042b67d8b8d");

            migrationBuilder.DeleteData(
                table: "ApplicationRoles",
                keyColumn: "Id",
                keyValue: "e8bcf27d-e87d-462c-bbbd-3fc1a42c5701");

            migrationBuilder.DropColumn(
                name: "AdditionalInfoBotId",
                table: "BotInfo");

            migrationBuilder.AddColumn<string>(
                name: "BotInfoBotId",
                table: "BotAdditionalInfo",
                type: "TEXT",
                nullable: true);

            migrationBuilder.InsertData(
                table: "ApplicationRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "22c98256-7794-4d42-bdc8-4725445de3c9", "10/14/2024 11:29:05", "viewer", "VIEWER" },
                    { "969600de-70d9-43e9-bc01-56415c57863b", "10/14/2024 11:29:05", "admin", "ADMIN" },
                    { "b57f3fda-a6d3-4f93-8faa-ce99a06a476d", "10/14/2024 11:29:05", "bot_manager", "BOT_MANAGER" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_BotAdditionalInfo_BotInfoBotId",
                table: "BotAdditionalInfo",
                column: "BotInfoBotId");

            migrationBuilder.AddForeignKey(
                name: "FK_BotAdditionalInfo_BotInfo_BotInfoBotId",
                table: "BotAdditionalInfo",
                column: "BotInfoBotId",
                principalTable: "BotInfo",
                principalColumn: "BotId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BotAdditionalInfo_BotInfo_BotInfoBotId",
                table: "BotAdditionalInfo");

            migrationBuilder.DropIndex(
                name: "IX_BotAdditionalInfo_BotInfoBotId",
                table: "BotAdditionalInfo");

            migrationBuilder.DeleteData(
                table: "ApplicationRoles",
                keyColumn: "Id",
                keyValue: "22c98256-7794-4d42-bdc8-4725445de3c9");

            migrationBuilder.DeleteData(
                table: "ApplicationRoles",
                keyColumn: "Id",
                keyValue: "969600de-70d9-43e9-bc01-56415c57863b");

            migrationBuilder.DeleteData(
                table: "ApplicationRoles",
                keyColumn: "Id",
                keyValue: "b57f3fda-a6d3-4f93-8faa-ce99a06a476d");

            migrationBuilder.DropColumn(
                name: "BotInfoBotId",
                table: "BotAdditionalInfo");

            migrationBuilder.AddColumn<string>(
                name: "AdditionalInfoBotId",
                table: "BotInfo",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

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
    }
}
