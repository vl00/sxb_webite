using Microsoft.EntityFrameworkCore.Migrations;

namespace PMS.CommentsManage.Repository.Migrations
{
    public partial class SchoolReply : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsHaveImagers",
                table: "SchoolComments",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsTop",
                table: "QuestionsAnswersInfos",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsHaveImagers",
                table: "QuestionInfos",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsHaveImagers",
                table: "SchoolComments");

            migrationBuilder.DropColumn(
                name: "IsTop",
                table: "QuestionsAnswersInfos");

            migrationBuilder.DropColumn(
                name: "IsHaveImagers",
                table: "QuestionInfos");
        }
    }
}
