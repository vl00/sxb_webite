//using Microsoft.SqlServer.Types;
//using System;
//using System.Collections.Generic;
//using System.Data;
//using System.Data.SqlClient;
//using System.IO;
//using System.Runtime.InteropServices;
//using System.Security;

//namespace iSchool
//{
//    /// <summary>
//    /// 经纬度
//    /// </summary>
//    public sealed partial class LngLatLocation
//    {
//        public LngLatLocation(double lng, double lat, int srid = 4326)
//        {
//            Lng = lng;
//            Lat = lat;
//            SRID = srid;
//        }

//        public double Lng { get; set; }
//        public double Lat { get; set; }
//        public int SRID { get; set; }

//        public static void Init_With_Dapper()
//        {
//            Dapper.SqlMapper.AddTypeHandler(typeof(LngLatLocation), new LngLatLocationTypeHandler());
//            //SqlServerTypes.Utilities.LoadNativeAssembly(Directory.GetCurrentDirectory(), "SqlServerSpatial140.dll");
//        }
//    }

//    public class LngLatLocationTypeHandler : Dapper.SqlMapper.ITypeHandler
//    {
//        public void SetValue(IDbDataParameter parameter, object value)
//        {
//            if (parameter is SqlParameter sqlParameter)
//            {
//                sqlParameter.UdtTypeName = "GEOGRAPHY";
//                parameter.Value = value == null ? (object)DBNull.Value :
//                    value is LngLatLocation location ? SqlGeography.Point(location.Lat, location.Lng, location.SRID) :
//                    throw new NotSupportedException();
//            }
//        }

//        public object Parse(Type destinationType, object value)
//        {
//            if (value == null || value is DBNull) return null;
//            if (destinationType == typeof(LngLatLocation))
//            {
//                if (value is SqlGeography geography) return new LngLatLocation(geography.Long.Value, geography.Lat.Value, geography.STSrid.Value);
//                return null; //DbGeography.FromText(value.ToString());
//            }
//            throw new NotSupportedException();
//        }
//    }

//    public sealed partial class LngLatLocation
//    {
//        /// <summary>
//        /// 2个经纬度的距离 - 烈嘉算法
//        /// </summary>
//        /// <param name="other"></param>
//        /// <returns>单位：米</returns>
//        public double DistanceByLiejia(LngLatLocation other)
//        {
//            if (Equals(other, null) || this.SRID != other.SRID)
//            {
//                throw new InvalidOperationException("srid is not same");
//            }
//            if (this.SRID != 4326)
//            {
//                throw new InvalidOperationException("srid is not 4326");
//            }

//            double rad(double d)
//            {
//                return d * Math.PI / 180.0;
//            }

//            double radLat1 = rad(this.Lat);
//            double radLat2 = rad(other.Lat);
//            double a = radLat1 - radLat2;
//            double b = rad(this.Lng) - rad(other.Lng);

//            double s = 2 * Math.Asin(Math.Sqrt(Math.Pow(Math.Sin(a / 2), 2) + Math.Cos(radLat1) * Math.Cos(radLat2) * Math.Pow(Math.Sin(b / 2), 2)));
//            s = s * SridList.GetEllipsoidParameters(this.SRID).semi_major;
//            s = Math.Round(s * 10000) / 10000;
//            return s;
//        }
//    }

//    public sealed partial class LngLatLocation
//    {
//        /// <summary>
//        /// 2个经纬度的距离 - sqlserver内部算法
//        /// </summary>
//        /// <param name="other"></param>
//        /// <returns>单位：米</returns>
//        [Obsolete("没找到支持linux的SqlServerSpatialXXX.dll")]
//        public double DistanceBySql(LngLatLocation other)
//        {
//            if (Equals(other, null) || this.SRID != other.SRID)
//            {
//                throw new InvalidOperationException("srid is not same");
//            }
//            return GeodeticPointDistance(new Point(this.Lat, this.Lng), new Point(other.Lat, other.Lng), SridList.GetEllipsoidParameters(this.SRID));
//        }

//        [DllImport("SqlServerSpatial140.dll", CharSet = CharSet.None, ExactSpelling = false)]
//        [SuppressUnmanagedCodeSecurity]
//        static extern double GeodeticPointDistance(in Point p1, in Point p2, in EllipsoidParameters ep);

//        [Serializable]
//        struct Point
//        {
//            public double x;
//            public double y;

//            public Point(double X, double Y)
//            {
//                this.x = X;
//                this.y = Y;
//            }
//        }

//        struct EllipsoidParameters
//        {
//            public double semi_major;
//            public double semi_minor;

//            public EllipsoidParameters(double major, double minor)
//            {
//                this.semi_major = major;
//                this.semi_minor = minor;
//            }

//            public double GetEccentricity()
//            {
//                return this.semi_minor / this.semi_major;
//            }

//            public double GetMaxCurvature()
//            {
//                double semiMinor = this.semi_minor;
//                return this.semi_major / (semiMinor * semiMinor);
//            }

//            public double GetMinCurvature()
//            {
//                double semiMajor = this.semi_major;
//                return this.semi_minor / (semiMajor * semiMajor);
//            }
//        }

//        class SridInfo
//        {
//            public int spatial_reference_id;
//            public string authority_name;
//            public int authorized_spatial_reference_id;
//            public string well_known_text;
//            public string unit_of_measure;
//            public double unit_conversion_factor;
//            public double semi_major_axis;
//            public double semi_minor_axis;

//            public SridInfo(int spatial_reference_id, string authority_name, int authorized_spatial_reference_id, string well_known_text, string unit_of_measure, double unit_conversion_factor, double semi_major_axis, double semi_minor_axis)
//            {
//                this.spatial_reference_id = spatial_reference_id;
//                this.authority_name = authority_name;
//                this.authorized_spatial_reference_id = authorized_spatial_reference_id;
//                this.well_known_text = well_known_text;
//                this.unit_of_measure = unit_of_measure;
//                this.unit_conversion_factor = unit_conversion_factor;
//                this.semi_major_axis = semi_major_axis;
//                this.semi_minor_axis = semi_minor_axis;
//            }
//        }

//        class SridList
//        {
//            private static SortedList<int, SridInfo> _sridList;

//            public static int Null => -1;

//            static SridList()
//            {
//                _sridList = new SortedList<int, SridInfo>()
//                {
//                    { 4326, new SridInfo(4326, "EPSG", 4326, "GEOGCS[\"WGS 84\", DATUM[\"World Geodetic System 1984\", ELLIPSOID[\"WGS 84\", 6378137, 298.257223563]], PRIMEM[\"Greenwich\", 0], UNIT[\"Degree\", 0.0174532925199433]]", "metre", 1, 6378137, 6356752.314) },
//                };
//            }

//            public SridList() { }

//            public static EllipsoidParameters GetEllipsoidParameters(int srid)
//            {
//                var item = _sridList[srid];
//                return new EllipsoidParameters(item.semi_major_axis, item.semi_minor_axis);
//            }
//        }
//    }
//}
