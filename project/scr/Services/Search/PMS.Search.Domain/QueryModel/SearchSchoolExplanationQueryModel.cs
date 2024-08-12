using System;
using System.Collections.Generic;
using System.Text;

namespace PMS.Search.Domain.QueryModel
{
    public class SearchSchoolExplanationQueryModel
    {
        public Guid Id {  get; set; }

        public string Name {  get; set; }

        public double Score { get; set; }
        /// <summary>
        /// 评分详情
        /// </summary>
        public object Explanations {  get; set; }
    }
}
