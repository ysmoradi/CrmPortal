using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace CrmPortal.Migrations
{
    public partial class Customer_CreatedBy : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "CreatedById",
                table: "Customers",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_Customers_CreatedById",
                table: "Customers",
                column: "CreatedById");

            migrationBuilder.AddForeignKey(
                name: "FK_Customers_Users_CreatedById",
                table: "Customers",
                column: "CreatedById",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Customers_Users_CreatedById",
                table: "Customers");

            migrationBuilder.DropIndex(
                name: "IX_Customers_CreatedById",
                table: "Customers");

            migrationBuilder.DropColumn(
                name: "CreatedById",
                table: "Customers");
        }
    }
}
