using CrmPortal.Util;
using Microsoft.EntityFrameworkCore.Migrations;
using System;

namespace CrmPortal.Migrations
{
    public partial class InitialUserSchemaAndData : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    UserName = table.Column<string>(maxLength: 50, nullable: false),
                    Password = table.Column<string>(maxLength: 250, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Users_UserName",
                table: "Users",
                column: "UserName",
                unique: true);

            migrationBuilder.InsertData(table: "Users", columns: new[] { "Id", "UserName", "Password" }, values: new object[] { Guid.Parse("d8f7d3c9-79d2-46a1-ac38-7d4a8273651d"), "User1", HashUtility.Hash("P@ssw0rd") });
            migrationBuilder.InsertData(table: "Users", columns: new[] { "Id", "UserName", "Password" }, values: new object[] { Guid.Parse("0d993be2-2539-48ac-97b9-d395ef939ead"), "User2", HashUtility.Hash("P@ssw0rd") });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
