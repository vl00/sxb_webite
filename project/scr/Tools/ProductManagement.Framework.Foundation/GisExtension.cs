using System;
namespace ProductManagement.Framework.Foundation
{
    public static class GisExtension
    {
        /// <summary>
        /// 获取两点之间的距离
        /// 单位 km
        /// </summary>
        /// <param name="latitude1"></param>
        /// <param name="longitude1"></param>
        /// <param name="latitude2"></param>
        /// <param name="longitude2"></param>
        /// <returns></returns>
        public static double GetDistance(double latitude1, double longitude1, double latitude2, double longitude2)
        {
            double radLat1 = Rad(latitude1);
            double radLat2 = Rad(latitude2);
            double a = radLat1 - radLat2;
            double b = Rad(longitude1) - Rad(longitude2);
            double s = 2 * Math.Asin(Math.Sqrt(Math.Pow(Math.Sin(a / 2), 2) +
            Math.Cos(radLat1) * Math.Cos(radLat2) * Math.Pow(Math.Sin(b / 2), 2)));
            s = s * EARTH_RADIUS;
            s = Math.Round(s * 10000) / 10000;
            return s;
        }
        private const double EARTH_RADIUS = 6378.137;
        private static double Rad(double d)
        {
            return d * Math.PI / 180.0;
        }
    }
}
