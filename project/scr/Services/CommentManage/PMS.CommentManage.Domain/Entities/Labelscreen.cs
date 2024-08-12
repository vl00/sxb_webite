using PMS.CommentsManage.Domain.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace PMS.CommentsManage.Domain.Entities
{
    /// <summary>
    /// 学校点评、问答页 标签筛选
    /// </summary>
    public class Labelscreen
    {
        /*
         (
	        Id int identity(1,1) primary key,
	        Label varchar(30) not null,
	        LabelType int not null,
	        Prohibit bit not null
         */
        public int Id { get; set; }
        public string Label { get; set; }
        public LabelType LabelType { get; set; }
        public bool Prohibit { get; set; }
    }
}
