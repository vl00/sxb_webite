using System;

namespace ProductManagement.API.Http.Model.FinanceCenter
{

    public class PayCenterResponseResult<T>
    {
        public bool succeed { get; set; }
        public int status { get; set; }
        public string msg { get; set; }
        public T data { get; set; }
    }

 

}
