using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProjectDaedalus.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddNotificationSystem : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "notification",
                columns: table => new
                {
                    notification_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    userplant_id = table.Column<int>(type: "int", nullable: false),
                    message = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: false, collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    notification_type = table.Column<int>(type: "int", nullable: false),
                    is_read = table.Column<bool>(type: "tinyint(1)", nullable: false, defaultValue: false),
                    created_at = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_notification", x => x.notification_id);
                    table.ForeignKey(
                        name: "FK_notification_user_plants_userplant_id",
                        column: x => x.userplant_id,
                        principalTable: "user_plants",
                        principalColumn: "user_plant_id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4")
                .Annotation("Relational:Collation", "utf8mb4_0900_ai_ci");

            migrationBuilder.CreateTable(
                name: "notification_history",
                columns: table => new
                {
                    notification_history_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    user_plant_id = table.Column<int>(type: "int", nullable: false),
                    notification_type = table.Column<int>(type: "int", nullable: false),
                    moisture_value = table.Column<decimal>(type: "decimal(5,2)", precision: 5, scale: 2, nullable: false),
                    threshold_value = table.Column<decimal>(type: "decimal(5,2)", precision: 5, scale: 2, nullable: false),
                    created_at = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_notification_history", x => x.notification_history_id);
                    table.ForeignKey(
                        name: "FK_notification_history_user_plants_user_plant_id",
                        column: x => x.user_plant_id,
                        principalTable: "user_plants",
                        principalColumn: "user_plant_id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4")
                .Annotation("Relational:Collation", "utf8mb4_0900_ai_ci");

            migrationBuilder.CreateIndex(
                name: "idx_notifications_created",
                table: "notification",
                column: "created_at");

            migrationBuilder.CreateIndex(
                name: "idx_notifications_userplant_read_created",
                table: "notification",
                columns: new[] { "userplant_id", "is_read", "created_at" });

            migrationBuilder.CreateIndex(
                name: "idx_notification_history_dedup",
                table: "notification_history",
                columns: new[] { "user_plant_id", "notification_type" });

            migrationBuilder.CreateIndex(
                name: "idx_notification_history_userplant_type_created",
                table: "notification_history",
                columns: new[] { "user_plant_id", "notification_type", "created_at" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "notification");

            migrationBuilder.DropTable(
                name: "notification_history");
        }
    }
}
