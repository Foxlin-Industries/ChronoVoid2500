using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace ChronoVoid.API.Migrations
{
    /// <inheritdoc />
    public partial class Phase1_Scaffold : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Factions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    LeaderId = table.Column<int>(type: "integer", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    XP = table.Column<int>(type: "integer", nullable: false),
                    WarMode = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Factions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "OwnershipLogs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    PlanetId = table.Column<int>(type: "integer", nullable: false),
                    PreviousOwnerId = table.Column<int>(type: "integer", nullable: true),
                    NewOwnerId = table.Column<int>(type: "integer", nullable: true),
                    Timestamp = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Method = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OwnershipLogs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OwnershipLogs_Planets_PlanetId",
                        column: x => x.PlanetId,
                        principalTable: "Planets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OwnershipLogs_Users_NewOwnerId",
                        column: x => x.NewOwnerId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_OwnershipLogs_Users_PreviousOwnerId",
                        column: x => x.PreviousOwnerId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "PlanetContracts",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    PlanetId = table.Column<int>(type: "integer", nullable: false),
                    StarbaseId = table.Column<int>(type: "integer", nullable: true),
                    Price = table.Column<decimal>(type: "numeric", nullable: false),
                    ExpiresAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsUsed = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlanetContracts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PlanetContracts_Planets_PlanetId",
                        column: x => x.PlanetId,
                        principalTable: "Planets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PlanetContracts_Starbases_StarbaseId",
                        column: x => x.StarbaseId,
                        principalTable: "Starbases",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "PlanetProductions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    PlanetId = table.Column<int>(type: "integer", nullable: false),
                    ResourceType = table.Column<string>(type: "text", nullable: false),
                    BaseRate = table.Column<decimal>(type: "numeric", nullable: false),
                    CurrentStock = table.Column<int>(type: "integer", nullable: false),
                    LastUpdate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlanetProductions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PlanetProductions_Planets_PlanetId",
                        column: x => x.PlanetId,
                        principalTable: "Planets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Ships",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    OwnerId = table.Column<int>(type: "integer", nullable: false),
                    CurrentNodeId = table.Column<int>(type: "integer", nullable: true),
                    Tier = table.Column<int>(type: "integer", nullable: false),
                    CargoCapacity = table.Column<int>(type: "integer", nullable: false),
                    WeaponLevel = table.Column<int>(type: "integer", nullable: false),
                    ShieldLevel = table.Column<int>(type: "integer", nullable: false),
                    ComputerLevel = table.Column<int>(type: "integer", nullable: false),
                    LivesRemaining = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Ships", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Ships_NeuralNodes_CurrentNodeId",
                        column: x => x.CurrentNodeId,
                        principalTable: "NeuralNodes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Ships_Users_OwnerId",
                        column: x => x.OwnerId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TradeGoods",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    StarbaseId = table.Column<int>(type: "integer", nullable: false),
                    ResourceType = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    BuyPrice = table.Column<decimal>(type: "numeric", nullable: false),
                    SellPrice = table.Column<decimal>(type: "numeric", nullable: false),
                    Stock = table.Column<int>(type: "integer", nullable: false),
                    LastUpdate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TradeGoods", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TradeGoods_Starbases_StarbaseId",
                        column: x => x.StarbaseId,
                        principalTable: "Starbases",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TradeTransactions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    StarbaseId = table.Column<int>(type: "integer", nullable: false),
                    BuyerId = table.Column<int>(type: "integer", nullable: true),
                    SellerId = table.Column<int>(type: "integer", nullable: true),
                    ResourceType = table.Column<string>(type: "text", nullable: false),
                    Quantity = table.Column<int>(type: "integer", nullable: false),
                    Price = table.Column<decimal>(type: "numeric", nullable: false),
                    Timestamp = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TradeTransactions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TradeTransactions_Starbases_StarbaseId",
                        column: x => x.StarbaseId,
                        principalTable: "Starbases",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TradeTransactions_Users_BuyerId",
                        column: x => x.BuyerId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TradeTransactions_Users_SellerId",
                        column: x => x.SellerId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Troops",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    OwnerId = table.Column<int>(type: "integer", nullable: false),
                    PlanetId = table.Column<int>(type: "integer", nullable: false),
                    Level = table.Column<int>(type: "integer", nullable: false),
                    Count = table.Column<int>(type: "integer", nullable: false),
                    DeployedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DailyPay = table.Column<decimal>(type: "numeric", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Troops", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Troops_Planets_PlanetId",
                        column: x => x.PlanetId,
                        principalTable: "Planets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Troops_Users_OwnerId",
                        column: x => x.OwnerId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "FactionMembers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    FactionId = table.Column<int>(type: "integer", nullable: false),
                    UserId = table.Column<int>(type: "integer", nullable: false),
                    Role = table.Column<string>(type: "text", nullable: false),
                    JoinedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FactionMembers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FactionMembers_Factions_FactionId",
                        column: x => x.FactionId,
                        principalTable: "Factions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_FactionMembers_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ShipCargos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ShipId = table.Column<int>(type: "integer", nullable: false),
                    ResourceType = table.Column<string>(type: "text", nullable: false),
                    Quantity = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ShipCargos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ShipCargos_Ships_ShipId",
                        column: x => x.ShipId,
                        principalTable: "Ships",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_FactionMembers_FactionId_UserId",
                table: "FactionMembers",
                columns: new[] { "FactionId", "UserId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_FactionMembers_UserId",
                table: "FactionMembers",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Factions_Name",
                table: "Factions",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_OwnershipLogs_NewOwnerId",
                table: "OwnershipLogs",
                column: "NewOwnerId");

            migrationBuilder.CreateIndex(
                name: "IX_OwnershipLogs_PlanetId",
                table: "OwnershipLogs",
                column: "PlanetId");

            migrationBuilder.CreateIndex(
                name: "IX_OwnershipLogs_PreviousOwnerId",
                table: "OwnershipLogs",
                column: "PreviousOwnerId");

            migrationBuilder.CreateIndex(
                name: "IX_PlanetContracts_PlanetId",
                table: "PlanetContracts",
                column: "PlanetId");

            migrationBuilder.CreateIndex(
                name: "IX_PlanetContracts_StarbaseId",
                table: "PlanetContracts",
                column: "StarbaseId");

            migrationBuilder.CreateIndex(
                name: "IX_PlanetProductions_PlanetId",
                table: "PlanetProductions",
                column: "PlanetId");

            migrationBuilder.CreateIndex(
                name: "IX_ShipCargos_ShipId",
                table: "ShipCargos",
                column: "ShipId");

            migrationBuilder.CreateIndex(
                name: "IX_Ships_CurrentNodeId",
                table: "Ships",
                column: "CurrentNodeId");

            migrationBuilder.CreateIndex(
                name: "IX_Ships_OwnerId",
                table: "Ships",
                column: "OwnerId");

            migrationBuilder.CreateIndex(
                name: "IX_TradeGoods_StarbaseId_ResourceType",
                table: "TradeGoods",
                columns: new[] { "StarbaseId", "ResourceType" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TradeTransactions_BuyerId",
                table: "TradeTransactions",
                column: "BuyerId");

            migrationBuilder.CreateIndex(
                name: "IX_TradeTransactions_SellerId",
                table: "TradeTransactions",
                column: "SellerId");

            migrationBuilder.CreateIndex(
                name: "IX_TradeTransactions_StarbaseId",
                table: "TradeTransactions",
                column: "StarbaseId");

            migrationBuilder.CreateIndex(
                name: "IX_Troops_OwnerId",
                table: "Troops",
                column: "OwnerId");

            migrationBuilder.CreateIndex(
                name: "IX_Troops_PlanetId",
                table: "Troops",
                column: "PlanetId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FactionMembers");

            migrationBuilder.DropTable(
                name: "OwnershipLogs");

            migrationBuilder.DropTable(
                name: "PlanetContracts");

            migrationBuilder.DropTable(
                name: "PlanetProductions");

            migrationBuilder.DropTable(
                name: "ShipCargos");

            migrationBuilder.DropTable(
                name: "TradeGoods");

            migrationBuilder.DropTable(
                name: "TradeTransactions");

            migrationBuilder.DropTable(
                name: "Troops");

            migrationBuilder.DropTable(
                name: "Factions");

            migrationBuilder.DropTable(
                name: "Ships");
        }
    }
}
