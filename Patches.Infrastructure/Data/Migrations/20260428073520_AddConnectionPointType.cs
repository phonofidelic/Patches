using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Patches.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddConnectionPointType : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ConnectionPointType");

            migrationBuilder.AddColumn<int>(
                name: "TypeId",
                table: "ConnectionPoints",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "ConnectionPointTypes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ConnectionPointTypes", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ConnectionPoints_TypeId",
                table: "ConnectionPoints",
                column: "TypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_ConnectionPoints_ConnectionPointTypes_TypeId",
                table: "ConnectionPoints",
                column: "TypeId",
                principalTable: "ConnectionPointTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ConnectionPoints_ConnectionPointTypes_TypeId",
                table: "ConnectionPoints");

            migrationBuilder.DropTable(
                name: "ConnectionPointTypes");

            migrationBuilder.DropIndex(
                name: "IX_ConnectionPoints_TypeId",
                table: "ConnectionPoints");

            migrationBuilder.DropColumn(
                name: "TypeId",
                table: "ConnectionPoints");

            migrationBuilder.CreateTable(
                name: "ConnectionPointType",
                columns: table => new
                {
                    Name = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                });
        }
    }
}
