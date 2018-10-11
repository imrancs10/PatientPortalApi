using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace OnSolve.Mobile.Data.Migrations
{
    public partial class RemovedGWBOptions : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "GetWordBackDetails");

            migrationBuilder.AddColumn<long>(
                name: "MessageId",
                table: "MessageDetails",
                nullable: false,
                defaultValue: 0L);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MessageId",
                table: "MessageDetails");

            migrationBuilder.CreateTable(
                name: "GetWordBackDetails",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    MessageDetailId = table.Column<int>(nullable: true),
                    MessageId = table.Column<long>(nullable: false),
                    WordBackId = table.Column<long>(nullable: false),
                    WordBackResponse = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GetWordBackDetails", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GetWordBackDetails_MessageDetails_MessageDetailId",
                        column: x => x.MessageDetailId,
                        principalTable: "MessageDetails",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_GetWordBackDetails_MessageDetailId",
                table: "GetWordBackDetails",
                column: "MessageDetailId");
        }
    }
}
