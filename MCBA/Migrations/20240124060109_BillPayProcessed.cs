using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MCBA.Migrations
{
    /// <inheritdoc />
    public partial class BillPayProcessed : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Processed",
                table: "BillPays",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Processed",
                table: "BillPays");
        }
    }
}
