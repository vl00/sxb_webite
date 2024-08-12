using System;
using System.Collections.Generic;
using System.Text;

namespace PMS.OperationPlateform.Domain.DTOs
{
    public class ArticleDetailDto
    {

        /// <summary>
        /// 标识
        /// </summary>
        public Guid Id { get; set; }


        /// <summary>
        /// 文章标题
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// 发表时间
        /// </summary>
        public string Time { get; set; }

        /// <summary>
        /// 阅读量
        /// </summary>
        public int ViewCount { get; set; }


        /// <summary>
        /// 评论数量
        /// </summary>
        public int CommentCount { get; set; }


        /// <summary>
        /// 文章内容 
        /// </summary>
        public string Html { get; set; }


        public string Digest { get; set; }

        /// <summary>
        /// 相关的群组二维码
        /// </summary>
        public List<string> GroupQRCodes { get; set; }

        /// <summary>
        /// 相关热点标签
        /// </summary>
        public List<string> CorrelationTags { get; set; }

        /// <summary>
        /// 上一篇文章, 空为第一篇
        /// </summary>
        public UrlArticle PrevArticle { get; set; }
        /// <summary>
        /// 下一篇文章, 空为最后一篇
        /// </summary>
        public UrlArticle NextArticle { get; set; }

        /// <summary>
        /// 作者名称  普通作者/达人作者
        /// </summary>
        public string Author { get; set; }

        /// <summary>
        /// 当前登录人是否收藏文章
        /// </summary>
        public bool IsFollow { get; set; }

        /// <summary>
        /// 如果为达人的作者信息
        /// </summary>
        public ArticleDetailUserDto AuthorUserInfo { get; set; }

        /// <summary>
        /// 上一篇 下一篇 url action
        /// </summary>
        public class UrlArticle
        {
            /// <summary>
            /// 文章短链接
            /// </summary>
            public string No { get; set; }
            /// <summary>
            /// 文章标题
            /// </summary>
            public string Title { get; set; }
        }

        /// <summary>
        /// 文章作者信息
        /// </summary>
        public class ArticleDetailUserDto
        {
            /// <summary>
            /// 作者Id
            /// </summary>
            public Guid? UserId { get; set; }

            /// <summary>
            /// 当前登录人是否关注作者
            /// </summary>
            public bool IsFollow { get; set; }

            /// <summary> 
            /// 用户名称
            /// </summary> 
            public string NickName { get; set; }

            /// <summary>
            ///用户头像
            /// </summary>
            public string HeadImgUrl { get; set; }

            /// <summary>
            /// 认证名称
            /// </summary>
            public string AuthTitle { get; set; }

            /// <summary>
            /// 达人粉丝数
            /// </summary>
            public long FansTotal { get; set; }

            /// <summary>
            /// 达人回答问题数
            /// 仅算回答问题, 回复回答不算
            /// </summary>
            public long AnswersTotal { get; set; }

            /// <summary>
            /// 达人类型 0 个人  1 机构
            /// </summary>
            public int Type { get; set; }

            /// <summary>
            /// 用户简介
            /// </summary>
            public string Introduction { get; set; }

            /// <summary>
            /// 作者的圈子id
            /// </summary>
            public Guid? CircleId { get; set; }

            /// <summary>
            /// 作者的圈子名称
            /// </summary>
            public string CircleName { get; set; }
        }
    }
}
