using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace OnSolve.Mobile.Data.Migrations
{
    public partial class InitialMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "EmailVerificationCode",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Email = table.Column<string>(nullable: false),
                    Code = table.Column<string>(nullable: false),
                    CreatedDateTime = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmailVerificationCode", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MessageSenderDetails",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    SenderFullName = table.Column<string>(nullable: true),
                    SenderPhoneNumber = table.Column<string>(nullable: true),
                    SenderEmail = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MessageSenderDetails", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MobileRecipients",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    MobileUserId = table.Column<long>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MobileRecipients", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MobileUser",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    RecipientId = table.Column<long>(nullable: false),
                    AccountId = table.Column<int>(nullable: false),
                    ENSUserId = table.Column<long>(nullable: true),
                    Username = table.Column<string>(nullable: true),
                    Password = table.Column<string>(nullable: true),
                    Salt = table.Column<string>(nullable: true),
                    CreatedOn = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MobileUser", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "FCMTokenInfo",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    FCMToken = table.Column<string>(nullable: false),
                    MobileUserId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FCMTokenInfo", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FCMTokenInfo_MobileUser_MobileUserId",
                        column: x => x.MobileUserId,
                        principalTable: "MobileUser",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MessageDetails",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Title = table.Column<string>(nullable: false),
                    Text = table.Column<string>(nullable: false),
                    MessageTransactionId = table.Column<long>(nullable: false),
                    ContactPointId = table.Column<long>(nullable: false),
                    IsGetWordBack = table.Column<bool>(nullable: false),
                    RecipientId = table.Column<long>(nullable: false),
                    DateTimeSent = table.Column<DateTime>(nullable: false),
                    MessageSenderDetailId = table.Column<int>(nullable: true),
                    ExpirationDate = table.Column<DateTime>(nullable: false),
                    MobileUserId = table.Column<int>(nullable: true),
                    MessageType = table.Column<int>(nullable: false),
                    IsRead = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MessageDetails", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MessageDetails_MessageSenderDetails_MessageSenderDetailId",
                        column: x => x.MessageSenderDetailId,
                        principalTable: "MessageSenderDetails",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_MessageDetails_MobileUser_MobileUserId",
                        column: x => x.MobileUserId,
                        principalTable: "MobileUser",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ResetPasswordCode",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    ResetCode = table.Column<string>(nullable: false),
                    MobileUserId = table.Column<int>(nullable: false),
                    CreationTime = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ResetPasswordCode", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ResetPasswordCode_MobileUser_MobileUserId",
                        column: x => x.MobileUserId,
                        principalTable: "MobileUser",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "GetWordBackDetails",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    WordBackId = table.Column<long>(nullable: false),
                    MessageId = table.Column<long>(nullable: false),
                    WordBackResponse = table.Column<string>(nullable: true),
                    MessageDetailId = table.Column<int>(nullable: true)
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
                name: "IX_FCMTokenInfo_MobileUserId",
                table: "FCMTokenInfo",
                column: "MobileUserId");

            migrationBuilder.CreateIndex(
                name: "IX_GetWordBackDetails_MessageDetailId",
                table: "GetWordBackDetails",
                column: "MessageDetailId");

            migrationBuilder.CreateIndex(
                name: "IX_MessageDetails_MessageSenderDetailId",
                table: "MessageDetails",
                column: "MessageSenderDetailId");

            migrationBuilder.CreateIndex(
                name: "IX_MessageDetails_MobileUserId",
                table: "MessageDetails",
                column: "MobileUserId");

            migrationBuilder.CreateIndex(
                name: "IX_ResetPasswordCode_MobileUserId",
                table: "ResetPasswordCode",
                column: "MobileUserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EmailVerificationCode");

            migrationBuilder.DropTable(
                name: "FCMTokenInfo");

            migrationBuilder.DropTable(
                name: "GetWordBackDetails");

            migrationBuilder.DropTable(
                name: "MobileRecipients");

            migrationBuilder.DropTable(
                name: "ResetPasswordCode");

            migrationBuilder.DropTable(
                name: "MessageDetails");

            migrationBuilder.DropTable(
                name: "MessageSenderDetails");

            migrationBuilder.DropTable(
                name: "MobileUser");
        }
    }
}
