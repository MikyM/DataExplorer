using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BookLibrary.DataAccessLayer.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "author",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false),
                    first_name = table.Column<string>(type: "text", nullable: false),
                    surname = table.Column<string>(type: "text", nullable: false),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    updated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    is_disabled = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_author", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "client",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false),
                    first_name = table.Column<string>(type: "text", nullable: false),
                    surname = table.Column<string>(type: "text", nullable: false),
                    phone_number = table.Column<string>(type: "text", nullable: false),
                    email = table.Column<string>(type: "text", nullable: false),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    updated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    is_disabled = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_client", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "publisher",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false),
                    name = table.Column<string>(type: "text", nullable: false),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    updated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    is_disabled = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_publisher", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "book",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false),
                    title = table.Column<string>(type: "text", nullable: false),
                    published_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    author_id = table.Column<long>(type: "bigint", nullable: false),
                    publisher_id = table.Column<long>(type: "bigint", nullable: false),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    updated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    is_disabled = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_book", x => x.id);
                    table.ForeignKey(
                        name: "fk_book_author_author_id",
                        column: x => x.author_id,
                        principalTable: "author",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_book_publisher_publisher_id",
                        column: x => x.publisher_id,
                        principalTable: "publisher",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "borrowing",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false),
                    client_id = table.Column<long>(type: "bigint", nullable: false),
                    book_id = table.Column<long>(type: "bigint", nullable: false),
                    borrowed_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    returned_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    updated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    is_disabled = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_borrowing", x => x.id);
                    table.ForeignKey(
                        name: "fk_borrowing_book_book_id",
                        column: x => x.book_id,
                        principalTable: "book",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_borrowing_client_client_id",
                        column: x => x.client_id,
                        principalTable: "client",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_book_author_id",
                table: "book",
                column: "author_id");

            migrationBuilder.CreateIndex(
                name: "ix_book_publisher_id",
                table: "book",
                column: "publisher_id");

            migrationBuilder.CreateIndex(
                name: "ix_borrowing_book_id",
                table: "borrowing",
                column: "book_id");

            migrationBuilder.CreateIndex(
                name: "ix_borrowing_client_id",
                table: "borrowing",
                column: "client_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "borrowing");

            migrationBuilder.DropTable(
                name: "book");

            migrationBuilder.DropTable(
                name: "client");

            migrationBuilder.DropTable(
                name: "author");

            migrationBuilder.DropTable(
                name: "publisher");
        }
    }
}
