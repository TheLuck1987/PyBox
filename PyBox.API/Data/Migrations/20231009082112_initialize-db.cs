using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PyBox.API.Data.Migrations
{
    /// <inheritdoc />
    public partial class initializedb : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Scripts",
                columns: table => new
                {
                    ScriptId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Title = table.Column<string>(type: "varchar(50)", nullable: false),
                    Description = table.Column<string>(type: "varchar(255)", nullable: true),
                    ScriptText = table.Column<string>(type: "text", nullable: true),
                    Enabled = table.Column<bool>(type: "INTEGER", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    DeletedAt = table.Column<DateTime>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Scripts", x => x.ScriptId);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Scripts");
        }
    }
}
