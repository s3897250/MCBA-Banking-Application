using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MCBA.Migrations
{
    /// <inheritdoc />
    public partial class BillPayUpdate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsFailed",
                table: "BillPays",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsFailed",
                table: "BillPays");
        }
    }
}
