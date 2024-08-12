using Microsoft.EntityFrameworkCore.Migrations;

namespace PMS.CommentsManage.Repository.Migrations
{
    public partial class Updateuser : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "HeadImager",
                table: "UserInfo",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "HeadImager",
                table: "UserInfo");
        }
    }
}
