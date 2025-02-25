using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ASM_CS4.Migrations
{
    /// <inheritdoc />
    public partial class RemoveRoleColumnFromCustomer : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
			migrationBuilder.DropColumn(
		name: "Role",
		table: "AspNetUsers");
		}

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
