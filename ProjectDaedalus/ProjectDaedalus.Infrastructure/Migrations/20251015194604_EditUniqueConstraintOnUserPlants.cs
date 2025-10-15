using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProjectDaedalus.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class EditUniqueConstraintOnUserPlants : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "status",
                table: "devices",
                type: "enum('Active','Inactive','Disconnected')",
                nullable: true,
                collation: "utf8mb4_0900_ai_ci",
                oldClrType: typeof(string),
                oldType: "enum('Active','Inactive','Disconnected')")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("Relational:Collation", "utf8mb4_0900_ai_ci");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "devices",
                keyColumn: "status",
                keyValue: null,
                column: "status",
                value: "");

            migrationBuilder.AlterColumn<string>(
                name: "status",
                table: "devices",
                type: "enum('Active','Inactive','Disconnected')",
                nullable: false,
                collation: "utf8mb4_0900_ai_ci",
                oldClrType: typeof(string),
                oldType: "enum('Active','Inactive','Disconnected')",
                oldNullable: true)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("Relational:Collation", "utf8mb4_0900_ai_ci");
        }
    }
}
