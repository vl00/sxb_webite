using System;
using System.Collections.Generic;

namespace Sxb.Web.RequestModel.School
{
    public class SchoolExtFilterData
    {
        /// <summary>
        /// 搜索关键字
        /// </summary>
        public string KeyWords { get; set; }
        /// <summary>
        /// 城市ID（选择城市用）
        /// </summary>
        public int? SelectCity { get; set; }
        /// <summary>
        /// 城市ID（筛选城市用）
        /// </summary>
        public List<int> CityCode { get; set; } = new List<int>();
        /// <summary>
        /// 选中区域类型
        /// </summary>
        public int AreaSelect { get; set; } = 1;
        /// <summary>
        /// 市区ID
        /// </summary>
        public List<int> AreaCodes { get; set; } = new List<int>();
        /// <summary>
        /// 选中地铁线路
        /// </summary>
        public Guid? MetroSelect { get; set; }
        /// <summary>
        /// 地铁线路ID
        /// </summary>
        public List<Guid> MetroLineIds { get; set; } = new List<Guid>();
        /// <summary>
        /// 地铁站点ID
        /// </summary>
        public List<int> MetroStationIds { get; set; } = new List<int>();
        /// <summary>
        /// 距离
        /// </summary>
        public decimal? Distance { get; set; }
        /// <summary>
        /// 1幼儿园 2小学 3初中 4高中
        /// </summary>
        public List<int> Grade { get; set; } = new List<int>();
        /// <summary>
        /// 选中年级
        /// </summary>
        public int? GradeSelect { get; set; }
        /// <summary>
        /// 学校类型
        /// </summary>
        public List<string> Type { get; set; } = new List<string>();
        /// <summary>
        /// 最低学费
        /// </summary>
        public int? MinCost { get; set; }
        /// <summary>
        /// 最高学费
        /// </summary>
        public int? MaxCost { get; set; }
        /// <summary>
        /// 最低学生人数
        /// </summary>
        public int? MinStudent { get; set; }
        /// <summary>
        /// 最高学生人数
        /// </summary>
        public int? MaxStudent { get; set; }
        /// <summary>
        /// 最低教师人数
        /// </summary>
        public int? MinTeacher { get; set; }
        /// <summary>
        /// 最高教师人数
        /// </summary>
        public int? MaxTeacher { get; set; }
        /// <summary>
        /// 最低综合评分
        /// </summary>
        public int? MinCScore { get; set; }
        /// <summary>
        /// 最高综合评分
        /// </summary>
        public int? MaxCScore { get; set; }
        /// <summary>
        /// 最低师资评分
        /// </summary>
        public int? MinTScore { get; set; }
        /// <summary>
        /// 最高师资评分
        /// </summary>
        public int? MaxTScore { get; set; }
        /// <summary>
        /// 最低硬件评分
        /// </summary>
        public int? MinHScore { get; set; }
        /// <summary>
        /// 最高硬件评分
        /// </summary>
        public int? MaxHScore { get; set; }
        /// <summary>
        /// 最低课程评分
        /// </summary>
        public int? MinCuScore { get; set; }
        /// <summary>
        /// 最高课程评分
        /// </summary>
        public int? MaxCuScore { get; set; }
        /// <summary>
        /// 最低学术评分
        /// </summary>
        public int? MinLScore { get; set; }
        /// <summary>
        /// 最高学术评分
        /// </summary>
        public int? MaxLScore { get; set; }
        /// <summary>
        /// 最低费用评分
        /// </summary>
        public int? MinCtScore { get; set; }
        /// <summary>
        /// 最高费用评分
        /// </summary>
        public int? MaxCtScore { get; set; }
        /// <summary>
        /// 最低总分评分
        /// </summary>
        public int? MinTotal { get; set; }
        /// <summary>
        /// 最高总分评分
        /// </summary>
        public int? MaxTotal { get; set; }
        /// <summary>
        /// 是否寄宿，1走读，2寄宿，3寄宿或走读
        /// </summary>
        public int? Lodging { get; set; }
        /// <summary>
        /// 是否有饭堂
        /// </summary>
        public bool? Canteen { get; set; }
        /// <summary>
        /// 学校认证
        /// </summary>
        public List<Guid> AuthIds { get; set; } = new List<Guid>();
        /// <summary>
        /// 特色课程
        /// </summary>
        public List<Guid> CharacIds { get; set; } = new List<Guid>();
        /// <summary>
        /// 出国方向
        /// </summary>
        public List<Guid> AbroadIds { get; set; } = new List<Guid>();
        /// <summary>
        /// 课程
        /// </summary>
        public List<Guid> CourseIds { get; set; } = new List<Guid>();
        /// <summary>
        /// 排序
        /// </summary>
        public int Orderby { get; set; }
        /// <summary>
        /// 页码
        /// </summary>
        public int PageNo { get; set; } = 1;
        /// <summary>
        /// 每页行数
        /// </summary>
        public int PageSize { get; set; } = 10;
    }
}
