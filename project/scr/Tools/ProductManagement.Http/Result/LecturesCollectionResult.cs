using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ProductManagement.API.Http.Result
{
    public class LecturesCollectionResult
    {
        public int Status { get; set; }
        public string ErrorDescription { get; set; }

        public List<LectureCItem> Items { get; set; }

        public PageinfoClass pageinfo { get; set; } 
    }


    public class LectureCItem
    {
        public LectureCCItem Lecture { get; set; }
        public DateTime Time { get; set; }
        public bool IsBooking { get; set; }
    }

    public class LectureCCItem
    {
        public LectorClass lector { get; set; }
        public Guid Id { get; set; }
        public string Subject { get; set; }
        public int Status { get; set; }

        public object Time { get; set; }
        public string Intro { get; set; }
        public int Appointcount { get; set; }
        public int Onlinecount { get; set; }
        public object Lefttime { get; set; }
        public string cover { get; set; }
        public bool? isbooking { get; set; }
        public bool? isjoin { get; set; }
        public bool? ispermission { get; set; }
        public bool? isadmin { get; set; }
        public decimal fee { get; set; }
        public int? type { get; set; }
        public int collectioncount { get; set; }
        public int userCount { get; set; }
        public int? level { get; set; }
        public decimal? weight { get; set; }
        public string stickstart { get; set; }
        public string stickend { get; set; }
        public bool? stickstatus { get; set; }
        public object CreateTime { get; set; }
        public string Code { get; set; }
        public string lectorname { get; set; }
        public Guid? activityid { get; set; }
        public string activityTitle { get; set; }
        public Guid? seriesid { get; set; }
        public bool? isfocusgzh { get; set; }
        public string Auth { get; set; }
        public string DetailQRCode { get; set; }
        public string LivingQRCode { get; set; }
        public string DetailGroupIntro { get; set; }
        public string LivingGroupIntro { get; set; }
        public bool? QRStartSend { get; set; }
        public bool? QREndSend { get; set; }
        public string HomeCover { get; set; }
        public string DetailCover { get; set; }
        public bool? Stick { get; set; }
        public int? province { get; set; }
        public int? city { get; set; }
        public int? area { get; set; }
        public bool? HasInviter { get; set; }
        public string IntroImages { get; set; }
        public bool? must_activity { get; set; }
        public int enable_activity_status { get; set; }
    }
}

