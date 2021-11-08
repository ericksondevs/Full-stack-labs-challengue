using System;
using DataAccessLayer.Data;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;


namespace Entities.Migrations
{
    [Migration("202107111428")]
    public class _202107111428_create_table_url_statistics_t : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "UrlStastics",
                columns: table => new
                {
                    UrlStatId = table.Column<int>(type: "uniqueidentifier", nullable: false),
                    UserAgent = table.Column<string>(),
                    Browser = table.Column<String>(),
                    Date = table.Column<DateTime>(),
                    URL_UrlId = table.Column<int>(),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UrlStat", x => x.UrlStatId);
                    table.ForeignKey("FK_UrlStat", x => x.URL_UrlId, "Urls", "Id", onDelete: ReferentialAction.Cascade);
                }); ;
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey("FK_UrlStat", "UrlStastics");
            migrationBuilder.DropTable(
                name: "UrlStastics");
        }
    }
}
