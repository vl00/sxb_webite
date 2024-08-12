using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace PMS.CommentsManage.Repository.Migrations
{
    public partial class _008 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "UserInfoExId",
                table: "SchoolComments",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "UserInfoExId",
                table: "SchoolCommentReports",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "UserInfoExId",
                table: "QuestionsAnswersReports",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "UserInfoExId",
                table: "QuestionsAnswersInfos",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "UserInfoExId",
                table: "QuestionInfos",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "UserInfo",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    NickName = table.Column<string>(nullable: false),
                    Phone = table.Column<string>(nullable: false),
                    Role = table.Column<int>(nullable: false),
                    HeadImager = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserInfo", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SchoolComments_UserInfoExId",
                table: "SchoolComments",
                column: "UserInfoExId");

            migrationBuilder.CreateIndex(
                name: "IX_SchoolCommentReports_UserInfoExId",
                table: "SchoolCommentReports",
                column: "UserInfoExId");

            migrationBuilder.CreateIndex(
                name: "IX_QuestionsAnswersReports_UserInfoExId",
                table: "QuestionsAnswersReports",
                column: "UserInfoExId");

            migrationBuilder.CreateIndex(
                name: "IX_QuestionsAnswersInfos_UserInfoExId",
                table: "QuestionsAnswersInfos",
                column: "UserInfoExId");

            migrationBuilder.CreateIndex(
                name: "IX_QuestionInfos_UserInfoExId",
                table: "QuestionInfos",
                column: "UserInfoExId");

            migrationBuilder.AddForeignKey(
                name: "FK_QuestionInfos_UserInfo_UserInfoExId",
                table: "QuestionInfos",
                column: "UserInfoExId",
                principalTable: "UserInfo",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_QuestionsAnswersInfos_UserInfo_UserInfoExId",
                table: "QuestionsAnswersInfos",
                column: "UserInfoExId",
                principalTable: "UserInfo",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_QuestionsAnswersReports_UserInfo_UserInfoExId",
                table: "QuestionsAnswersReports",
                column: "UserInfoExId",
                principalTable: "UserInfo",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_SchoolCommentReports_UserInfo_UserInfoExId",
                table: "SchoolCommentReports",
                column: "UserInfoExId",
                principalTable: "UserInfo",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_SchoolComments_UserInfo_UserInfoExId",
                table: "SchoolComments",
                column: "UserInfoExId",
                principalTable: "UserInfo",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_QuestionInfos_UserInfo_UserInfoExId",
                table: "QuestionInfos");

            migrationBuilder.DropForeignKey(
                name: "FK_QuestionsAnswersInfos_UserInfo_UserInfoExId",
                table: "QuestionsAnswersInfos");

            migrationBuilder.DropForeignKey(
                name: "FK_QuestionsAnswersReports_UserInfo_UserInfoExId",
                table: "QuestionsAnswersReports");

            migrationBuilder.DropForeignKey(
                name: "FK_SchoolCommentReports_UserInfo_UserInfoExId",
                table: "SchoolCommentReports");

            migrationBuilder.DropForeignKey(
                name: "FK_SchoolComments_UserInfo_UserInfoExId",
                table: "SchoolComments");

            migrationBuilder.DropTable(
                name: "UserInfo");

            migrationBuilder.DropIndex(
                name: "IX_SchoolComments_UserInfoExId",
                table: "SchoolComments");

            migrationBuilder.DropIndex(
                name: "IX_SchoolCommentReports_UserInfoExId",
                table: "SchoolCommentReports");

            migrationBuilder.DropIndex(
                name: "IX_QuestionsAnswersReports_UserInfoExId",
                table: "QuestionsAnswersReports");

            migrationBuilder.DropIndex(
                name: "IX_QuestionsAnswersInfos_UserInfoExId",
                table: "QuestionsAnswersInfos");

            migrationBuilder.DropIndex(
                name: "IX_QuestionInfos_UserInfoExId",
                table: "QuestionInfos");

            migrationBuilder.DropColumn(
                name: "UserInfoExId",
                table: "SchoolComments");

            migrationBuilder.DropColumn(
                name: "UserInfoExId",
                table: "SchoolCommentReports");

            migrationBuilder.DropColumn(
                name: "UserInfoExId",
                table: "QuestionsAnswersReports");

            migrationBuilder.DropColumn(
                name: "UserInfoExId",
                table: "QuestionsAnswersInfos");

            migrationBuilder.DropColumn(
                name: "UserInfoExId",
                table: "QuestionInfos");
        }
    }
}
