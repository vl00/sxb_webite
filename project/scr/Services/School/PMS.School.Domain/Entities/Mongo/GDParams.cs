using MongoDB.Bson.Serialization.Attributes;
using ProductManagement.Framework.Foundation;

namespace PMS.School.Domain.Entities.Mongo
{
    [BsonIgnoreExtraElements]
    public class GDParams
    {
        public string SchFtype { get; set; }
        public int area { get; set; }
        public string eid { get; set; }

        double? _buildingprice;
        /// <summary>
        /// 房价
        /// </summary>
        public double? buildingprice
        {
            get { return _buildingprice?.CutDoubleWithN(0) ?? 0; }
            set { _buildingprice = value; }
        }
        public string BuildingPriceString
        {
            get
            {
                if (_buildingprice.HasValue)
                {
                    return ((int)_buildingprice).ToString();
                }
                else
                {
                    return "0";
                }
            }
        }
        double? _toptraininfo;
        /// <summary>
        /// 头部教育机构
        /// </summary>
        public double? toptraininfo
        {
            get
            {
                return _toptraininfo?.CutDoubleWithN(2) ?? 0;
            }
            set
            {
                _toptraininfo = value;
            }
        }
        double? _shoppinginfo;
        /// <summary>
        /// 购物商场
        /// </summary>
        public double? shoppinginfo { get { return _shoppinginfo?.CutDoubleWithN(2) ?? 0; } set { _shoppinginfo = value; } }
        double? _traininfo;
        /// <summary>
        /// 培训机构
        /// </summary>
        public double? traininfo { get { return _traininfo?.CutDoubleWithN(2) ?? 0; } set { _traininfo = value; } }
        double? _metro;
        /// <summary>
        /// 地铁站
        /// </summary>
        public double? metro { get { return _metro?.CutDoubleWithN(2) ?? 0; } set { _metro = value; } }
        double? _bus;
        /// <summary>
        /// 公交站
        /// </summary>
        public double? bus { get { return _bus?.CutDoubleWithN(2) ?? 0; } set { _bus = value; } }
        double? _hospital;
        /// <summary>
        /// 医院
        /// </summary>
        public double? hospital { get { return _hospital?.CutDoubleWithN(2) ?? 0; } set { _hospital = value; } }
        double? _police;
        /// <summary>
        /// 警察局
        /// </summary>
        public double? police { get { return _police?.CutDoubleWithN(2) ?? 0; } set { _police = value; } }
        double? _bookmarket;
        /// <summary>
        /// 书店
        /// </summary>
        public double? bookmarket { get { return _bookmarket?.CutDoubleWithN(2) ?? 0; } set { _bookmarket = value; } }
        double? _museum;
        /// <summary>
        /// 博物馆
        /// </summary>
        public double? museum { get { return _museum?.CutDoubleWithN(2) ?? 0; } set { _museum = value; } }
        private double? _library;
        /// <summary>
        /// 图书馆
        /// </summary>
        public double? library
        {
            get { return _library?.CutDoubleWithN(2) ?? 0; }
            set { _library = value; }
        }
    }
}
