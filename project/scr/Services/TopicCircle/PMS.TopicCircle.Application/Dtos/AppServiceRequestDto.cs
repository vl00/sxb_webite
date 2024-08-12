using System;
using System.Collections.Generic;
using System.Text;

namespace PMS.TopicCircle.Application.Dtos
{

    public class AppServicePageRequestDto<TRequest>
    {
        public int offset { get; set; }

        public int limit { get; set; }

        public TRequest input { get; set; }


    }

}
