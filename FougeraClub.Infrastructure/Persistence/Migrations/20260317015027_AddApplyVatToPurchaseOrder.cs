using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FougeraClub.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddApplyVatToPurchaseOrder : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "ApplyVat",
                table: "PurchaseOrders",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ApplyVat",
                table: "PurchaseOrders");
        }
    }
}
