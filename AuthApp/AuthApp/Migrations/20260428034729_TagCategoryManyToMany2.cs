using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AuthApp.Migrations
{
    /// <inheritdoc />
    public partial class TagCategoryManyToMany2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Tags_TagCategories_TagCategoryId",
                table: "Tags");

            migrationBuilder.DropIndex(
                name: "IX_Tags_TagCategoryId",
                table: "Tags");

            migrationBuilder.DropColumn(
                name: "TagCategoryId",
                table: "Tags");

            migrationBuilder.CreateTable(
                name: "TagCategoryTags",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    TagId = table.Column<int>(type: "int", nullable: false),
                    TagCategoryId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TagCategoryTags", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TagCategoryTags_TagCategories_TagCategoryId",
                        column: x => x.TagCategoryId,
                        principalTable: "TagCategories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TagCategoryTags_Tags_TagId",
                        column: x => x.TagId,
                        principalTable: "Tags",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_TagCategoryTags_TagCategoryId",
                table: "TagCategoryTags",
                column: "TagCategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_TagCategoryTags_TagId_TagCategoryId",
                table: "TagCategoryTags",
                columns: new[] { "TagId", "TagCategoryId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TagCategoryTags");

            migrationBuilder.AddColumn<int>(
                name: "TagCategoryId",
                table: "Tags",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Tags_TagCategoryId",
                table: "Tags",
                column: "TagCategoryId");

            migrationBuilder.AddForeignKey(
                name: "FK_Tags_TagCategories_TagCategoryId",
                table: "Tags",
                column: "TagCategoryId",
                principalTable: "TagCategories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
