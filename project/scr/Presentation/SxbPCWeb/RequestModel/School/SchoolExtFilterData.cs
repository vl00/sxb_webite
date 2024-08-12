using System;
using System.Collections.Generic;

namespace Sxb.PCWeb.RequestModel.School
{
    public class SchoolExtFilterData
    {
        //搜索关键字
        public string KeyWords { get; set; }

        //城市ID（选择城市用）
        public int? SelectCity { get; set; }

        //城市ID（筛选城市用）
        public List<int> CityCode { get; set; } = new List<int>();
        //选中区域类型
        public int AreaSelect { get; set; } = 1;
        //市区ID
        public List<int> AreaCodes { get; set; } = new List<int>();
        //选中地铁线路
        public Guid? MetroSelect { get; set; }
        //地铁线路ID
        public List<Guid> MetroLineIds { get; set; } = new List<Guid>();
        //地铁站点ID
        public List<int> MetroStationIds { get; set; } = new List<int>();
        //距离
        public decimal? Distance { get; set; }
        //1幼儿园 2小学 3初中 4高中
        public List<int> Grade { get; set; } = new List<int>();
        //选中年级
        public int? GradeSelect { get; set; }
        //学校类型
        public List<string> Type { get; set; } = new List<string>();


        //最低学费
        public int? MinCost { get; set; }
        //最高学费
        public int? MaxCost { get; set; }

        //最低学生人数
        public int? MinStudent { get; set; }
        //最高学生人数
        public int? MaxStudent { get; set; }

        //最低教师人数
        public int? MinTeacher { get; set; }
        //最高教师人数
        public int? MaxTeacher { get; set; }

        //最低综合评分
        public int? MinCScore { get; set; }
        //最高综合评分
        public int? MaxCScore { get; set; }

        //最低师资评分
        public int? MinTScore { get; set; }
        //最高师资评分
        public int? MaxTScore { get; set; }

        //最低硬件评分
        public int? MinHScore { get; set; }
        //最高硬件评分
        public int? MaxHScore { get; set; }

        //最低课程评分
        public int? MinCuScore { get; set; }
        //最高课程评分
        public int? MaxCuScore { get; set; }

        //最低学术评分
        public int? MinLScore { get; set; }
        //最高学术评分
        public int? MaxLScore { get; set; }

        //最低费用评分
        public int? MinCtScore { get; set; }
        //最高费用评分
        public int? MaxCtScore { get; set; }


        //最低总分评分
        public int? MinTotal { get; set; }
        //最高总分评分
        public int? MaxTotal { get; set; }

        /// <summary>
        /// 是否寄宿
        /// </summary>
        public bool? Lodging { get; set; }
        /// <summary>
        /// 是否有饭堂
        /// </summary>
        public bool? Canteen { get; set; }
        //学校认证
        public List<Guid> AuthIds { get; set; } = new List<Guid>();
        //特色课程
        public List<Guid> CharacIds { get; set; } = new List<Guid>();
        //出国方向
        public List<Guid> AbroadIds { get; set; } = new List<Guid>();
        //课程
        public List<Guid> CourseIds { get; set; } = new List<Guid>();


        //排序
        public int Orderby { get; set; }

        public int PageNo { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}
