using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace CrmPortal.Migrations
{
    public partial class InitalBlackListSchema : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "BlackLists",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Value = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BlackLists", x => x.Id);
                });

            migrationBuilder.InsertData(table: "BlackLists", columns: new[] { "Id", "Value" }, values: new object[] { Guid.NewGuid(), "Bad1" });
            migrationBuilder.InsertData(table: "BlackLists", columns: new[] { "Id", "Value" }, values: new object[] { Guid.NewGuid(), "Bad2" });
            migrationBuilder.InsertData(table: "BlackLists", columns: new[] { "Id", "Value" }, values: new object[] { Guid.NewGuid(), "Bad3" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BlackLists");
        }
    }
}
