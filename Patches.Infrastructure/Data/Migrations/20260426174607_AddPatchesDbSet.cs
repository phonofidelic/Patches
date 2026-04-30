using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Patches.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddPatchesDbSet : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Connections_Patch_PatchId",
                table: "Connections");

            migrationBuilder.DropForeignKey(
                name: "FK_Modules_Patch_PatchId",
                table: "Modules");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Patch",
                table: "Patch");

            migrationBuilder.RenameTable(
                name: "Patch",
                newName: "Patches");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Patches",
                table: "Patches",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Connections_Patches_PatchId",
                table: "Connections",
                column: "PatchId",
                principalTable: "Patches",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Modules_Patches_PatchId",
                table: "Modules",
                column: "PatchId",
                principalTable: "Patches",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Connections_Patches_PatchId",
                table: "Connections");

            migrationBuilder.DropForeignKey(
                name: "FK_Modules_Patches_PatchId",
                table: "Modules");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Patches",
                table: "Patches");

            migrationBuilder.RenameTable(
                name: "Patches",
                newName: "Patch");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Patch",
                table: "Patch",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Connections_Patch_PatchId",
                table: "Connections",
                column: "PatchId",
                principalTable: "Patch",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Modules_Patch_PatchId",
                table: "Modules",
                column: "PatchId",
                principalTable: "Patch",
                principalColumn: "Id");
        }
    }
}
