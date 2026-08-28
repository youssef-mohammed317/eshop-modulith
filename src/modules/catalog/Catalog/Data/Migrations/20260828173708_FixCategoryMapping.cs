using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Catalog.Data.Migrations
{
    /// <inheritdoc />
    public partial class FixCategoryMapping : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<List<string>>(
                name: "Category",
                schema: "catalog",
                table: "Products",
                type: "character varying(100)[]",
                nullable: false,
                oldClrType: typeof(string[]),
                oldType: "text[]");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string[]>(
                name: "Category",
                schema: "catalog",
                table: "Products",
                type: "text[]",
                nullable: false,
                oldClrType: typeof(List<string>),
                oldType: "character varying(100)[]");
        }
    }
}
