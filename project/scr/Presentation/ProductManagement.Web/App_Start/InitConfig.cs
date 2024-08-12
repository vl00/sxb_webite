using AutoMapper;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;

namespace ProductManagement.Web.App_Start
{
    public static class InitConfig
    {
        public static void Init()
        {

            /* 对象映射 */
            Mapper.Initialize(x => {
                x.AddProfile<VoMapperConfiguration>();
            });
        }
    }
}
