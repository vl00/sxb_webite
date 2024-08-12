using System;
using System.Collections.Generic;
using System.Text;

namespace iSchool.Internal.API.OperationModule.Models
{
    public class ResponseModel<T>
    {
        public int errCode { get; set; }
        public string msg { get; set; }
        public T data { get; set; }

        public bool IsOK()
        {
            return errCode == 1;
        }

    }
}
