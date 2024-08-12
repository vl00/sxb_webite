using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace PMS.UserManage.Domain.Entities
{
    [Table("talent_img")]
    public class TalentImg
    {
        /// <summary>
        /// 主键
        /// </summary>
        [Key]
        public Guid id { get; set; }

        /// <summary>
        /// 达人表主键
        /// </summary>
        public Guid talent_id { get; set; }

        /// <summary>
        /// 链接
        /// </summary>
        public string url { get; set; }

        /// <summary>
        /// 图片类型  默认0为证明材料，当为机构达人时，0为单位证明，1为公司认证公函
        /// </summary>
        public int? type { get; set; } = 0;
    }
}
