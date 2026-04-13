using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BudgetTracker.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "budgets",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    AllocatedAmount = table.Column<decimal>(type: "numeric(14,2)", nullable: false),
                    Currency = table.Column<string>(type: "character varying(8)", maxLength: 8, nullable: false),
                    Note = table.Column<string>(type: "text", nullable: true),
                    PeriodStart = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    PeriodEnd = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    IsArchived = table.Column<bool>(type: "boolean", nullable: false),
                    DisplayOrder = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_budgets", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "categories",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Kind = table.Column<string>(type: "character varying(16)", maxLength: 16, nullable: false),
                    Color = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: true),
                    Icon = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
                    DisplayOrder = table.Column<int>(type: "integer", nullable: false),
                    IsArchived = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_categories", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "subcategories",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CategoryId = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_subcategories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_subcategories_categories_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "categories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "budget_operations",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Kind = table.Column<string>(type: "character varying(16)", maxLength: 16, nullable: false),
                    BudgetId = table.Column<Guid>(type: "uuid", nullable: true),
                    SourceBudgetId = table.Column<Guid>(type: "uuid", nullable: true),
                    TargetBudgetId = table.Column<Guid>(type: "uuid", nullable: true),
                    Amount = table.Column<decimal>(type: "numeric(14,2)", nullable: true),
                    DebitAmount = table.Column<decimal>(type: "numeric(14,2)", nullable: true),
                    CreditAmount = table.Column<decimal>(type: "numeric(14,2)", nullable: true),
                    SubcategoryId = table.Column<Guid>(type: "uuid", nullable: true),
                    Note = table.Column<string>(type: "text", nullable: true),
                    OccurredAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_budget_operations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_budget_operations_budgets_BudgetId",
                        column: x => x.BudgetId,
                        principalTable: "budgets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_budget_operations_budgets_SourceBudgetId",
                        column: x => x.SourceBudgetId,
                        principalTable: "budgets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_budget_operations_budgets_TargetBudgetId",
                        column: x => x.TargetBudgetId,
                        principalTable: "budgets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_budget_operations_subcategories_SubcategoryId",
                        column: x => x.SubcategoryId,
                        principalTable: "subcategories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_budget_operations_BudgetId_OccurredAt",
                table: "budget_operations",
                columns: new[] { "BudgetId", "OccurredAt" });

            migrationBuilder.CreateIndex(
                name: "IX_budget_operations_Kind_OccurredAt",
                table: "budget_operations",
                columns: new[] { "Kind", "OccurredAt" });

            migrationBuilder.CreateIndex(
                name: "IX_budget_operations_OccurredAt",
                table: "budget_operations",
                column: "OccurredAt");

            migrationBuilder.CreateIndex(
                name: "IX_budget_operations_SourceBudgetId_OccurredAt",
                table: "budget_operations",
                columns: new[] { "SourceBudgetId", "OccurredAt" });

            migrationBuilder.CreateIndex(
                name: "IX_budget_operations_SubcategoryId_OccurredAt",
                table: "budget_operations",
                columns: new[] { "SubcategoryId", "OccurredAt" });

            migrationBuilder.CreateIndex(
                name: "IX_budget_operations_TargetBudgetId_OccurredAt",
                table: "budget_operations",
                columns: new[] { "TargetBudgetId", "OccurredAt" });

            migrationBuilder.CreateIndex(
                name: "IX_budgets_Currency",
                table: "budgets",
                column: "Currency");

            migrationBuilder.CreateIndex(
                name: "IX_budgets_Name_PeriodStart_PeriodEnd",
                table: "budgets",
                columns: new[] { "Name", "PeriodStart", "PeriodEnd" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_budgets_PeriodStart_PeriodEnd",
                table: "budgets",
                columns: new[] { "PeriodStart", "PeriodEnd" });

            migrationBuilder.CreateIndex(
                name: "IX_categories_Kind",
                table: "categories",
                column: "Kind");

            migrationBuilder.CreateIndex(
                name: "IX_categories_Kind_Name",
                table: "categories",
                columns: new[] { "Kind", "Name" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_subcategories_CategoryId",
                table: "subcategories",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_subcategories_CategoryId_Name",
                table: "subcategories",
                columns: new[] { "CategoryId", "Name" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "budget_operations");

            migrationBuilder.DropTable(
                name: "budgets");

            migrationBuilder.DropTable(
                name: "subcategories");

            migrationBuilder.DropTable(
                name: "categories");
        }
    }
}
