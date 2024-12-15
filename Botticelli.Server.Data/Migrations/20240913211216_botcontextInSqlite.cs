using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Botticelli.Server.Data.Migrations
{
    /// <inheritdoc />
    public partial class botcontextInSqlite : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "ApplicationRoles",
                keyColumn: "Id",
                keyValue: "4997b014-6ca0-4736-aa14-db77741c9fbb");

            migrationBuilder.DeleteData(
                table: "ApplicationRoles",
                keyColumn: "Id",
                keyValue: "4b585771-7feb-4e6a-aa05-09076415edf1");

            migrationBuilder.DeleteData(
                table: "ApplicationRoles",
                keyColumn: "Id",
                keyValue: "91f76d24-9f30-446c-b3db-42a6cff5c1a6");
        }
    }
}
