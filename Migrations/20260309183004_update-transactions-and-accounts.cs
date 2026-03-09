using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace banking_api.Migrations
{
    /// <inheritdoc />
    public partial class updatetransactionsandaccounts : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Transactions_BankAccounts_AccountId",
                table: "Transactions");

            migrationBuilder.AddColumn<decimal>(
                name: "Fee",
                table: "Transactions",
                type: "numeric",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "Transactions",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<Guid>(
                name: "TargetAccountId",
                table: "Transactions",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "ReservedBalance",
                table: "BankAccounts",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.CreateIndex(
                name: "IX_Transactions_TargetAccountId",
                table: "Transactions",
                column: "TargetAccountId");

            migrationBuilder.AddForeignKey(
                name: "FK_Transactions_BankAccounts_AccountId",
                table: "Transactions",
                column: "AccountId",
                principalTable: "BankAccounts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Transactions_BankAccounts_TargetAccountId",
                table: "Transactions",
                column: "TargetAccountId",
                principalTable: "BankAccounts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Transactions_BankAccounts_AccountId",
                table: "Transactions");

            migrationBuilder.DropForeignKey(
                name: "FK_Transactions_BankAccounts_TargetAccountId",
                table: "Transactions");

            migrationBuilder.DropIndex(
                name: "IX_Transactions_TargetAccountId",
                table: "Transactions");

            migrationBuilder.DropColumn(
                name: "Fee",
                table: "Transactions");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "Transactions");

            migrationBuilder.DropColumn(
                name: "TargetAccountId",
                table: "Transactions");

            migrationBuilder.DropColumn(
                name: "ReservedBalance",
                table: "BankAccounts");

            migrationBuilder.AddForeignKey(
                name: "FK_Transactions_BankAccounts_AccountId",
                table: "Transactions",
                column: "AccountId",
                principalTable: "BankAccounts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
