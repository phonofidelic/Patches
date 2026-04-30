using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Patches.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddConnectionEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "PatchId",
                table: "Modules",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Patch",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    Description = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Patch", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Connections",
                columns: table => new
                {
                    PatchId = table.Column<int>(type: "INTEGER", nullable: false),
                    InputId = table.Column<int>(type: "INTEGER", nullable: false),
                    OutputId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Connections", x => new { x.PatchId, x.InputId, x.OutputId });
                    table.ForeignKey(
                        name: "FK_Connections_ConnectionPoints_InputId",
                        column: x => x.InputId,
                        principalTable: "ConnectionPoints",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Connections_ConnectionPoints_OutputId",
                        column: x => x.OutputId,
                        principalTable: "ConnectionPoints",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Connections_Patch_PatchId",
                        column: x => x.PatchId,
                        principalTable: "Patch",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Modules_PatchId",
                table: "Modules",
                column: "PatchId");

            migrationBuilder.CreateIndex(
                name: "IX_Connections_InputId",
                table: "Connections",
                column: "InputId");

            migrationBuilder.CreateIndex(
                name: "IX_Connections_OutputId",
                table: "Connections",
                column: "OutputId");

            migrationBuilder.AddForeignKey(
                name: "FK_Modules_Patch_PatchId",
                table: "Modules",
                column: "PatchId",
                principalTable: "Patch",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Modules_Patch_PatchId",
                table: "Modules");

            migrationBuilder.DropTable(
                name: "Connections");

            migrationBuilder.DropTable(
                name: "Patch");

            migrationBuilder.DropIndex(
                name: "IX_Modules_PatchId",
                table: "Modules");

            migrationBuilder.DropColumn(
                name: "PatchId",
                table: "Modules");
        }
    }
}
