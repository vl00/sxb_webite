using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace ProductManagement.Framework.SearchAccessor
{
    public class QueryDesc<T> where T : class, new()
    {
        #region 中间结果
        string queryId;

        List<TermUnit<T>> termList;

        bool isMatchAll;

        int querySkip;

        int queryTake;
        #endregion

        #region 结果读取
        public string QueryId => queryId;

        public List<TermUnit<T>> TermList => termList;

        public bool IsMatchAll => isMatchAll;

        public int QuerySkip => querySkip;

        public int QueryTake => queryTake;

        #endregion

        public QueryDesc() : base()
        {
            queryId = null;
            termList = new List<TermUnit<T>>();
            isMatchAll = false;
            querySkip = 0;
            queryTake = 0;
        }

        #region 链式方法
        public QueryDesc<T> Id(string id)
        {
            queryId = id;
            return this;
        }

        public QueryDesc<T> Match(Expression<Func<T, object>> fieldDesc, string value, double boost = 1)
        {
            termList.Add(new TermUnit<T>() { CompareType = CompareType.Match, FieldDesc = fieldDesc, ReferValue = value, Boost = boost });
            return this;
        }

        public QueryDesc<T> NonMatch(Expression<Func<T, object>> fieldDesc, string value, double boost = 1)
        {
            termList.Add(new TermUnit<T>() { CompareType = CompareType.NonMatch, FieldDesc = fieldDesc, ReferValue = value, Boost = boost });
            return this;
        }


        public QueryDesc<T> MatchAll()
        {
            isMatchAll = true;
            return this;
        }

        public QueryDesc<T> Skip(int skip)
        {
            querySkip = skip;
            return this;
        }

        public QueryDesc<T> Take(int take)
        {
            queryTake = take;
            return this;
        }
        #endregion
    }
}
