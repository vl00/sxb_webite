using System;

namespace PMS.Live.Domain.Entities
{
    [Serializable]
    public class LectureInfo
    {
        public Guid Id { get; set; }
        public string Subject { get; set; }
        public string Intro { get; set; }
        public Guid Lector_id { get; set; }
        public DateTime Time_start { get; set; }
        public int UserCount { get; set; }
        public int BookCount { get; set; }
        public bool Show { get; set; }
        public int Level { get; set; }
        public string Auth { get; set; }
        public decimal Fee { get; set; }
        public int Papa { get; set; }
        public int Status { get; set; }
        public bool Stick { get; set; }
        public int Type { get; set; }
        public string Code { get; set; }
        public DateTime Createtime { get; set; }
        public float Weight { get; set; }
        public DateTime Stickstart { get; set; }
        public DateTime Stickend { get; set; }

        public Guid? Activtiy_id { get; set; }
        public bool Must_activity { get; set; }

        public int? Province { get; set; }
        public int? City { get; set; }
        public int? Area { get; set; }
        public Guid? Series_id { get; set; }

        //群引导
        public string group_qrcode_detail { get; set; }
        public string group_qrcode_living { get; set; }

        //首页封面
        public string Cover_home { get; set; }
        //详情页封面
        public string Cover_detail { get; set; }
    }
}
