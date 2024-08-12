using MongoDB.Bson.Serialization.Attributes;
using ProductManagement.Framework.Foundation;

namespace PMS.School.Domain.Entities.Mongo
{
    [BsonIgnoreExtraElements]
    public class AiParams
    {
        public string eid { get; set; }
        public string SchFtype { get; set; }
        public int areacode { get; set; }
        /// <summary>
        /// 学校总评分
        /// </summary>
        public int totalscore { get; set; }
        public string Str_Score
        {
            get
            {
                return ((double)totalscore).GetScoreString();
            }
        }
        /// <summary>
        /// 学生人数
        /// </summary>
        public int student_count { get; set; }

        double _avgcommentscore;
        /// <summary>
        /// 家长点评分
        /// </summary>
        public double avgcommentscore
        {
            get
            {
                return (_avgcommentscore/20).CutDoubleWithN(1);
            }
            set
            {
                _avgcommentscore = value;
            }
        }
        double _hot;
        /// <summary>
        /// 学校评论数
        /// </summary>
        public double hot
        {
            get
            {
                return _hot.CutDoubleWithN(2);
            }
            set
            {
                _hot = value;
            }
        }
        double _searchhot;
        /// <summary>
        /// 百度指数
        /// </summary>
        public double searchhot
        {
            get
            {
                return _searchhot.CutDoubleWithN(2);
            }
            set
            {
                _searchhot = value;
            }
        }
    }
}
