using PMS.OperationPlateform.Application.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sxb.PCWeb.ViewModels.Common
{
    public class AdvResponseModel
    {
        public int status { get; set; }

        public string msg { get; set; }

        public List<AdvOption> Advs { get; set; }

        public class AdvOption
        {
            public int Place { get; set; }

            public List<AdvertisingBaseGetAdvertisingResultDto> Items { get; set; }
        }

        //public class AdvBase
        //{
        //    public int Sort { get; set; }

        //    /// <summary>
        //    /// 图片地址
        //    /// </summary>
        //    public string PicUrl { get; set; }

        //    /// <summary>
        //    /// 标题
        //    /// </summary>
        //    public string SloGan { get; set; }

        //    public string Url { get; set; }

        //    public double Rate { get; set; }
        //}
    }
}