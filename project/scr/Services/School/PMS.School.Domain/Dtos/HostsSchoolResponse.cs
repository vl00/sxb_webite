using System;
using System.Collections.Generic;

namespace PMS.School.Domain.Dtos
{
    public class HostsSchoolResponse<T>
    {
        public int Status { get; set; }
        public string ErrorDescription { get; set; }

        public T Items { get; set; }

        public PageInfo PageInfo { get; set; }
    }

    public class PageInfo
    {
        public int Curpage { get; set; }
        public int TotalPage { get; set; }
        public int CountPerpage { get; set; }
        public int TotalCount { get; set; }
    }


    public class ExtIdItem
    {
        public int Grade { get; set; }
        public List<Guid> Ids { get; set; }
    }
}
