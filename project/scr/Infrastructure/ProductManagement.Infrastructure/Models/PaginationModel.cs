using ProductManagement.Framework.Foundation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ProductManagement.Infrastructure.Models
{
    public class PaginationModel
    {
        public static PaginationModel<T> Build<T>(List<T> data, long total)
        {
            return new PaginationModel<T>()
            {
                Data = data,
                Total = total
            };
        }

        public static PaginationModel<T> Build<T>(IEnumerable<T> data, long total)
        {
            return new PaginationModel<T>()
            {
                Data = data.ToList(),
                Total = total
            };
        }
        public static PaginationModel<TDestination> MapperTo<TSource, TDestination>(PaginationModel<TSource> source)
            where TSource : class
            where TDestination : class
        {
            return new PaginationModel<TDestination>()
            {
                Data = CommonHelper.MapperProperty<TSource, TDestination>(source.Data).ToList(),
                Total = source.Total
            };
        }
    }


    public class PaginationModel<T>
    {
        public long Total { get; set; }

        public List<T> Data { get; set; }

        [Obsolete(nameof(PaginationModelExt<T>))]
        public dynamic Statistics { get; set; }

        public static PaginationModel<T> Build(List<T> data, long total)
        {
            return new PaginationModel<T>()
            {
                Data = data,
                Total = total
            };
        }

        public static PaginationModel<T> Build(IEnumerable<T> data, long total)
        {
            return new PaginationModel<T>()
            {
                Data = data.ToList(),
                Total = total
            };
        }
    }


    public class PaginationModelExt<T> : PaginationModel<T>
    {
        public dynamic Statistics { get; set; }
    }
}
