using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GoLondonAPI.Migrations
{
    public partial class UserRefreshTokens : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "UserRefreshTokens",
                columns: table => new
                {
                    UserUUID = table.Column<string>(type: "varchar(255)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    TokenClient = table.Column<string>(type: "varchar(255)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    RefreshToken = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    TokenIssueDate = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    TokenExpiryDate = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "tinyint(1)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserRefreshTokens", x => new { x.UserUUID, x.TokenClient });
                    table.ForeignKey(
                        name: "FK_UserRefreshTokens_Users_UserUUID",
                        column: x => x.UserUUID,
                        principalTable: "Users",
                        principalColumn: "UserUUID",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserRefreshTokens");
        }
    }
}
