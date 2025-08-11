using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace ChronoVoid.API.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "NexusRealms",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    NodeCount = table.Column<int>(type: "integer", nullable: false),
                    QuantumStationSeedRate = table.Column<int>(type: "integer", nullable: false),
                    NoDeadNodes = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NexusRealms", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "NeuralNodes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    RealmId = table.Column<int>(type: "integer", nullable: false),
                    NodeNumber = table.Column<int>(type: "integer", nullable: false),
                    CoordinateX = table.Column<int>(type: "integer", nullable: false),
                    CoordinateY = table.Column<int>(type: "integer", nullable: false),
                    HasQuantumStation = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NeuralNodes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_NeuralNodes_NexusRealms_RealmId",
                        column: x => x.RealmId,
                        principalTable: "NexusRealms",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "HyperTunnels",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    FromNodeId = table.Column<int>(type: "integer", nullable: false),
                    ToNodeId = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HyperTunnels", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HyperTunnels_NeuralNodes_FromNodeId",
                        column: x => x.FromNodeId,
                        principalTable: "NeuralNodes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_HyperTunnels_NeuralNodes_ToNodeId",
                        column: x => x.ToNodeId,
                        principalTable: "NeuralNodes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Username = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Email = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    CurrentNodeId = table.Column<int>(type: "integer", nullable: true),
                    RealmId = table.Column<int>(type: "integer", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    LastLogin = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Users_NeuralNodes_CurrentNodeId",
                        column: x => x.CurrentNodeId,
                        principalTable: "NeuralNodes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Users_NexusRealms_RealmId",
                        column: x => x.RealmId,
                        principalTable: "NexusRealms",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateIndex(
                name: "IX_HyperTunnels_FromNodeId_ToNodeId",
                table: "HyperTunnels",
                columns: new[] { "FromNodeId", "ToNodeId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_HyperTunnels_ToNodeId",
                table: "HyperTunnels",
                column: "ToNodeId");

            migrationBuilder.CreateIndex(
                name: "IX_NeuralNodes_RealmId_NodeNumber",
                table: "NeuralNodes",
                columns: new[] { "RealmId", "NodeNumber" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_NexusRealms_Name",
                table: "NexusRealms",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_CurrentNodeId",
                table: "Users",
                column: "CurrentNodeId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_Email",
                table: "Users",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_RealmId",
                table: "Users",
                column: "RealmId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_Username",
                table: "Users",
                column: "Username",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "HyperTunnels");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "NeuralNodes");

            migrationBuilder.DropTable(
                name: "NexusRealms");
        }
    }
}
