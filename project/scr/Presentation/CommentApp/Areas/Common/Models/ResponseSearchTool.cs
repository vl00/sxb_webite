using PMS.Infrastructure.Application.ModelDto;
using PMS.School.Application.ModelDto;
using Sxb.Web.Areas.Common.Models.Search;
using Sxb.Web.ViewModels.School;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static PMS.Search.Domain.Common.GlobalEnum;

namespace Sxb.Web.Areas.Common.Models
{
    /// <summary>
    /// 搜索筛选列表页数据
    /// </summary>
    public class ResponseSearchTool
    {
        public ChannelIndex Channel { get; set; }

        public int CityCode { get; set; }

        /// <summary>
        /// 检测当前用户身份 （true：校方用户 |false：普通用户）
        /// </summary>
        public bool IsSchoolRole { get; set; }

        /// <summary>
        /// 区域 - 城区
        /// </summary>
        public List<AreaDto> Areas { get; set; }
        /// <summary>
        /// 区域 - 地铁
        /// </summary>
        public List<MetroDto> Metros { get; set; }
        /// <summary>
        /// 类型
        /// </summary>
        public List<SchoolGradeDetailVM> Types { get; set; }
        /// <summary>
        /// 筛选 - 学校认证
        /// </summary>
        public List<object> Authentications { get; set; }
        /// <summary>
        /// 筛选 - 出国方向
        /// </summary>
        public List<object> Abroads { get; set; }
        /// <summary>
        /// 筛选 - 特色课程或项目
        /// </summary>
        public List<object> Characteristics { get; set; }
        /// <summary>
        /// 诗选 - 课程设置
        /// </summary>
        public List<object> Courses { get; set; }

        /// <summary>
        /// 学校评分  A+ A B C D
        /// </summary>
        public string[] SchoolScores { get; set; }

        /// <summary>
        /// 价格点击比例
        /// </summary>
        public IEnumerable<CostSectionDto> Costs { get; set; }
    }


    /// <summary>
    /// PC搜索筛选列表页数据
    /// </summary>
    public class ResponsePCSearchTool : ResponseSearchTool
    {
    }
}
