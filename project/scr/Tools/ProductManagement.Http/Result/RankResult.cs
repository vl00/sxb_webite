using System;
using System.Collections.Generic;

namespace ProductManagement.API.Http.Result
{
    public class RankResult
    {
        public int ErrCode { get; set; }
        public string Msg { get; set; }
        public DataClass Data { get; set; }


        public class DataClass
        {
            public int Total { get; set; }
            public int Offset { get; set; }
            public int Limit { get; set; }
            public List<Row> Rows { get; set; }
        }

        public class Row
        {
            public Guid Id { get; set; }
            public string Title { get; set; }
            public int Sort { get; set; }
            public bool IsView { get; set; }
            public DateTime UpdateTime { get; set; }
            public bool IsToTop { get; set; }
            public List<School> BindSchools { get; set; }
        }
        public class School
        {
            public Guid Id { get; set; }
            public string Name { get; set; }
            public int Sort { get; set; }
        }
    }
}
