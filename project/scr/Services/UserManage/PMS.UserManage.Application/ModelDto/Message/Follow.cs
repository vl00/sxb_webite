using PMS.CommentsManage.Application.ModelDto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PMS.UserManage.Application.ModelDto.Message
{
    public class MessageModel
    {
        public Guid Id { get; set; }
        public Guid DataID { get; set; }
        public Guid SenderID { get; set; }
        public string Nickname { get; set; }
        public string HeadImgUrl { get; set; }
        public string Content { get; set; }
        public string Time { get; set; }
        public byte Type { get; set; }
        public byte DataType { get; set; }
        public bool IsAnony { get; set; }
        public SchoolModel SchoolModel { get; set; }
    }

    public class ApiMessageModel
    {
        public Guid Id { get; set; }
        public Guid DataID { get; set; }
        public Guid SenderID { get; set; }
        public string Nickname { get; set; }
        public string HeadImgUrl { get; set; }
        public string Content { get; set; }
        public string Time { get; set; }
        public byte Type { get; set; }
        public byte DataType { get; set; }
        public bool IsAnony { get; set; }
        public SchoolInfoDto SchoolModel { get; set; }
    }
}
