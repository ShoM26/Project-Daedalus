using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProjectDaedalus.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdatingRelationshipsAndFieldNames : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "devices",
                newName: "user_id");

            migrationBuilder.RenameColumn(
                name: "device_name",
                table: "devices",
                newName: "hardware_identifier");

            migrationBuilder.CreateIndex(
                name: "fk_device",
                table: "devices",
                column: "user_id");

            migrationBuilder.AddForeignKey(
                name: "fk_device",
                table: "devices",
                column: "user_id",
                principalTable: "users",
                principalColumn: "user_id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_device",
                table: "devices");

            migrationBuilder.DropIndex(
                name: "fk_device",
                table: "devices");

            migrationBuilder.RenameColumn(
                name: "user_id",
                table: "devices",
                newName: "UserId");

            migrationBuilder.RenameColumn(
                name: "hardware_identifier",
                table: "devices",
                newName: "device_name");
        }
    }
}
