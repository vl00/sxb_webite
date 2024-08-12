using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace PMS.CommentsManage.Repository.Migrations
{
    public partial class UpdateEntitys : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "ReplayId",
                table: "QuestionsAnswersReports",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ReportDataType",
                table: "QuestionsAnswersReports",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "IsTop",
                table: "QuestionInfos",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "ReplyCount",
                table: "QuestionInfos",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ReplayId",
                table: "QuestionsAnswersReports");

            migrationBuilder.DropColumn(
                name: "ReportDataType",
                table: "QuestionsAnswersReports");

            migrationBuilder.DropColumn(
                name: "IsTop",
                table: "QuestionInfos");

            migrationBuilder.DropColumn(
                name: "ReplyCount",
                table: "QuestionInfos");
        }
    }
}
