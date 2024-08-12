using PMS.OperationPlateform.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace PMS.OperationPlateform.Domain.Entitys
{
    public class LSRFSchoolDetail
    {
        public Guid Eid { get; set; }
        public Guid Sid { get; set; }
        public string Sname { get; set; }
        /// <summary>
        /// 是否寄宿
        /// </summary>
        public bool? Lodging { get; set; }
        /// <summary>
        /// 是否走读
        /// </summary>
        public bool? Sdextern { get; set; }

        /// <summary>
        /// 广告配图
        /// </summary>
        public string AdvPicUrl { get; set; }
        /// <summary>
        /// 广告展示类型
        /// </summary>
        public int AdvType { get; set; }
        /// <summary>
        /// 出国方向
        /// </summary>
        public string Abroad { get; set; }
        /// <summary>
        /// 学校认证
        /// </summary>
        public string Authentication { get; set; }
        /// <summary>
        /// 学校简介
        /// </summary>
        public string Intro { get; set; }
        /// <summary>
        /// 课程设置
        /// </summary>
        public string Courses { get; set; }
        /// <summary>
        /// 考试科目
        /// </summary>
        public string Subjects { get; }
        /// <summary>
        /// 招生人数
        /// </summary>
        public string Count { get; set; }
        /// <summary>
        /// 硬件设施图
        /// </summary>
        public string Hardware { get; set; }
        /// <summary>
        /// 展示类型（3：广告，2推荐，1：普通）
        /// </summary>
        public LSRFSchoolType Type { get; set; }
    }

    public class SingSchoolRank 
    {
        public Guid SchId { get; set; }
        public long Rank { get; set; }
    }
}
