using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace ProductManagement.Infrastructure.Configs.Models
{
    /// <summary>
    /// 图文客服消息模板
    /// </summary>
    public class TxtMsgTemplate
    {

        [Required]
        public string Title { get; set; }

        [Required]
        public string Description { get; set; }

        /// <summary>
        /// 绑定账户链接
        /// </summary>
        [Required]
        public string RedirectUrl { get; set; }

        /// <summary>
        /// 图文链接
        /// </summary>
        [Required]
        public string ImgUrl { get; set; }

    }
}
