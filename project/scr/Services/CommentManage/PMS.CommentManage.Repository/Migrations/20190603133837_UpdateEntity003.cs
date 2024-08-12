using Microsoft.EntityFrameworkCore.Migrations;

namespace PMS.CommentsManage.Repository.Migrations
{
    public partial class UpdateEntity003 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ReplyCount",
                table: "QuestionsAnswersInfos",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ReplyCount",
                table: "QuestionsAnswersInfos");
        }
    }
}
