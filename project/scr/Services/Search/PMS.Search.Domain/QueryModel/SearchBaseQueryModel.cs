using PMS.Search.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PMS.Search.Domain.QueryModel
{
    public interface ISearchSuggestion
    {
        List<string> Suggestions { get; set; }
    }
    public class SearchSuggestionBase : ISearchSuggestion
    {
        /// <summary>
        /// 搜索建议的词
        /// </summary>
        public List<string> Suggestions { get; set; }

        /// <summary>
        /// 兼容前端拼接字符串
        /// </summary>
        private string suggestionsStr;
        public string SuggestionsStr
        {
            get { return suggestionsStr; }
            set
            {
                suggestionsStr = value;
                if (value != null)
                {
                    Suggestions = value.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).ToList();
                }
            }
        }

        public string GetQueryEsKeyword(string keyword)
        {

            if (Suggestions != null && Suggestions.Count > 0)
            {
                var sugg = Suggestions.Where(s => !string.IsNullOrWhiteSpace(s));
                if (sugg.Any())
                {
                    return string.Join(" ", sugg);
                }
            }
            return keyword;
        }

        public List<string> GetQueryEsKeywords(string keyword)
        {
            if (Suggestions != null && Suggestions.Count > 0)
            {
                return Suggestions;
            }
            if (!string.IsNullOrWhiteSpace(keyword))
            {
                return new List<string>() { keyword };
            }
            return new List<string>();
        }
    }


    public class SearchBaseQueryModel : SearchSuggestionBase
    {
        public SearchBaseQueryModel(string keyword, int pageIndex, int pageSize)
        {
            Keyword = keyword;
            PageIndex = pageIndex;
            PageSize = pageSize;
        }

        public SearchBaseQueryModel()
        {
        }

        /// <summary>
        /// 输入的关键词
        /// </summary>
        public string Keyword { get; set; }
        /// <summary>
        /// 实际查询Es的关键词
        /// </summary>
        public string QueryEsKeyword => GetQueryEsKeyword(Keyword);
        public List<string> QueryEsKeywords => GetQueryEsKeywords(Keyword);

        public ArticleOrderBy OrderBy { get; set; }

        public int PageIndex { get; set; } = 1;

        public int PageSize { get; set; } = 10;

        /// <summary>
        /// 兼容旧
        /// </summary>
        public int PageNo { get { return PageIndex; } set { PageIndex = value; } }

        public int CityCode { get; set; }
        public bool MustCityCode { get; set; }

        /// <summary>
        /// 所有字都必须命中
        /// </summary>
        public bool IsMatchAllWord { get; set; }
    }
}
