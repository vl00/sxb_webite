using Nest;
using PMS.Search.Domain.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace PMS.Search.Domain.Entities
{
    public class SearchPUV
    {
        public virtual long Pv { get; set; }

        public virtual long Uv { get; set; }

        public static Func<SortDescriptor<T>, IPromise<IList<ISort>>> GetSortDesc<T>(ArticleOrderBy orderBy, Func<Func<SortDescriptor<T>, IPromise<IList<ISort>>>> def, string orderTimeField = "updateTime")
            where T : SearchPUV
        {
            Func<SortDescriptor<T>, IPromise<IList<ISort>>> sort;
            switch (orderBy)
            {
                case ArticleOrderBy.HotDesc:
                    sort = s => s.Descending(a => a.Uv);
                    break;
                case ArticleOrderBy.HotAsc:
                    sort = s => s.Ascending(a => a.Uv);
                    break;
                case ArticleOrderBy.TimeDesc:
                    sort = s => s.Descending(Infer.Field(orderTimeField));
                    break;
                case ArticleOrderBy.TimeAsc:
                    sort = s => s.Ascending(Infer.Field(orderTimeField));
                    break;
                case ArticleOrderBy.Default:
                default:
                    sort = def.Invoke();
                    break;
            }
            return sort;
        }

        public static Func<SortDescriptor<T>, IPromise<IList<ISort>>> GetSortDesc<T>(ArticleOrderBy orderBy, string orderTimeField = "updateTime")
            where T : SearchPUV
        {
            return GetSortDesc<T>(orderBy, () => s => s.Descending(SortSpecialField.Score), orderTimeField);
        }
    }
}
