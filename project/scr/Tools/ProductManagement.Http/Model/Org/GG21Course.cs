using System;
using System.Collections.Generic;
using System.Text;

namespace ProductManagement.API.Http.Model.Org
{
    public class GG21Course
    {
            public string id { get; set; }
            public string id_s { get; set; }
            public string orgName { get; set; }
            public string title { get; set; }
            public string subtitle { get; set; }
            public string banner { get; set; }
            public float price { get; set; }
            public object origPrice { get; set; }
            public bool authentication { get; set; }
            public int sellcount { get; set; }
            public bool isExplosions { get; set; }
            public string pcUrl { get; set; }
            public string mUrl { get; set; }
            public object mpQrcode { get; set; }
    }
}
