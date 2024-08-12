using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Nest;

namespace ProductManagement.Framework.SearchAccessor
{
    /// <summary>
    /// 全文搜索接口。
    /// </summary>
    public interface ISearch
    {
        ElasticClient GetClient();
    }
}