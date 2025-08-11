using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ChronoVoid.API.Migrations
{
    /// <inheritdoc />
    public partial class AddAdvancedRealmSettings : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ActiveAlienRaces",
                table: "NexusRealms",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<double>(
                name: "ArtifactSystemChance",
                table: "NexusRealms",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "NexusRealms",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "EnableArtifactSystems",
                table: "NexusRealms",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "MaxAlienTechLevel",
                table: "NexusRealms",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "MaxPlanetsPerSystem",
                table: "NexusRealms",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "MinAlienTechLevel",
                table: "NexusRealms",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "MinPlanetsPerSystem",
                table: "NexusRealms",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "PlanetDensity",
                table: "NexusRealms",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "PlanetPurchaseContracts",
                table: "NexusRealms",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ResourceDensity",
                table: "NexusRealms",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ActiveAlienRaces",
                table: "NexusRealms");

            migrationBuilder.DropColumn(
                name: "ArtifactSystemChance",
                table: "NexusRealms");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "NexusRealms");

            migrationBuilder.DropColumn(
                name: "EnableArtifactSystems",
                table: "NexusRealms");

            migrationBuilder.DropColumn(
                name: "MaxAlienTechLevel",
                table: "NexusRealms");

            migrationBuilder.DropColumn(
                name: "MaxPlanetsPerSystem",
                table: "NexusRealms");

            migrationBuilder.DropColumn(
                name: "MinAlienTechLevel",
                table: "NexusRealms");

            migrationBuilder.DropColumn(
                name: "MinPlanetsPerSystem",
                table: "NexusRealms");

            migrationBuilder.DropColumn(
                name: "PlanetDensity",
                table: "NexusRealms");

            migrationBuilder.DropColumn(
                name: "PlanetPurchaseContracts",
                table: "NexusRealms");

            migrationBuilder.DropColumn(
                name: "ResourceDensity",
                table: "NexusRealms");
        }
    }
}
