using Microsoft.EntityFrameworkCore.Migrations;

namespace PMS.CommentsManage.Repository.Migrations
{
    public partial class Updaterrecord : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsPartTimeJob",
                table: "ExaminerRecords",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsPartTimeJob",
                table: "ExaminerRecords");
        }
    }
}
