using Microsoft.EntityFrameworkCore.Migrations;

namespace OnSolve.Mobile.Data.Migrations
{
    public partial class vanishisdeletedmessagedetail : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "MessageDetails",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsVanishEnabled",
                table: "MessageDetails",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "MessageDetails");

            migrationBuilder.DropColumn(
                name: "IsVanishEnabled",
                table: "MessageDetails");
        }
    }
}
