using Dapper.Contrib.Extensions;
using System;
using System.Collections.Generic; 
using System.Data;
using System.Text;

namespace PMS.School.Domain.Entities
{
    /// <summary>
    /// 
    /// </summary>
    [Table("ExtraSchCorrect")]
    public partial class ExtraSchCorrect0
    {

        /// <summary>
        /// 
        /// </summary> 
        [ExplicitKey]
        public Guid Id { get; set; }

        /// <summary>
        /// 学部id
        /// </summary>
        public Guid Eid { get; set; }

        /// <summary>
        /// 纠错类型
        /// </summary> 
        public byte Type { get; set; }

        /// <summary>
        /// 处理状态
        /// </summary> 
        public byte Status { get; set; }

        /// <summary>
        /// 图片
        /// </summary> 
        public string Img { get; set; }

        /// <summary>
        /// 备注
        /// </summary> 
        public string Remark { get; set; }

        /// <summary>
        /// 
        /// </summary> 
        public Guid Creator { get; set; }

        /// <summary>
        /// 
        /// </summary> 
        //dbDefaultValue// select (getdate()) 
        public DateTime CreateTime { get; set; } = DateTime.Now;

        /// <summary>
        /// 
        /// </summary> 
        public DateTime? ModifyDateTime { get; set; }

        /// <summary>
        /// 处理人
        /// </summary> 
        public Guid? Modifier { get; set; }

        /// <summary>
        /// 
        /// </summary> 
        //dbDefaultValue// select ((1)) 
        public bool IsValid { get; set; } = true;

        /// <summary>
        /// 
        /// </summary> 
        public string Address { get; set; }

        /// <summary>
        /// 
        /// </summary> 
        public double? Lng { get; set; }

        /// <summary>
        /// 
        /// </summary> 
        public double? Lat { get; set; }

        /// <summary>
        /// 
        /// </summary> 
        public string SchName { get; set; }

        /// <summary>
        /// 学校总类型
        /// </summary> 
        public string SchType { get; set; }

        /// <summary>
        /// 回复/不受理原因
        /// </summary>
        public string Reply { get; set; }

    }
}
