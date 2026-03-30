using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace friendzone_backend.Migrations
{
    /// <inheritdoc />
    public partial class AddLocationIndexes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                "CREATE INDEX idx_location_userid ON \"Locations\"(\"UserId\");"
            );
            migrationBuilder.Sql(
                "CREATE INDEX idx_location_updatedat ON \"Locations\"(\"UpdatedAt\");"
            );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DROP INDEX IF EXISTS idx_location_userid;");
            migrationBuilder.Sql("DROP INDEX IF EXISTS idx_location_updatedat;");
        }
    }
}
