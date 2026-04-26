using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Patches.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddModuleToConnectionPoint : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ConnectionPoints_Modules_ModuleId",
                table: "ConnectionPoints");

            migrationBuilder.AlterColumn<Guid>(
                name: "ModuleId",
                table: "ConnectionPoints",
                type: "TEXT",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_ConnectionPoints_Modules_ModuleId",
                table: "ConnectionPoints",
                column: "ModuleId",
                principalTable: "Modules",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ConnectionPoints_Modules_ModuleId",
                table: "ConnectionPoints");

            migrationBuilder.AlterColumn<Guid>(
                name: "ModuleId",
                table: "ConnectionPoints",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "TEXT");

            migrationBuilder.AddForeignKey(
                name: "FK_ConnectionPoints_Modules_ModuleId",
                table: "ConnectionPoints",
                column: "ModuleId",
                principalTable: "Modules",
                principalColumn: "Id");
        }
    }
}
