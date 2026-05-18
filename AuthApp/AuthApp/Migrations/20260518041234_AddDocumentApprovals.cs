using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AuthApp.Migrations
{
    /// <inheritdoc />
    public partial class AddDocumentApprovals : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ContractDocument_FileDocuments_FileDocumentId",
                table: "ContractDocument");

            migrationBuilder.DropIndex(
                name: "IX_ContractDocument_FileDocumentId",
                table: "ContractDocument");

            migrationBuilder.AlterColumn<int>(
                name: "FileDocumentId",
                table: "ContractDocument",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<int>(
                name: "ApprovalStatus",
                table: "ContractDocument",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "ApprovedAt",
                table: "ContractDocument",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "ContractDocument",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "FilePath",
                table: "ContractDocument",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "FullName",
                table: "ContractDocument",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "DocumentApprovals",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    ContractDocumentId = table.Column<int>(type: "int", nullable: false),
                    ContractParticipantId = table.Column<int>(type: "int", nullable: false),
                    OrderIndex = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    ViewedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    DecidedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    Comment = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DocumentApprovals", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DocumentApprovals_ContractDocument_ContractDocumentId",
                        column: x => x.ContractDocumentId,
                        principalTable: "ContractDocument",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DocumentApprovals_ContractParticipants_ContractParticipantId",
                        column: x => x.ContractParticipantId,
                        principalTable: "ContractParticipants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_DocumentApprovals_ContractDocumentId",
                table: "DocumentApprovals",
                column: "ContractDocumentId");

            migrationBuilder.CreateIndex(
                name: "IX_DocumentApprovals_ContractParticipantId",
                table: "DocumentApprovals",
                column: "ContractParticipantId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DocumentApprovals");

            migrationBuilder.DropColumn(
                name: "ApprovalStatus",
                table: "ContractDocument");

            migrationBuilder.DropColumn(
                name: "ApprovedAt",
                table: "ContractDocument");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "ContractDocument");

            migrationBuilder.DropColumn(
                name: "FilePath",
                table: "ContractDocument");

            migrationBuilder.DropColumn(
                name: "FullName",
                table: "ContractDocument");

            migrationBuilder.AlterColumn<int>(
                name: "FileDocumentId",
                table: "ContractDocument",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ContractDocument_FileDocumentId",
                table: "ContractDocument",
                column: "FileDocumentId");

            migrationBuilder.AddForeignKey(
                name: "FK_ContractDocument_FileDocuments_FileDocumentId",
                table: "ContractDocument",
                column: "FileDocumentId",
                principalTable: "FileDocuments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
