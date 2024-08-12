using System;
using System.Collections.Generic;
using System.Text;

namespace iSchool.Internal.API.UserModule.Models
{
    public class ResponseModel
    {
        public int status { get; set; }
        public string errorDescription { get; set; }
        public string ReturnUrl { get; set; }

        public bool IsOK()
        {
            return status == 0;
        }
    }
}
