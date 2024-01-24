using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MCBA.Migrations
{
    /// <inheritdoc />
    public partial class BillPayFKUpdate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BillPays_Accounts_AccountNumber1",
                table: "BillPays");

            migrationBuilder.DropIndex(
                name: "IX_BillPays_AccountNumber1",
                table: "BillPays");

            migrationBuilder.DropColumn(
                name: "AccountNumber1",
                table: "BillPays");

            migrationBuilder.CreateIndex(
                name: "IX_BillPays_AccountNumber",
                table: "BillPays",
                column: "AccountNumber");

            migrationBuilder.AddForeignKey(
                name: "FK_BillPays_Accounts_AccountNumber",
                table: "BillPays",
                column: "AccountNumber",
                principalTable: "Accounts",
                principalColumn: "AccountNumber",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BillPays_Accounts_AccountNumber",
                table: "BillPays");

            migrationBuilder.DropIndex(
                name: "IX_BillPays_AccountNumber",
                table: "BillPays");

            migrationBuilder.AddColumn<int>(
                name: "AccountNumber1",
                table: "BillPays",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_BillPays_AccountNumber1",
                table: "BillPays",
                column: "AccountNumber1");

            migrationBuilder.AddForeignKey(
                name: "FK_BillPays_Accounts_AccountNumber1",
                table: "BillPays",
                column: "AccountNumber1",
                principalTable: "Accounts",
                principalColumn: "AccountNumber",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
