using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AuthApp.Migrations
{
    /// <inheritdoc />
    public partial class AddTrashToFileDocuments : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                table: "FileDocuments",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "FileDocuments",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "FileDocuments");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "FileDocuments");
        }
    }
}
