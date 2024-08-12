using System;
namespace PMS.Infrastructure.Application.ModelDto
{
    public class AreaPolylineDto
    {
        public string Name { get; set; }

        public int Code { get; set; }
        /// <summary>
        /// 维度
        /// </summary>
        public double Latitude { get; set; }

        /// <summary>
        /// 经度
        /// </summary>
        public double Longitude { get; set; }
    }
}
