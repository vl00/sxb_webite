using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace PMS.CommentsManage.Repository.Migrations
{
    public partial class _005 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ParentId",
                table: "SchoolCommentReplies");

            migrationBuilder.AlterColumn<Guid>(
                name: "ReplyId",
                table: "SchoolCommentReplies",
                nullable: true,
                oldClrType: typeof(Guid));

            migrationBuilder.CreateTable(
                name: "SchoolScores",
                columns: table => new
                {
                    SchoolSectionId = table.Column<Guid>(nullable: false),
                    SchoolId = table.Column<Guid>(nullable: false),
                    AggScore = table.Column<decimal>(nullable: false),
                    CommentCount = table.Column<int>(nullable: false),
                    AttendCommentCount = table.Column<int>(nullable: false),
                    TeachScore = table.Column<decimal>(nullable: false),
                    HardScore = table.Column<decimal>(nullable: false),
                    EnvirScore = table.Column<decimal>(nullable: false),
                    ManageScore = table.Column<decimal>(nullable: false),
                    LifeScore = table.Column<decimal>(nullable: false),
                    CreateTime = table.Column<DateTime>(nullable: false),
                    UpdateTime = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SchoolScores", x => x.SchoolSectionId);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SchoolCommentReplies_ReplyId",
                table: "SchoolCommentReplies",
                column: "ReplyId");

            migrationBuilder.CreateIndex(
                name: "IX_QuestionsAnswersInfos_ParentId",
                table: "QuestionsAnswersInfos",
                column: "ParentId");

            migrationBuilder.AddForeignKey(
                name: "FK_QuestionsAnswersInfos_QuestionsAnswersInfos_ParentId",
                table: "QuestionsAnswersInfos",
                column: "ParentId",
                principalTable: "QuestionsAnswersInfos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_SchoolCommentReplies_SchoolCommentReplies_ReplyId",
                table: "SchoolCommentReplies",
                column: "ReplyId",
                principalTable: "SchoolCommentReplies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_QuestionsAnswersInfos_QuestionsAnswersInfos_ParentId",
                table: "QuestionsAnswersInfos");

            migrationBuilder.DropForeignKey(
                name: "FK_SchoolCommentReplies_SchoolCommentReplies_ReplyId",
                table: "SchoolCommentReplies");

            migrationBuilder.DropTable(
                name: "SchoolScores");

            migrationBuilder.DropIndex(
                name: "IX_SchoolCommentReplies_ReplyId",
                table: "SchoolCommentReplies");

            migrationBuilder.DropIndex(
                name: "IX_QuestionsAnswersInfos_ParentId",
                table: "QuestionsAnswersInfos");

            migrationBuilder.AlterColumn<Guid>(
                name: "ReplyId",
                table: "SchoolCommentReplies",
                nullable: false,
                oldClrType: typeof(Guid),
                oldNullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "ParentId",
                table: "SchoolCommentReplies",
                nullable: true);
        }
    }
}
