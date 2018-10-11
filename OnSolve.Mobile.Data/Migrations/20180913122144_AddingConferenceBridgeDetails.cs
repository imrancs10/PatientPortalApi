using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace OnSolve.Mobile.Data.Migrations
{
    public partial class AddingConferenceBridgeDetails : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ConferenceBridgeDetailId",
                table: "MessageDetails",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsConference",
                table: "MessageDetails",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateTable(
                name: "ConferenceBridgeDetails",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    ConferencePhoneNumber = table.Column<string>(nullable: true),
                    ParticipantCode = table.Column<string>(nullable: true),
                    IsSendConferenceIdInMessage = table.Column<bool>(nullable: false),
                    IsSendConferencePhoneInMessage = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ConferenceBridgeDetails", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MessageDetails_ConferenceBridgeDetailId",
                table: "MessageDetails",
                column: "ConferenceBridgeDetailId");

            migrationBuilder.AddForeignKey(
                name: "FK_MessageDetails_ConferenceBridgeDetails_ConferenceBridgeDetailId",
                table: "MessageDetails",
                column: "ConferenceBridgeDetailId",
                principalTable: "ConferenceBridgeDetails",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MessageDetails_ConferenceBridgeDetails_ConferenceBridgeDetailId",
                table: "MessageDetails");

            migrationBuilder.DropTable(
                name: "ConferenceBridgeDetails");

            migrationBuilder.DropIndex(
                name: "IX_MessageDetails_ConferenceBridgeDetailId",
                table: "MessageDetails");

            migrationBuilder.DropColumn(
                name: "ConferenceBridgeDetailId",
                table: "MessageDetails");

            migrationBuilder.DropColumn(
                name: "IsConference",
                table: "MessageDetails");
        }
    }
}
