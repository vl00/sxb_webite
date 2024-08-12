using System;
using System.Collections.Generic;
using System.Text;

namespace PMS.UserManage.Application.ModelDto
{
    public class ModelSort<T> where T : class
    {
        public int Score { get; set; }

        public T Item { get; set; }

    }
}
