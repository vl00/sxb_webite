using System;
using PMS.School.Domain.Common;
using ProductManagement.Framework.Foundation;

namespace PMS.CommentsManage.Application.ModelDto
{
    public class SchoolInfoQaDto
    {
        public Guid SchoolSectionId { get; set; }
        public Guid SchoolId { get; set; }
        public string SchoolName { get; set; }
        //学校类型
        public SchoolType SchoolType { get; set; }
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
        /// 当前学校下的总问题数
        /// </summary>
        public int SchoolQuestionTotal { get; set; }
        /// <summary>
        /// 学校总回复数
        /// </summary>
        public int SchoolReplyTotal { get; set; }
        /// <summary>
        /// 是否为国际
        /// </summary>
        public bool IsInternactioner { get; set; }
        /// <summary>
        /// 是否住校
        /// </summary>
        public bool IsLoding { get; set; }
        /// <summary>
        /// 是否认证
        /// </summary>
        public bool IsAuth { get; set; }

        public bool IsExists => SchoolQuestionTotal > 0;

        public string SchoolBranch => SchoolName.Split('-')[1];

        public int SchoolNo { get; set; }
        public string ShortSchoolNo => UrlShortIdUtil.Long2Base32(SchoolNo).ToLower();
    }
}
