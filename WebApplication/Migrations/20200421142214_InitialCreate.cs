using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace WebApplication.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Metrics",
                columns: table => new
                {
                    MetricsID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Humidity = table.Column<double>(nullable: false),
                    Temperature = table.Column<double>(nullable: false),
                    Noise = table.Column<double>(nullable: false),
                    CO2 = table.Column<double>(nullable: false),
                    LastUpdated = table.Column<DateTime>(nullable: false),
                    WeeklyStatisticsID = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Metrics", x => x.MetricsID);
                });

            migrationBuilder.CreateTable(
                name: "RecommendedLevels",
                columns: table => new
                {
                    Humidity = table.Column<double>(nullable: false),
                    Temperature = table.Column<double>(nullable: false),
                    Noise = table.Column<double>(nullable: false),
                    CO2 = table.Column<double>(nullable: false)
                },
                constraints: table =>
                {
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Metrics");

            migrationBuilder.DropTable(
                name: "RecommendedLevels");
        }
    }
}
