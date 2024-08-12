using System;
using System.Collections.Generic;
using System.Text;

namespace PMS.Infrastructure.Application.ModelDto
{
    /// <summary>
    /// 城市拼音模糊搜索
    /// </summary>
    public class CitySearchDto
    {
        public string Name { get; set; }
        public int CityCode { get; set; }
        public List<string> Pinyin { get; set; }
    }
}
