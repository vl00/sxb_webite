using System;
namespace PMS.School.Domain.Common
{
    public class GisPoint
    {
        public GisPoint(double lon, double lat)
        {
            Longitude = lon;
            Latitude = lat;
        }

        /// <summary>
        /// 维度
        /// </summary>
        public double Latitude { get; }

        /// <summary>
        /// 经度
        /// </summary>
        public double Longitude { get; }

        public override string ToString()
        {
            return $"({Longitude},{Latitude})";
        }
    }
}
