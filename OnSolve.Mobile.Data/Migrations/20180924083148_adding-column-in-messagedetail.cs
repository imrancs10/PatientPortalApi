using Microsoft.EntityFrameworkCore.Migrations;

namespace OnSolve.Mobile.Data.Migrations
{
    public partial class addingcolumninmessagedetail : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "ResponseRecieved",
                table: "MessageDetails",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ResponseRecieved",
                table: "MessageDetails");
        }
    }
}
