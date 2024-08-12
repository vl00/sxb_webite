using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace PMS.School.Infrastructure.Common
{
   
    public class SmallLocation
    {
        private double longitude;
        private double latitude;
        private string name;

        /// <summary>
        /// 经度
        /// </summary>
        public double Longitude
        {
            get { return longitude; }
            set { longitude = value; }
        }

        /// <summary>
        /// 纬度
        /// </summary>
        public double Latitude
        {
            get { return latitude; }
            set { latitude = value; }
        }
        /// <summary>
        /// 地址
        /// </summary>
        public string Name
        {
            get { return name; }
            set { name = value; }
        }
        public int state { get; set; } = 0;
        public static bool operator ==(SmallLocation lhs, SmallLocation rhs)
        {

            return lhs.Latitude == rhs.Latitude && lhs.Longitude == rhs.Longitude;
        }

        public static bool operator !=(SmallLocation lhs, SmallLocation rhs)
        {
            return !(lhs == rhs);
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }
            if ((obj.GetType().Equals(this.GetType())) == false)
            {
                return false;
            }
            SmallLocation temp = null;
            temp = (SmallLocation)obj;
            return this.Latitude.Equals(temp.Latitude) && this.Longitude.Equals(temp.Longitude);
        }

        public override int GetHashCode()
        {
            return this.Latitude.GetHashCode() + this.Longitude.GetHashCode();
        }
    }
    /// <summary>
    /// 求包围所有点的凸边形
    /// </summary>
    public class PolygonHelper
    {
        private List<SmallLocation> list;
        private SmallLocation first = new SmallLocation();
        private SmallLocation next = new SmallLocation();
        private SmallLocation pre = new SmallLocation();
        private int maxPoints = 0;

        public PolygonHelper(List<SmallLocation> _list)
        {
            list = _list;
            first = list.OrderByDescending(x => x.Latitude).FirstOrDefault();
        }

        public List<SmallLocation> GetPolygon()
        {
            List<SmallLocation> result = new List<SmallLocation>();

            while (!next.Equals(first))
            {
                var c = GetNextPoint();
                if (c.Longitude == 0 && c.Latitude == 0)
                {
                    break;
                }
                result.Add(c);


                maxPoints++;
                if (maxPoints > 1000)//防逻辑错误死循环
                {
                    break;
                }
            }
            return result;
        }



        private SmallLocation GetNextPoint()
        {

            if (next.Longitude == 0 && next.Latitude == 0)
            {
                next = first;

            }
            foreach (SmallLocation location in list)
            {
                if (!next.Equals(location)&&!pre.Equals(location))
                {
                    if (AtOneSide(next, location))
                    {
                        pre = next;
                        next = location;
                        return location;
                    }
                }

            }
           
            return new SmallLocation();
        }


        private bool AtOneSide(SmallLocation l1, SmallLocation l2)
        {
            var r = true;
            var count = 0;
            var direction = 0;
            double k = (l1.Latitude - l2.Latitude) / (l1.Longitude - l2.Longitude);
            double b = l1.Latitude - l1.Longitude * k;
            foreach (SmallLocation l in list)
            {
                count++;
                //如果斜率为0，处理
                if (0 == k)
                {
                    if (!l.Equals(l1) && !l.Equals(l2))
                    {
                        if (1 == count)
                        {
                            if (l.Longitude > l1.Longitude)
                            {

                                direction = 1;
                            }

                        }
                        if (1 == direction)
                        {
                            if (l.Longitude < l1.Longitude)
                            {
                                r = false;
                                break;
                            }
                        }
                        else
                        {
                            if (l.Longitude > l1.Longitude)
                            {
                                r = false;
                                break;
                            }

                        }

                    }
                }
                else
                {

                    if (!l.Equals(l1) && !l.Equals(l2))
                    {
                        if (1 == count)
                        {
                            if ((l.Latitude > (k * l.Longitude + b)))
                            {

                                direction = 1;
                            }

                        }
                        if (1 == direction)
                        {
                            if (l.Latitude < (k * l.Longitude + b))
                            {
                                r = false;
                                break;
                            }
                        }
                        else
                        {
                            if (l.Latitude > (k * l.Longitude + b))
                            {
                                r = false;
                                break;
                            }

                        }

                    }
                }



            }

            return r;
        }


    }
}
