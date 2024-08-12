using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq.Expressions;

namespace ProductManagement.Framework.SearchAccessor
{
    /// <summary>
    /// 查询条件
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class QueryCondition<T> where T : class, new()
    {
        #region 中间结果
        string queryId;

        readonly List<TermUnit<T>> termList;

        bool _isMatchAll;
        #endregion

        #region 结果读取
        [EditorBrowsable(EditorBrowsableState.Never)]
        public string QueryId => queryId;

        [EditorBrowsable(EditorBrowsableState.Never)]
        public List<TermUnit<T>> TermList => termList;

        [EditorBrowsable(EditorBrowsableState.Never)]
        public bool IsMatchAll => _isMatchAll;

        #endregion

        public QueryCondition()
        {
            queryId = null;
            _isMatchAll = false;
            termList = new List<TermUnit<T>>();
        }

        #region 链式方法
        public QueryCondition<T> Id(string id)
        {
            queryId = id;
            return this;
        }

        public QueryCondition<T> Match(Expression<Func<T, object>> fieldDesc, string value, double boost = 1)
        {
            termList.Add(new TermUnit<T>() { CompareType = CompareType.Match, FieldDesc = fieldDesc, ReferValue = value, Boost = boost });
            return this;
        }

        public QueryCondition<T> NonMatch(Expression<Func<T, object>> fieldDesc, string value, double boost = 1)
        {
            termList.Add(new TermUnit<T>() { CompareType = CompareType.NonMatch, FieldDesc = fieldDesc, ReferValue = value, Boost = boost });
            return this;
        }

        public QueryCondition<T> MatchAll()
        {
            _isMatchAll = true;
            return this;
        }
        #endregion

        #region 隐藏基类成员
        [EditorBrowsable(EditorBrowsableState.Never)]
        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public override string ToString()
        {
            return base.ToString();
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public new Type GetType()
        {
            return base.GetType();
        }
        #endregion
    }

    public class TermUnit<T> where T : class, new()
    {
        /// <summary>
        /// 指定成员。
        /// </summary>
        public Expression<Func<T, object>> FieldDesc;

        /// <summary>
        /// 指定参数。
        /// </summary>
        public string ReferValue;

        /// <summary>
        /// 指定方式。
        /// </summary>
        public CompareType CompareType;

        /// <summary>
        /// 指定权重。
        /// </summary>
        public double Boost = 1;
    }

    public enum CompareType
    {
        Match = 1,
        NonMatch = 2,
    }
}