using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ProductManagement.API.Http.Result
{
    public class OperationResult<T>
    {
        public T Data { get; set; }
        public bool success { get; set; }

        public OperationCode status { get; set; }

        public string erroMsg { get; set; }
    }

    public enum OperationCode
    {
        success = 1001,
        fail = 1002,
    }
}

