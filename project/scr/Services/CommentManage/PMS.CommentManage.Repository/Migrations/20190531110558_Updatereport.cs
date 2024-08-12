using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace PMS.CommentsManage.Repository.Migrations
{
    public partial class Updatereport : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AnswersReportReplys_QuestionsAnswersReports_ReportId",
                table: "AnswersReportReplys");

            migrationBuilder.DropForeignKey(
                name: "FK_QuestionsAnswersReports_QuestionsAnswersInfos_QuestionsAnswersInfoId",
                table: "QuestionsAnswersReports");

            migrationBuilder.DropForeignKey(
                name: "FK_QuestionsAnswersReports_ReportTypes_ReportReasonType",
                table: "QuestionsAnswersReports");

            migrationBuilder.DropForeignKey(
                name: "FK_QuestionsAnswersReports_UserInfo_ReportUserId",
                table: "QuestionsAnswersReports");

            migrationBuilder.DropPrimaryKey(
                name: "PK_QuestionsAnswersReports",
                table: "QuestionsAnswersReports");

            migrationBuilder.DropIndex(
                name: "IX_QuestionsAnswersReports_QuestionsAnswersInfoId",
                table: "QuestionsAnswersReports");

            migrationBuilder.RenameTable(
                name: "QuestionsAnswersReports",
                newName: "QuestionsReports");

            migrationBuilder.RenameIndex(
                name: "IX_QuestionsAnswersReports_ReportUserId",
                table: "QuestionsReports",
                newName: "IX_QuestionsReports_ReportUserId");

            migrationBuilder.RenameIndex(
                name: "IX_QuestionsAnswersReports_ReportReasonType",
                table: "QuestionsReports",
                newName: "IX_QuestionsReports_ReportReasonType");

            migrationBuilder.AddColumn<Guid>(
                name: "ReplayId",
                table: "SchoolCommentReports",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ReportDataType",
                table: "SchoolCommentReports",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<Guid>(
                name: "ParentId",
                table: "QuestionsAnswersInfos",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_QuestionsReports",
                table: "QuestionsReports",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_AnswersReportReplys_QuestionsReports_ReportId",
                table: "AnswersReportReplys",
                column: "ReportId",
                principalTable: "QuestionsReports",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_QuestionsReports_ReportTypes_ReportReasonType",
                table: "QuestionsReports",
                column: "ReportReasonType",
                principalTable: "ReportTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_QuestionsReports_UserInfo_ReportUserId",
                table: "QuestionsReports",
                column: "ReportUserId",
                principalTable: "UserInfo",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AnswersReportReplys_QuestionsReports_ReportId",
                table: "AnswersReportReplys");

            migrationBuilder.DropForeignKey(
                name: "FK_QuestionsReports_ReportTypes_ReportReasonType",
                table: "QuestionsReports");

            migrationBuilder.DropForeignKey(
                name: "FK_QuestionsReports_UserInfo_ReportUserId",
                table: "QuestionsReports");

            migrationBuilder.DropPrimaryKey(
                name: "PK_QuestionsReports",
                table: "QuestionsReports");

            migrationBuilder.DropColumn(
                name: "ReplayId",
                table: "SchoolCommentReports");

            migrationBuilder.DropColumn(
                name: "ReportDataType",
                table: "SchoolCommentReports");

            migrationBuilder.DropColumn(
                name: "ParentId",
                table: "QuestionsAnswersInfos");

            migrationBuilder.RenameTable(
                name: "QuestionsReports",
                newName: "QuestionsAnswersReports");

            migrationBuilder.RenameIndex(
                name: "IX_QuestionsReports_ReportUserId",
                table: "QuestionsAnswersReports",
                newName: "IX_QuestionsAnswersReports_ReportUserId");

            migrationBuilder.RenameIndex(
                name: "IX_QuestionsReports_ReportReasonType",
                table: "QuestionsAnswersReports",
                newName: "IX_QuestionsAnswersReports_ReportReasonType");

            migrationBuilder.AddPrimaryKey(
                name: "PK_QuestionsAnswersReports",
                table: "QuestionsAnswersReports",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_QuestionsAnswersReports_QuestionsAnswersInfoId",
                table: "QuestionsAnswersReports",
                column: "QuestionsAnswersInfoId");

            migrationBuilder.AddForeignKey(
                name: "FK_AnswersReportReplys_QuestionsAnswersReports_ReportId",
                table: "AnswersReportReplys",
                column: "ReportId",
                principalTable: "QuestionsAnswersReports",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_QuestionsAnswersReports_QuestionsAnswersInfos_QuestionsAnswersInfoId",
                table: "QuestionsAnswersReports",
                column: "QuestionsAnswersInfoId",
                principalTable: "QuestionsAnswersInfos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_QuestionsAnswersReports_ReportTypes_ReportReasonType",
                table: "QuestionsAnswersReports",
                column: "ReportReasonType",
                principalTable: "ReportTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_QuestionsAnswersReports_UserInfo_ReportUserId",
                table: "QuestionsAnswersReports",
                column: "ReportUserId",
                principalTable: "UserInfo",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
