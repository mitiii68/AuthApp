using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AuthApp.Migrations
{
    /// <inheritdoc />
    public partial class AddContracts : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Contract",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    FullName = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ShortName = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CounterpartyId = table.Column<int>(type: "int", nullable: true),
                    Stage = table.Column<int>(type: "int", nullable: true),
                    ContractType = table.Column<int>(type: "int", nullable: true),
                    ResponsibleFromCustomer = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ResponsibleUserId = table.Column<int>(type: "int", nullable: true),
                    AmountWithVat = table.Column<decimal>(type: "decimal(65,30)", nullable: true),
                    ConclusionDate = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    ClosingDate = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    ExecutionStartDate = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    ContractNumber = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    SourceContractId = table.Column<int>(type: "int", nullable: true),
                    ProjectId = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Contract", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Contract_Contract_SourceContractId",
                        column: x => x.SourceContractId,
                        principalTable: "Contract",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Contract_Counterparties_CounterpartyId",
                        column: x => x.CounterpartyId,
                        principalTable: "Counterparties",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Contract_Users_ResponsibleUserId",
                        column: x => x.ResponsibleUserId,
                        principalTable: "Users",
                        principalColumn: "UserId");
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "ContractParticipant",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    ContractId = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ContractParticipant", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ContractParticipant_Contract_ContractId",
                        column: x => x.ContractId,
                        principalTable: "Contract",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ContractParticipant_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_Contract_CounterpartyId",
                table: "Contract",
                column: "CounterpartyId");

            migrationBuilder.CreateIndex(
                name: "IX_Contract_ResponsibleUserId",
                table: "Contract",
                column: "ResponsibleUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Contract_SourceContractId",
                table: "Contract",
                column: "SourceContractId");

            migrationBuilder.CreateIndex(
                name: "IX_ContractParticipant_ContractId_UserId",
                table: "ContractParticipant",
                columns: new[] { "ContractId", "UserId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ContractParticipant_UserId",
                table: "ContractParticipant",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ContractParticipant");

            migrationBuilder.DropTable(
                name: "Contract");
        }
    }
}
