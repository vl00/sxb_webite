using System;
using System.Collections.Generic;
using System.Text;

namespace PMS.Infrastructure.Application.ModelDto
{
    public class IsCollectedDto
    {
        public int status { get; set; }
        public bool iscollected { get; set; }
        public Guid userID { get; set; }
    }
}
