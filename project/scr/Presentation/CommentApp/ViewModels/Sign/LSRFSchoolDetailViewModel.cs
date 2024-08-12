using PMS.OperationPlateform.Domain.Entitys;
using PMS.OperationPlateform.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sxb.Web.ViewModels.Sign
{
    public class LSRFSchoolDetailViewModel
    {
        public Guid Eid { get; set; }
        public Guid Sid { get; set; }
        public string Sname { get; set; }
        /// <summary>
        /// 寄宿类型
        /// </summary>
        public int LodgingType { get; set; }
        /// <summary>
        /// 寄宿类型描述
        /// </summary>
        public string LodgingReason { get; set; }
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
        public List<KeyValue> Authentication { get; set; }
        /// <summary>
        /// 学校简介
        /// </summary>
        public string Intro { get; set; }
        /// <summary>
        /// 课程设置
        /// </summary>
        public List<KeyValue> Courses { get; set; }
        /// <summary>
        /// 考试科目
        /// </summary>
        public string Subjects { get; set; }
        /// <summary>
        /// 招生人数
        /// </summary>
        public int Count { get; set; }
        /// <summary>
        /// 硬件设施图
        /// </summary>
        public List<string> Hardware { get; set; }
        /// <summary>
        /// 展示类型
        /// </summary>
        public LSRFSchoolType Type { get; set; }
    }
}
