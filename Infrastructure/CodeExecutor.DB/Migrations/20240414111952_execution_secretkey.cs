using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CodeExecutor.DB.Migrations
{
    /// <inheritdoc />
    public partial class execution_secretkey : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "SecretKey",
                table: "CodeExecutions",
                type: "character varying(32)",
                maxLength: 32,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SecretKey",
                table: "CodeExecutions");
        }
    }
}
