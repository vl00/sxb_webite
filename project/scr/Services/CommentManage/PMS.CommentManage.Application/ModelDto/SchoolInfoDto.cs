using System;
using PMS.School.Domain.Common;
using ProductManagement.Framework.Foundation;

namespace PMS.CommentsManage.Application.ModelDto
{
    public class SchoolInfoDto
    {
        public Guid SchoolSectionId { get; set; }
        public Guid SchoolId { get; set; }
        public string SchoolName { get; set; }
        //学校类型
        public SchoolType SchoolType { get; set; }

        public SchoolGrade SchoolGrade { get; set; }

        /// <summary>
        /// 是否寄宿
        /// </summary>
        public bool? Lodging { get; set; }
        /// <summary>
        /// 是否走读
        /// </summary>
        public bool? Sdextern { get; set; }

        /// <summary>
        /// 寄宿类型
        /// </summary>
        public LodgingEnum LodgingType => LodgingUtil.Reason(Lodging, Sdextern);


        /// <summary>
        /// 学校总平均分
        /// </summary>
        public decimal SchoolAvgScore { get; set; }

        /// <summary>
        /// 前端展示星星
        /// </summary>
        public int SchoolStars { get; set; }

        /// <summary>
        /// 分部所在学校的评论数
        /// </summary>
        public int CommentTotal { get; set; }

        /// <summary>
        /// 分部自身的评论数
        /// </summary>
        public int SectionCommentTotal { get; set; }

        /// <summary>
        /// 分部下总提问数
        /// </summary>
        public int SectionQuestionTotal { get; set; }

        /// <summary>
        /// 是否上学帮认证
        /// </summary>
        public bool IsAuth { get; set; }

        public bool IsExists => SectionCommentTotal > 0;

        public string SchoolBranch => SchoolName.Split('-')[1];
        public int SchoolNo { get; set; }
        public string ShortSchoolNo
        {
            get
            {
                return UrlShortIdUtil.Long2Base32(SchoolNo)?.ToLower();
            }
        }
        public decimal SchoolCommentAvgScore { get; set; }
    }

    public class SchoolInfoStatuDto : SchoolInfoDto
    {
        public bool IsValid { get; set; }

        public int Status { get; set; }
    }

}
