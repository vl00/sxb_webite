using System;
using System.Collections.Generic;
using ProductManagement.Framework.Foundation;
using Sxb.PCWeb.ViewModels.Rank;

namespace Sxb.PCWeb.ViewModels.School
{
    public class SchoolExtListItemViewModel
    {
        /// <summary>
        /// 学校分部id
        /// </summary>
        public Guid BranchId { get; set; }
        /// <summary>
        /// 学校id
        /// </summary>
        public Guid SchoolId { get; set; }

        /// <summary>
        /// 学校详情页URL
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        /// 学校名称
        /// </summary>
        public string SchoolName { get; set; }

        /// <summary>
        /// 是否上学帮认证
        /// </summary>
        public bool Auth { get; set; }

        /// <summary>
        /// 寄宿类型
        /// </summary>
        public int LodgingType { get; set; }
        /// <summary>
        /// 寄宿类型描述
        /// </summary>
        public string LodgingReason { get; set; }

        public int Type { get; set; }

        /// <summary>
        /// 是否国际学校
        /// </summary>
        public bool International { get; set; }

        /// <summary>
        /// 费用
        /// </summary>
        public double? Cost { get; set; }

        /// <summary>
        /// 学生人数
        /// </summary>
        public int? StudentCount { get; set; }
        /// <summary>
        /// 教师人数
        /// </summary>
        public int? TeacherCount { get; set; }
        /// <summary>
        /// 建校时间
        /// </summary>
        public DateTime? CreationDate { get; set; }

        /// <summary>
        /// 距离
        /// </summary>
        public double? Distance { get; set; }

        /// <summary>
        /// 城市名称
        /// </summary>
        public string CityName { get; set; }

        /// <summary>
        /// 城区名称
        /// </summary>
        public string AreaName { get; set; }


        public List<string> Tags { get; set; }

        /// <summary>
        /// 口碑评级分
        /// </summary>
        public double? Score { get; set; }
        /// <summary>
        /// 口碑评级
        /// </summary>
        public string Str_Score
        {
            get
            {
                if (Score.HasValue) return Score.Value.GetScoreString();
                return string.Empty;
            }
        }
        /// <summary>
        /// 星星
        /// </summary>
        public int Stars { get; set; }
        /// <summary>
        /// 学校点评总条数
        /// </summary>
        public int CommentTotal { get; set; }


        /// <summary>
        /// 评论的id
        /// </summary>
        public Guid? CommontId { get; set; }
        /// <summary>
        /// 评论的序号
        /// </summary>
        public string CommontNo { get; set; }
        /// <summary>
        /// 评论内容
        /// </summary>
        public string Comment { get; set; }
        /// <summary>
        /// 评论条数
        /// </summary>
        public int CommentReplyCount { get; set; }

        /// <summary>
        /// 排行榜
        /// </summary>
        public List<SchoolRankInfoViewModel> Ranks;

        public double? SearchScore { get; set; }
        /// <summary>
        /// 学校自编编号
        /// </summary>
        public int SchoolNo { get; set; }
        public string ShortSchoolNo
        {
            get
            {
                return UrlShortIdUtil.Long2Base32(SchoolNo).ToLower();
            }
        }
    }
}
