﻿using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace API.Infra.EntityFramework.Migrations
{
    /// <inheritdoc />
    public partial class BasketEntityUpdated : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ClientSecret",
                table: "Baskets",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PaymentIntentId",
                table: "Baskets",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ClientSecret",
                table: "Baskets");

            migrationBuilder.DropColumn(
                name: "PaymentIntentId",
                table: "Baskets");
        }
    }
}
