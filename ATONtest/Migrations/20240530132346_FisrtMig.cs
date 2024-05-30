using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ATONtest.Migrations
{
    /// <inheritdoc />
    public partial class FisrtMig : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Login = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    Password = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    Gender = table.Column<int>(type: "int", nullable: false),
                    Birthday = table.Column<DateTime>(type: "date", nullable: true),
                    Admin = table.Column<bool>(type: "bit", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "date", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ModifiedOn = table.Column<DateTime>(type: "date", nullable: false),
                    ModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RevokedOn = table.Column<DateTime>(type: "date", nullable: true),
                    RevokedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Users_Login",
                table: "Users",
                column: "Login",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
