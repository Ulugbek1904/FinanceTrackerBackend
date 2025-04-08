using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FinanceTracker.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddingExtrFeatureForAccount : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Source",
                table: "Accounts",
                newName: "Type");

            migrationBuilder.AddColumn<bool>(
                name: "IsPrimary",
                table: "Accounts",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsPrimary",
                table: "Accounts");

            migrationBuilder.RenameColumn(
                name: "Type",
                table: "Accounts",
                newName: "Source");
        }
    }
}
