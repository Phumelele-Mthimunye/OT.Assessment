using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OT.Assessment.App.Migrations
{
    /// <inheritdoc />
    public partial class init : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Players",
                columns: table => new
                {
                    accountId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    username = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    email = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    createdDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Players", x => x.accountId);
                });

            migrationBuilder.CreateTable(
                name: "Providers",
                columns: table => new
                {
                    providerId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Providers", x => x.providerId);
                });

            migrationBuilder.CreateTable(
                name: "TransactionTypes",
                columns: table => new
                {
                    transactionTypeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TransactionTypes", x => x.transactionTypeId);
                });

            migrationBuilder.CreateTable(
                name: "Games",
                columns: table => new
                {
                    gameId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    theme = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    ProviderId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Games", x => x.gameId);
                    table.ForeignKey(
                        name: "FK_Games_Providers_ProviderId",
                        column: x => x.ProviderId,
                        principalTable: "Providers",
                        principalColumn: "providerId");
                });

            migrationBuilder.CreateTable(
                name: "CasinoWagers",
                columns: table => new
                {
                    wagerId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    GameId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ProviderId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    AccountId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    externalReferenceId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    TransactionTypeId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    transactionId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    brandId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    amount = table.Column<double>(type: "float", nullable: true),
                    username = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    createdDateTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    numberOfBets = table.Column<int>(type: "int", nullable: true),
                    countryCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    sessionData = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    duration = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CasinoWagers", x => x.wagerId);
                    table.ForeignKey(
                        name: "FK_CasinoWagers_Games_GameId",
                        column: x => x.GameId,
                        principalTable: "Games",
                        principalColumn: "gameId");
                    table.ForeignKey(
                        name: "FK_CasinoWagers_Players_AccountId",
                        column: x => x.AccountId,
                        principalTable: "Players",
                        principalColumn: "accountId");
                    table.ForeignKey(
                        name: "FK_CasinoWagers_Providers_ProviderId",
                        column: x => x.ProviderId,
                        principalTable: "Providers",
                        principalColumn: "providerId");
                    table.ForeignKey(
                        name: "FK_CasinoWagers_TransactionTypes_TransactionTypeId",
                        column: x => x.TransactionTypeId,
                        principalTable: "TransactionTypes",
                        principalColumn: "transactionTypeId");
                });

            migrationBuilder.CreateIndex(
                name: "IX_CasinoWagers_AccountId",
                table: "CasinoWagers",
                column: "AccountId");

            migrationBuilder.CreateIndex(
                name: "IX_CasinoWagers_GameId",
                table: "CasinoWagers",
                column: "GameId");

            migrationBuilder.CreateIndex(
                name: "IX_CasinoWagers_ProviderId",
                table: "CasinoWagers",
                column: "ProviderId");

            migrationBuilder.CreateIndex(
                name: "IX_CasinoWagers_TransactionTypeId",
                table: "CasinoWagers",
                column: "TransactionTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_Games_ProviderId",
                table: "Games",
                column: "ProviderId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CasinoWagers");

            migrationBuilder.DropTable(
                name: "Games");

            migrationBuilder.DropTable(
                name: "Players");

            migrationBuilder.DropTable(
                name: "TransactionTypes");

            migrationBuilder.DropTable(
                name: "Providers");
        }
    }
}
