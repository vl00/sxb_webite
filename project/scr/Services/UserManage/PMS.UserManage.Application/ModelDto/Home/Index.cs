using PMS.UserManage.Application.ModelDto.Info;
using PMS.UserManage.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PMS.UserManage.Application.ModelDto.Home
{
    public class Index: RootModel
    {
        public Index()
        {
            count = new CountData();
            verify = new List<byte>();
        }
        public bool islogin { get; set; }
        public string nickname { get; set; }
        public string headImgUrl { get; set; }
        public string mobile { get; set; }
        public DateTime regTime { get; set; }
        public List<byte> verify { get; set; }
        public CountData count { get; set; }
        public Interest interest { get; set; }
    }
    public class CountData
    {
        public int publish { get; set; }
        public int reply { get; set; }
        public int follow { get; set; }
        public int like { get; set; }
        public int message { get; set; }
    }
    public class M2CountData
    {
        public int publishTotal { get; set; }
        public int answerAndReplyTotal { get; set; }
        public int likeTotal { get; set; }
    }
}
