using System;
using System.Collections.Generic;
using System.Text;

namespace PMS.UserManage.Domain.Entities
{
    
    public class Feedback
    {
        public Guid Id { get; set; }
        public Guid UserID { get; set; }
        public byte type { get; set; }
        public string text { get; set; }
        public DateTime time { get; set; }
    }

    public class Feedback_Img
    {
        public Guid Id { get; set; }
        public Guid Feedback_Id { get; set; }
        public string url { get; set; }
    }

    public enum CorrectedType 
    {
         /// <summary>
         /// 学校名称
         /// </summary>
         Name = 1,
         /// <summary>
         /// 位置信息
         /// </summary>
         Location = 2,
         /// <summary>
         /// 学校类型
         /// </summary>
         SchType = 3,
         /// <summary>
         /// 其他
         /// </summary>
         Other = 4
    }

    //学校纠错 详情信息
    public class Feedback_corrected 
    {
        public Guid Id { get; set; }
        public CorrectedType Type { get; set; }
        public Guid Feedback_Id { get; set; }
        public string School_Id { get; set; }
        public string School_name { get; set; }
        public string Before_Corrected { get; set; }
        public string After_Corrected { get; set; }
    }

    
}
