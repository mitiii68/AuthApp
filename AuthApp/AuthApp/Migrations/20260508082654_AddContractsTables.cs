using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AuthApp.Migrations
{
    /// <inheritdoc />
    public partial class AddContractsTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Contract_Contract_SourceContractId",
                table: "Contract");

            migrationBuilder.DropForeignKey(
                name: "FK_Contract_Counterparties_CounterpartyId",
                table: "Contract");

            migrationBuilder.DropForeignKey(
                name: "FK_Contract_Users_ResponsibleUserId",
                table: "Contract");

            migrationBuilder.DropForeignKey(
                name: "FK_ContractParticipant_Contract_ContractId",
                table: "ContractParticipant");

            migrationBuilder.DropForeignKey(
                name: "FK_ContractParticipant_Users_UserId",
                table: "ContractParticipant");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ContractParticipant",
                table: "ContractParticipant");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Contract",
                table: "Contract");

            migrationBuilder.RenameTable(
                name: "ContractParticipant",
                newName: "ContractParticipants");

            migrationBuilder.RenameTable(
                name: "Contract",
                newName: "Contracts");

            migrationBuilder.RenameIndex(
                name: "IX_ContractParticipant_UserId",
                table: "ContractParticipants",
                newName: "IX_ContractParticipants_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_ContractParticipant_ContractId_UserId",
                table: "ContractParticipants",
                newName: "IX_ContractParticipants_ContractId_UserId");

            migrationBuilder.RenameColumn(
                name: "ContractType",
                table: "Contracts",
                newName: "Type");

            migrationBuilder.RenameIndex(
                name: "IX_Contract_SourceContractId",
                table: "Contracts",
                newName: "IX_Contracts_SourceContractId");

            migrationBuilder.RenameIndex(
                name: "IX_Contract_ResponsibleUserId",
                table: "Contracts",
                newName: "IX_Contracts_ResponsibleUserId");

            migrationBuilder.RenameIndex(
                name: "IX_Contract_CounterpartyId",
                table: "Contracts",
                newName: "IX_Contracts_CounterpartyId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ContractParticipants",
                table: "ContractParticipants",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Contracts",
                table: "Contracts",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ContractParticipants_Contracts_ContractId",
                table: "ContractParticipants",
                column: "ContractId",
                principalTable: "Contracts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ContractParticipants_Users_UserId",
                table: "ContractParticipants",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Contracts_Contracts_SourceContractId",
                table: "Contracts",
                column: "SourceContractId",
                principalTable: "Contracts",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Contracts_Counterparties_CounterpartyId",
                table: "Contracts",
                column: "CounterpartyId",
                principalTable: "Counterparties",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Contracts_Users_ResponsibleUserId",
                table: "Contracts",
                column: "ResponsibleUserId",
                principalTable: "Users",
                principalColumn: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ContractParticipants_Contracts_ContractId",
                table: "ContractParticipants");

            migrationBuilder.DropForeignKey(
                name: "FK_ContractParticipants_Users_UserId",
                table: "ContractParticipants");

            migrationBuilder.DropForeignKey(
                name: "FK_Contracts_Contracts_SourceContractId",
                table: "Contracts");

            migrationBuilder.DropForeignKey(
                name: "FK_Contracts_Counterparties_CounterpartyId",
                table: "Contracts");

            migrationBuilder.DropForeignKey(
                name: "FK_Contracts_Users_ResponsibleUserId",
                table: "Contracts");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Contracts",
                table: "Contracts");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ContractParticipants",
                table: "ContractParticipants");

            migrationBuilder.RenameTable(
                name: "Contracts",
                newName: "Contract");

            migrationBuilder.RenameTable(
                name: "ContractParticipants",
                newName: "ContractParticipant");

            migrationBuilder.RenameColumn(
                name: "Type",
                table: "Contract",
                newName: "ContractType");

            migrationBuilder.RenameIndex(
                name: "IX_Contracts_SourceContractId",
                table: "Contract",
                newName: "IX_Contract_SourceContractId");

            migrationBuilder.RenameIndex(
                name: "IX_Contracts_ResponsibleUserId",
                table: "Contract",
                newName: "IX_Contract_ResponsibleUserId");

            migrationBuilder.RenameIndex(
                name: "IX_Contracts_CounterpartyId",
                table: "Contract",
                newName: "IX_Contract_CounterpartyId");

            migrationBuilder.RenameIndex(
                name: "IX_ContractParticipants_UserId",
                table: "ContractParticipant",
                newName: "IX_ContractParticipant_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_ContractParticipants_ContractId_UserId",
                table: "ContractParticipant",
                newName: "IX_ContractParticipant_ContractId_UserId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Contract",
                table: "Contract",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ContractParticipant",
                table: "ContractParticipant",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Contract_Contract_SourceContractId",
                table: "Contract",
                column: "SourceContractId",
                principalTable: "Contract",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Contract_Counterparties_CounterpartyId",
                table: "Contract",
                column: "CounterpartyId",
                principalTable: "Counterparties",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Contract_Users_ResponsibleUserId",
                table: "Contract",
                column: "ResponsibleUserId",
                principalTable: "Users",
                principalColumn: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_ContractParticipant_Contract_ContractId",
                table: "ContractParticipant",
                column: "ContractId",
                principalTable: "Contract",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ContractParticipant_Users_UserId",
                table: "ContractParticipant",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
