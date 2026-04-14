using System;
using BudgetTracker.Domain.Enums;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BudgetTracker.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class RemoveBudgetPeriodColumns : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_budget_operations_budgets_BudgetId",
                table: "budget_operations");

            migrationBuilder.DropForeignKey(
                name: "FK_budget_operations_budgets_SourceBudgetId",
                table: "budget_operations");

            migrationBuilder.DropForeignKey(
                name: "FK_budget_operations_budgets_TargetBudgetId",
                table: "budget_operations");

            migrationBuilder.DropForeignKey(
                name: "FK_budget_operations_subcategories_SubcategoryId",
                table: "budget_operations");

            migrationBuilder.DropForeignKey(
                name: "FK_subcategories_categories_CategoryId",
                table: "subcategories");

            migrationBuilder.DropPrimaryKey(
                name: "PK_subcategories",
                table: "subcategories");

            migrationBuilder.DropPrimaryKey(
                name: "PK_categories",
                table: "categories");

            migrationBuilder.DropPrimaryKey(
                name: "PK_budgets",
                table: "budgets");

            migrationBuilder.DropIndex(
                name: "IX_budgets_Name_PeriodStart_PeriodEnd",
                table: "budgets");

            migrationBuilder.DropIndex(
                name: "IX_budgets_PeriodStart_PeriodEnd",
                table: "budgets");

            migrationBuilder.DropPrimaryKey(
                name: "PK_budget_operations",
                table: "budget_operations");

            migrationBuilder.DropColumn(
                name: "PeriodEnd",
                table: "budgets");

            migrationBuilder.DropColumn(
                name: "PeriodStart",
                table: "budgets");

            migrationBuilder.RenameColumn(
                name: "Name",
                table: "subcategories",
                newName: "name");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "subcategories",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "UpdatedAt",
                table: "subcategories",
                newName: "updated_at");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                table: "subcategories",
                newName: "created_at");

            migrationBuilder.RenameColumn(
                name: "CategoryId",
                table: "subcategories",
                newName: "category_id");

            migrationBuilder.RenameIndex(
                name: "IX_subcategories_CategoryId_Name",
                table: "subcategories",
                newName: "ix_subcategories_category_id_name");

            migrationBuilder.RenameIndex(
                name: "IX_subcategories_CategoryId",
                table: "subcategories",
                newName: "ix_subcategories_category_id");

            migrationBuilder.RenameColumn(
                name: "Name",
                table: "categories",
                newName: "name");

            migrationBuilder.RenameColumn(
                name: "Kind",
                table: "categories",
                newName: "kind");

            migrationBuilder.RenameColumn(
                name: "Icon",
                table: "categories",
                newName: "icon");

            migrationBuilder.RenameColumn(
                name: "Color",
                table: "categories",
                newName: "color");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "categories",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "UpdatedAt",
                table: "categories",
                newName: "updated_at");

            migrationBuilder.RenameColumn(
                name: "IsArchived",
                table: "categories",
                newName: "is_archived");

            migrationBuilder.RenameColumn(
                name: "DisplayOrder",
                table: "categories",
                newName: "display_order");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                table: "categories",
                newName: "created_at");

            migrationBuilder.RenameIndex(
                name: "IX_categories_Kind_Name",
                table: "categories",
                newName: "ix_categories_kind_name");

            migrationBuilder.RenameIndex(
                name: "IX_categories_Kind",
                table: "categories",
                newName: "ix_categories_kind");

            migrationBuilder.RenameColumn(
                name: "Note",
                table: "budgets",
                newName: "note");

            migrationBuilder.RenameColumn(
                name: "Name",
                table: "budgets",
                newName: "name");

            migrationBuilder.RenameColumn(
                name: "Currency",
                table: "budgets",
                newName: "currency");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "budgets",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "UpdatedAt",
                table: "budgets",
                newName: "updated_at");

            migrationBuilder.RenameColumn(
                name: "IsArchived",
                table: "budgets",
                newName: "is_archived");

            migrationBuilder.RenameColumn(
                name: "DisplayOrder",
                table: "budgets",
                newName: "display_order");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                table: "budgets",
                newName: "created_at");

            migrationBuilder.RenameColumn(
                name: "AllocatedAmount",
                table: "budgets",
                newName: "allocated_amount");

            migrationBuilder.RenameIndex(
                name: "IX_budgets_Currency",
                table: "budgets",
                newName: "ix_budgets_currency");

            migrationBuilder.RenameColumn(
                name: "Note",
                table: "budget_operations",
                newName: "note");

            migrationBuilder.RenameColumn(
                name: "Kind",
                table: "budget_operations",
                newName: "kind");

            migrationBuilder.RenameColumn(
                name: "Amount",
                table: "budget_operations",
                newName: "amount");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "budget_operations",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "UpdatedAt",
                table: "budget_operations",
                newName: "updated_at");

            migrationBuilder.RenameColumn(
                name: "TargetBudgetId",
                table: "budget_operations",
                newName: "target_budget_id");

            migrationBuilder.RenameColumn(
                name: "SubcategoryId",
                table: "budget_operations",
                newName: "subcategory_id");

            migrationBuilder.RenameColumn(
                name: "SourceBudgetId",
                table: "budget_operations",
                newName: "source_budget_id");

            migrationBuilder.RenameColumn(
                name: "OccurredAt",
                table: "budget_operations",
                newName: "occurred_at");

            migrationBuilder.RenameColumn(
                name: "DebitAmount",
                table: "budget_operations",
                newName: "debit_amount");

            migrationBuilder.RenameColumn(
                name: "CreditAmount",
                table: "budget_operations",
                newName: "credit_amount");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                table: "budget_operations",
                newName: "created_at");

            migrationBuilder.RenameColumn(
                name: "BudgetId",
                table: "budget_operations",
                newName: "budget_id");

            migrationBuilder.RenameIndex(
                name: "IX_budget_operations_TargetBudgetId_OccurredAt",
                table: "budget_operations",
                newName: "ix_budget_operations_target_budget_id_occurred_at");

            migrationBuilder.RenameIndex(
                name: "IX_budget_operations_SubcategoryId_OccurredAt",
                table: "budget_operations",
                newName: "ix_budget_operations_subcategory_id_occurred_at");

            migrationBuilder.RenameIndex(
                name: "IX_budget_operations_SourceBudgetId_OccurredAt",
                table: "budget_operations",
                newName: "ix_budget_operations_source_budget_id_occurred_at");

            migrationBuilder.RenameIndex(
                name: "IX_budget_operations_OccurredAt",
                table: "budget_operations",
                newName: "ix_budget_operations_occurred_at");

            migrationBuilder.RenameIndex(
                name: "IX_budget_operations_Kind_OccurredAt",
                table: "budget_operations",
                newName: "ix_budget_operations_kind_occurred_at");

            migrationBuilder.RenameIndex(
                name: "IX_budget_operations_BudgetId_OccurredAt",
                table: "budget_operations",
                newName: "ix_budget_operations_budget_id_occurred_at");

            migrationBuilder.AlterColumn<CategoryKind>(
                name: "kind",
                table: "categories",
                type: "category_kind",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(16)",
                oldMaxLength: 16);

            migrationBuilder.AlterColumn<CurrencyCode>(
                name: "currency",
                table: "budgets",
                type: "currency_code",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(8)",
                oldMaxLength: 8);

            migrationBuilder.AlterColumn<OperationKind>(
                name: "kind",
                table: "budget_operations",
                type: "operation_kind",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(16)",
                oldMaxLength: 16);

            migrationBuilder.AddPrimaryKey(
                name: "pk_subcategories",
                table: "subcategories",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "pk_categories",
                table: "categories",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "pk_budgets",
                table: "budgets",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "pk_budget_operations",
                table: "budget_operations",
                column: "id");

            migrationBuilder.CreateIndex(
                name: "ix_budgets_name",
                table: "budgets",
                column: "name",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "fk_budget_operations_budgets_budget_id",
                table: "budget_operations",
                column: "budget_id",
                principalTable: "budgets",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_budget_operations_budgets_source_budget_id",
                table: "budget_operations",
                column: "source_budget_id",
                principalTable: "budgets",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_budget_operations_budgets_target_budget_id",
                table: "budget_operations",
                column: "target_budget_id",
                principalTable: "budgets",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_budget_operations_subcategories_subcategory_id",
                table: "budget_operations",
                column: "subcategory_id",
                principalTable: "subcategories",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_subcategories_categories_category_id",
                table: "subcategories",
                column: "category_id",
                principalTable: "categories",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_budget_operations_budgets_budget_id",
                table: "budget_operations");

            migrationBuilder.DropForeignKey(
                name: "fk_budget_operations_budgets_source_budget_id",
                table: "budget_operations");

            migrationBuilder.DropForeignKey(
                name: "fk_budget_operations_budgets_target_budget_id",
                table: "budget_operations");

            migrationBuilder.DropForeignKey(
                name: "fk_budget_operations_subcategories_subcategory_id",
                table: "budget_operations");

            migrationBuilder.DropForeignKey(
                name: "fk_subcategories_categories_category_id",
                table: "subcategories");

            migrationBuilder.DropPrimaryKey(
                name: "pk_subcategories",
                table: "subcategories");

            migrationBuilder.DropPrimaryKey(
                name: "pk_categories",
                table: "categories");

            migrationBuilder.DropPrimaryKey(
                name: "pk_budgets",
                table: "budgets");

            migrationBuilder.DropIndex(
                name: "ix_budgets_name",
                table: "budgets");

            migrationBuilder.DropPrimaryKey(
                name: "pk_budget_operations",
                table: "budget_operations");

            migrationBuilder.RenameColumn(
                name: "name",
                table: "subcategories",
                newName: "Name");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "subcategories",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "updated_at",
                table: "subcategories",
                newName: "UpdatedAt");

            migrationBuilder.RenameColumn(
                name: "created_at",
                table: "subcategories",
                newName: "CreatedAt");

            migrationBuilder.RenameColumn(
                name: "category_id",
                table: "subcategories",
                newName: "CategoryId");

            migrationBuilder.RenameIndex(
                name: "ix_subcategories_category_id_name",
                table: "subcategories",
                newName: "IX_subcategories_CategoryId_Name");

            migrationBuilder.RenameIndex(
                name: "ix_subcategories_category_id",
                table: "subcategories",
                newName: "IX_subcategories_CategoryId");

            migrationBuilder.RenameColumn(
                name: "name",
                table: "categories",
                newName: "Name");

            migrationBuilder.RenameColumn(
                name: "kind",
                table: "categories",
                newName: "Kind");

            migrationBuilder.RenameColumn(
                name: "icon",
                table: "categories",
                newName: "Icon");

            migrationBuilder.RenameColumn(
                name: "color",
                table: "categories",
                newName: "Color");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "categories",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "updated_at",
                table: "categories",
                newName: "UpdatedAt");

            migrationBuilder.RenameColumn(
                name: "is_archived",
                table: "categories",
                newName: "IsArchived");

            migrationBuilder.RenameColumn(
                name: "display_order",
                table: "categories",
                newName: "DisplayOrder");

            migrationBuilder.RenameColumn(
                name: "created_at",
                table: "categories",
                newName: "CreatedAt");

            migrationBuilder.RenameIndex(
                name: "ix_categories_kind_name",
                table: "categories",
                newName: "IX_categories_Kind_Name");

            migrationBuilder.RenameIndex(
                name: "ix_categories_kind",
                table: "categories",
                newName: "IX_categories_Kind");

            migrationBuilder.RenameColumn(
                name: "note",
                table: "budgets",
                newName: "Note");

            migrationBuilder.RenameColumn(
                name: "name",
                table: "budgets",
                newName: "Name");

            migrationBuilder.RenameColumn(
                name: "currency",
                table: "budgets",
                newName: "Currency");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "budgets",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "updated_at",
                table: "budgets",
                newName: "UpdatedAt");

            migrationBuilder.RenameColumn(
                name: "is_archived",
                table: "budgets",
                newName: "IsArchived");

            migrationBuilder.RenameColumn(
                name: "display_order",
                table: "budgets",
                newName: "DisplayOrder");

            migrationBuilder.RenameColumn(
                name: "created_at",
                table: "budgets",
                newName: "CreatedAt");

            migrationBuilder.RenameColumn(
                name: "allocated_amount",
                table: "budgets",
                newName: "AllocatedAmount");

            migrationBuilder.RenameIndex(
                name: "ix_budgets_currency",
                table: "budgets",
                newName: "IX_budgets_Currency");

            migrationBuilder.RenameColumn(
                name: "note",
                table: "budget_operations",
                newName: "Note");

            migrationBuilder.RenameColumn(
                name: "kind",
                table: "budget_operations",
                newName: "Kind");

            migrationBuilder.RenameColumn(
                name: "amount",
                table: "budget_operations",
                newName: "Amount");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "budget_operations",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "updated_at",
                table: "budget_operations",
                newName: "UpdatedAt");

            migrationBuilder.RenameColumn(
                name: "target_budget_id",
                table: "budget_operations",
                newName: "TargetBudgetId");

            migrationBuilder.RenameColumn(
                name: "subcategory_id",
                table: "budget_operations",
                newName: "SubcategoryId");

            migrationBuilder.RenameColumn(
                name: "source_budget_id",
                table: "budget_operations",
                newName: "SourceBudgetId");

            migrationBuilder.RenameColumn(
                name: "occurred_at",
                table: "budget_operations",
                newName: "OccurredAt");

            migrationBuilder.RenameColumn(
                name: "debit_amount",
                table: "budget_operations",
                newName: "DebitAmount");

            migrationBuilder.RenameColumn(
                name: "credit_amount",
                table: "budget_operations",
                newName: "CreditAmount");

            migrationBuilder.RenameColumn(
                name: "created_at",
                table: "budget_operations",
                newName: "CreatedAt");

            migrationBuilder.RenameColumn(
                name: "budget_id",
                table: "budget_operations",
                newName: "BudgetId");

            migrationBuilder.RenameIndex(
                name: "ix_budget_operations_target_budget_id_occurred_at",
                table: "budget_operations",
                newName: "IX_budget_operations_TargetBudgetId_OccurredAt");

            migrationBuilder.RenameIndex(
                name: "ix_budget_operations_subcategory_id_occurred_at",
                table: "budget_operations",
                newName: "IX_budget_operations_SubcategoryId_OccurredAt");

            migrationBuilder.RenameIndex(
                name: "ix_budget_operations_source_budget_id_occurred_at",
                table: "budget_operations",
                newName: "IX_budget_operations_SourceBudgetId_OccurredAt");

            migrationBuilder.RenameIndex(
                name: "ix_budget_operations_occurred_at",
                table: "budget_operations",
                newName: "IX_budget_operations_OccurredAt");

            migrationBuilder.RenameIndex(
                name: "ix_budget_operations_kind_occurred_at",
                table: "budget_operations",
                newName: "IX_budget_operations_Kind_OccurredAt");

            migrationBuilder.RenameIndex(
                name: "ix_budget_operations_budget_id_occurred_at",
                table: "budget_operations",
                newName: "IX_budget_operations_BudgetId_OccurredAt");

            migrationBuilder.AlterColumn<string>(
                name: "Kind",
                table: "categories",
                type: "character varying(16)",
                maxLength: 16,
                nullable: false,
                oldClrType: typeof(CategoryKind),
                oldType: "category_kind");

            migrationBuilder.AlterColumn<string>(
                name: "Currency",
                table: "budgets",
                type: "character varying(8)",
                maxLength: 8,
                nullable: false,
                oldClrType: typeof(CurrencyCode),
                oldType: "currency_code");

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "PeriodEnd",
                table: "budgets",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "PeriodStart",
                table: "budgets",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.AlterColumn<string>(
                name: "Kind",
                table: "budget_operations",
                type: "character varying(16)",
                maxLength: 16,
                nullable: false,
                oldClrType: typeof(OperationKind),
                oldType: "operation_kind");

            migrationBuilder.AddPrimaryKey(
                name: "PK_subcategories",
                table: "subcategories",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_categories",
                table: "categories",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_budgets",
                table: "budgets",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_budget_operations",
                table: "budget_operations",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_budgets_Name_PeriodStart_PeriodEnd",
                table: "budgets",
                columns: new[] { "Name", "PeriodStart", "PeriodEnd" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_budgets_PeriodStart_PeriodEnd",
                table: "budgets",
                columns: new[] { "PeriodStart", "PeriodEnd" });

            migrationBuilder.AddForeignKey(
                name: "FK_budget_operations_budgets_BudgetId",
                table: "budget_operations",
                column: "BudgetId",
                principalTable: "budgets",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_budget_operations_budgets_SourceBudgetId",
                table: "budget_operations",
                column: "SourceBudgetId",
                principalTable: "budgets",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_budget_operations_budgets_TargetBudgetId",
                table: "budget_operations",
                column: "TargetBudgetId",
                principalTable: "budgets",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_budget_operations_subcategories_SubcategoryId",
                table: "budget_operations",
                column: "SubcategoryId",
                principalTable: "subcategories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_subcategories_categories_CategoryId",
                table: "subcategories",
                column: "CategoryId",
                principalTable: "categories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
