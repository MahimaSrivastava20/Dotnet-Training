using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PolicyService.Migrations
{
    /// <inheritdoc />
    public partial class AddRemainingCoverageAmount : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "RemainingCoverageAmount",
                table: "CustomerPolicies",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RemainingCoverageAmount",
                table: "CustomerPolicies");
        }
    }
}
