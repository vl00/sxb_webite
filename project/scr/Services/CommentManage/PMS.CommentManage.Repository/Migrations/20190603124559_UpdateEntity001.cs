using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace PMS.CommentsManage.Repository.Migrations
{
    public partial class UpdateEntity001 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ReplayId",
                table: "QuestionsAnswersReports",
                newName: "AnswersReplyId");

            migrationBuilder.AlterColumn<Guid>(
                name: "QuestionsAnswersInfoId",
                table: "QuestionsAnswersReports",
                nullable: true,
                oldClrType: typeof(Guid));

            migrationBuilder.AddColumn<Guid>(
                name: "QuestionId",
                table: "QuestionsAnswersReports",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "QuestionId",
                table: "QuestionsAnswersReports");

            migrationBuilder.RenameColumn(
                name: "AnswersReplyId",
                table: "QuestionsAnswersReports",
                newName: "ReplayId");

            migrationBuilder.AlterColumn<Guid>(
                name: "QuestionsAnswersInfoId",
                table: "QuestionsAnswersReports",
                nullable: false,
                oldClrType: typeof(Guid),
                oldNullable: true);
        }
    }
}
