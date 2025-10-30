using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProjectDaedalus.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RemoveUsernamePasswordUniqueConstraint : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "username",
                table: "users");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "username",
                table: "users",
                columns: new[] { "username", "password" },
                unique: true);
        }
    }
}
