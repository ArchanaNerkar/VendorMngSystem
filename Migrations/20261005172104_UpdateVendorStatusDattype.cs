using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VendorMngSystem.Migrations
{
    /// <inheritdoc />
    public partial class UpdateVendorStatusDattype : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<bool>(
                name: "VendorStatus",
                table: "Vendors",
                type: "bit",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "VendorStatus",
                table: "Vendors",
                type: "int",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit");
        }
    }
}
