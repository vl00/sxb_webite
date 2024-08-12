using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sxb.Api.Model
{
    public class Article_V2
    {
        public Guid Id { get; set; }

        public DateTime Time { get; set; }

        public string Title { get; set; }

        public int ViewCount { get; set; }

        public List<string> Covers { get; set; }

        public int Layout { get; set; }

        public string Digest { get; set; }



        public int CommentCount { get; set; }

    }
}
