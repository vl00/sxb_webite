using System;
namespace PMS.Search.Application.ModelDto
{
    public class SearchLiveDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string CoverHome { get; set; }
        public DateTime StartTime { get; set; }
        public int Status { get; set; }

        //订阅收藏数
        public int BookCount { get; set; }
        //用户收听数
        public int UserCount { get; set; }

        public Guid UserId { get; set; }
        public string UserName { get; set; }
        public string UserHeadImg { get; set; }
        public bool IsCollection { get; set; }

    }
}
