using System;
using System.Collections.Generic;
using System.Text;

namespace PMS.Infrastructure.Domain.Dtos
{
    public class WeixinSubscribePvDto
    {
        public string FromWhere { get; set; }

        public DateTime Date { get; set; }

        public int Pv { get; set; }

        public int Uv { get; set; }
    }

}
