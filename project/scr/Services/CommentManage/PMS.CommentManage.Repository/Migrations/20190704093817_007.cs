using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace PMS.CommentsManage.Repository.Migrations
{
    public partial class _007 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_QuestionInfos_UserInfo_UserId",
                table: "QuestionInfos");

            migrationBuilder.DropForeignKey(
                name: "FK_QuestionsAnswersInfos_UserInfo_UserId",
                table: "QuestionsAnswersInfos");

            migrationBuilder.DropForeignKey(
                name: "FK_QuestionsAnswersReports_UserInfo_ReportUserId",
                table: "QuestionsAnswersReports");

            migrationBuilder.DropForeignKey(
                name: "FK_SchoolCommentReplies_UserInfo_UserId",
                table: "SchoolCommentReplies");

            migrationBuilder.DropForeignKey(
                name: "FK_SchoolCommentReports_UserInfo_ReportUserId",
                table: "SchoolCommentReports");

            migrationBuilder.DropForeignKey(
                name: "FK_SchoolComments_UserInfo_CommentUserId",
                table: "SchoolComments");

            migrationBuilder.DropForeignKey(
                name: "FK_SchoolTags_UserInfo_UserId",
                table: "SchoolTags");

            migrationBuilder.DropTable(
                name: "UserInfo");

            migrationBuilder.DropIndex(
                name: "IX_SchoolTags_UserId",
                table: "SchoolTags");

            migrationBuilder.DropIndex(
                name: "IX_SchoolComments_CommentUserId",
                table: "SchoolComments");

            migrationBuilder.DropIndex(
                name: "IX_SchoolCommentReports_ReportUserId",
                table: "SchoolCommentReports");

            migrationBuilder.DropIndex(
                name: "IX_SchoolCommentReplies_UserId",
                table: "SchoolCommentReplies");

            migrationBuilder.DropIndex(
                name: "IX_QuestionsAnswersReports_ReportUserId",
                table: "QuestionsAnswersReports");

            migrationBuilder.DropIndex(
                name: "IX_QuestionsAnswersInfos_UserId",
                table: "QuestionsAnswersInfos");

            migrationBuilder.DropIndex(
                name: "IX_QuestionInfos_UserId",
                table: "QuestionInfos");

            migrationBuilder.AddColumn<int>(
                name: "PostUserRole",
                table: "SchoolComments",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "PostUserRole",
                table: "SchoolCommentReplies",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "PostUserRole",
                table: "QuestionsAnswersInfos",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "PostUserRole",
                table: "QuestionInfos",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PostUserRole",
                table: "SchoolComments");

            migrationBuilder.DropColumn(
                name: "PostUserRole",
                table: "SchoolCommentReplies");

            migrationBuilder.DropColumn(
                name: "PostUserRole",
                table: "QuestionsAnswersInfos");

            migrationBuilder.DropColumn(
                name: "PostUserRole",
                table: "QuestionInfos");

            migrationBuilder.CreateTable(
                name: "UserInfo",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    HeadImager = table.Column<string>(nullable: true),
                    NickName = table.Column<string>(nullable: false),
                    Phone = table.Column<string>(nullable: false),
                    Role = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserInfo", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SchoolTags_UserId",
                table: "SchoolTags",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_SchoolComments_CommentUserId",
                table: "SchoolComments",
                column: "CommentUserId");

            migrationBuilder.CreateIndex(
                name: "IX_SchoolCommentReports_ReportUserId",
                table: "SchoolCommentReports",
                column: "ReportUserId");

            migrationBuilder.CreateIndex(
                name: "IX_SchoolCommentReplies_UserId",
                table: "SchoolCommentReplies",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_QuestionsAnswersReports_ReportUserId",
                table: "QuestionsAnswersReports",
                column: "ReportUserId");

            migrationBuilder.CreateIndex(
                name: "IX_QuestionsAnswersInfos_UserId",
                table: "QuestionsAnswersInfos",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_QuestionInfos_UserId",
                table: "QuestionInfos",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_QuestionInfos_UserInfo_UserId",
                table: "QuestionInfos",
                column: "UserId",
                principalTable: "UserInfo",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_QuestionsAnswersInfos_UserInfo_UserId",
                table: "QuestionsAnswersInfos",
                column: "UserId",
                principalTable: "UserInfo",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_QuestionsAnswersReports_UserInfo_ReportUserId",
                table: "QuestionsAnswersReports",
                column: "ReportUserId",
                principalTable: "UserInfo",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_SchoolCommentReplies_UserInfo_UserId",
                table: "SchoolCommentReplies",
                column: "UserId",
                principalTable: "UserInfo",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_SchoolCommentReports_UserInfo_ReportUserId",
                table: "SchoolCommentReports",
                column: "ReportUserId",
                principalTable: "UserInfo",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_SchoolComments_UserInfo_CommentUserId",
                table: "SchoolComments",
                column: "CommentUserId",
                principalTable: "UserInfo",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_SchoolTags_UserInfo_UserId",
                table: "SchoolTags",
                column: "UserId",
                principalTable: "UserInfo",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
