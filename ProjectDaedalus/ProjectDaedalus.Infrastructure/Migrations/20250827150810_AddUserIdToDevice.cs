using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProjectDaedalus.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddUserIdToDevice : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "UserId",
                table: "devices",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UserId",
                table: "devices");
        }
    }
}
