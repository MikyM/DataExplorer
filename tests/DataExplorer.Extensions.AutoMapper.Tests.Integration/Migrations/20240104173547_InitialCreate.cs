using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace DataExplorer.EfCore.Tests.Integration.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "test_entity",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false, name: "id")
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "text", nullable: false),
                    description = table.Column<string>(type: "text", nullable: true, name: "description"),
                    version = table.Column<int>(type: "integer", nullable: false, name: "version"),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true, name: "created_at"),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true, name: "updated_at"),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_test_entity", x => x.id);
                });
            
            migrationBuilder.InsertData(
                table: "test_entity",
                columns: new[] { "id", "name", "description", "version", "created_at", "updated_at" },
                values: new object[,]
                {
                    { 1, "test1", "test1", 1, new DateTime(2025, 5, 5, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2025, 5, 5, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 2, "test2", "test2", 1, new DateTime(2025, 5, 5, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2025, 5, 5, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 3, "test3", "test3", 1, new DateTime(2025, 5, 5, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2025, 5, 5, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 4, "test4", "test4", 1, new DateTime(2025, 5, 5, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2025, 5, 5, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 5, "test5", "test5", 1, new DateTime(2025, 5, 5, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2025, 5, 5, 0, 0, 0, 0, DateTimeKind.Utc) }
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "test_entity");
        }
    }
}
