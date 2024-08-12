using System;
using System.Collections.Generic;

namespace Sxb.Web.Areas.SignUp.Models
{
    public class SaleSignData
    {
        public string Phone { get; set; }
        public string Verifycode { get; set; }

        public DateTime SignDate { get; set; }
        public string Name { get; set; }
        public string Grade { get; set; }
        public int AgreeShare { get; set; }

        public List<List<int>> Choices { get; set; } = new List<List<int>>();

        public string Other { get; set; }
    }
}
