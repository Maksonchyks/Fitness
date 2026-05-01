using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FitnessApp.Nutrition.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DailyTargets",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    Goal = table.Column<int>(type: "integer", nullable: false),
                    TargetCalories = table.Column<float>(type: "real", precision: 8, scale: 2, nullable: false),
                    TargetProteins = table.Column<float>(type: "real", precision: 8, scale: 2, nullable: false),
                    TargetFats = table.Column<float>(type: "real", precision: 8, scale: 2, nullable: false),
                    TargetCarbs = table.Column<float>(type: "real", precision: 8, scale: 2, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DailyTargets", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MealLogs",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    MealName = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    MealType = table.Column<int>(type: "integer", nullable: false),
                    Calories = table.Column<float>(type: "real", precision: 8, scale: 2, nullable: false),
                    Proteins = table.Column<float>(type: "real", precision: 8, scale: 2, nullable: false),
                    Fats = table.Column<float>(type: "real", precision: 8, scale: 2, nullable: false),
                    Carbs = table.Column<float>(type: "real", precision: 8, scale: 2, nullable: false),
                    LoggedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MealLogs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MealPlanTemplates",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    TotalCalories = table.Column<float>(type: "real", precision: 8, scale: 2, nullable: false),
                    TotalProteins = table.Column<float>(type: "real", precision: 8, scale: 2, nullable: false),
                    TotalFats = table.Column<float>(type: "real", precision: 8, scale: 2, nullable: false),
                    TotalCarbs = table.Column<float>(type: "real", precision: 8, scale: 2, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MealPlanTemplates", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "UserTemplateQueues",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    ViewedTemplateIds = table.Column<IReadOnlyCollection<Guid>>(type: "jsonb", nullable: false),
                    LastResetAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserTemplateQueues", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "WeightLogs",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    Weight = table.Column<float>(type: "real", precision: 5, scale: 2, nullable: false),
                    LoggedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WeightLogs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TemplateMeals",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TemplateId = table.Column<Guid>(type: "uuid", nullable: false),
                    MealType = table.Column<int>(type: "integer", nullable: false),
                    DishName = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    Calories = table.Column<float>(type: "real", precision: 8, scale: 2, nullable: false),
                    Proteins = table.Column<float>(type: "real", precision: 8, scale: 2, nullable: false),
                    Fats = table.Column<float>(type: "real", precision: 8, scale: 2, nullable: false),
                    Carbs = table.Column<float>(type: "real", precision: 8, scale: 2, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TemplateMeals", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TemplateMeals_MealPlanTemplates_TemplateId",
                        column: x => x.TemplateId,
                        principalTable: "MealPlanTemplates",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DailyTargets_UserId",
                table: "DailyTargets",
                column: "UserId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_MealLogs_UserId_LoggedAt",
                table: "MealLogs",
                columns: new[] { "UserId", "LoggedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_TemplateMeals_TemplateId",
                table: "TemplateMeals",
                column: "TemplateId");

            migrationBuilder.CreateIndex(
                name: "IX_UserTemplateQueues_UserId",
                table: "UserTemplateQueues",
                column: "UserId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_WeightLogs_UserId_LoggedAt",
                table: "WeightLogs",
                columns: new[] { "UserId", "LoggedAt" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DailyTargets");

            migrationBuilder.DropTable(
                name: "MealLogs");

            migrationBuilder.DropTable(
                name: "TemplateMeals");

            migrationBuilder.DropTable(
                name: "UserTemplateQueues");

            migrationBuilder.DropTable(
                name: "WeightLogs");

            migrationBuilder.DropTable(
                name: "MealPlanTemplates");
        }
    }
}
