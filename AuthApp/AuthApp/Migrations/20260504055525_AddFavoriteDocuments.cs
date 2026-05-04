using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AuthApp.Migrations
{
    
    public partial class AddFavoriteDocuments : Migration
    {
        
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "FavoriteDocuments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    UserEmail = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    FileDocumentsId = table.Column<int>(type: "int", nullable: false),
                    AddedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    FavoriteDocumentId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FavoriteDocuments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FavoriteDocuments_FavoriteDocuments_FavoriteDocumentId",
                        column: x => x.FavoriteDocumentId,
                        principalTable: "FavoriteDocuments",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_FavoriteDocuments_FileDocuments_FileDocumentsId",
                        column: x => x.FileDocumentsId,
                        principalTable: "FileDocuments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_FavoriteDocuments_FavoriteDocumentId",
                table: "FavoriteDocuments",
                column: "FavoriteDocumentId");

            migrationBuilder.CreateIndex(
                name: "IX_FavoriteDocuments_FileDocumentsId",
                table: "FavoriteDocuments",
                column: "FileDocumentsId");
        }

        
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FavoriteDocuments");
        }
    }
}
