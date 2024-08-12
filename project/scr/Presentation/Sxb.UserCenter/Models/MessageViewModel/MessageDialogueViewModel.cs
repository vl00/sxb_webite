using Sxb.UserCenter.Models.SchoolViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PMS.School.Domain.Common;

namespace Sxb.UserCenter.Models.MessageViewModel
{
    /// <summary>
    /// 消息实体
    /// </summary>
    public class MessageDialogueViewModel
    {
        public MessageDialogueViewModel()
        {
            Content = "";
            Title = "";
            SchoolName = "";
            ArticleCovers = "";
        }

        /// <summary>
        /// 数据id
        /// </summary>
        public Guid DataId { get; set; }
        /// <summary>
        /// 系统消息类型
        /// </summary>
        public int Type { get; set; }
        /// <summary>
        /// 数据类型
        /// </summary>
        public int? DataType { get; set; }
        /// <summary>
        /// 原始MessageType类型
        /// </summary>
        public int? OriType { get; set; }
        /// <summary>
        /// 消息日期
        /// </summary>
        public string Time { get; set; }
        /// <summary>
        /// 消息内容
        /// </summary>
        public string Content { get; set; }
        /// <summary>
        /// 消息标题
        /// </summary>
        public string Title { get; set; }
        /// <summary>
        /// 学校信息
        /// </summary>
        public string SchoolName { get; set; }
        /// <summary>
        /// 文章图
        /// </summary>
        public string ArticleCovers { get; set; }
        /// <summary>
        /// 文章查看数量
        /// </summary>
        public int ArticleViewCount { get; set; }

        /// <summary>
        /// 发布者名
        /// </summary>
        public string SenderNickname { get; set; }
        /// <summary>
        /// 发布者头像
        /// </summary>
        public string SenderHeadImgUrl { get; set; }

        /// <summary>
        /// 问题或者点评的页面序号
        /// </summary>
        public string No { get; set; }

        //学校类型
        public SchoolType SchoolType { get; set; }

        /// <summary>
        /// 寄宿类型 
        /// </summary>
        public LodgingEnum LodgingType { get; set; }
        /// <summary>
        /// 前端展示星星
        /// </summary>
        public int SchoolStars { get; set; }
    }
}
