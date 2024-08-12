using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sxb.PCWeb.Models.Images
{
    /// <summary>
    /// 前端图片展示
    /// </summary>
    public class Image
    {
        public Guid Id { get; set; }
        /// <summary>
        /// 所属数据id
        /// </summary>
        public Guid DataSourcetId { get; set; }
        /// <summary>
        /// 上传图片类型
        /// </summary>
        public string ImageType { get; set; }
        /// <summary>
        /// 图片地址
        /// </summary>
        public string ImageUrl { get; set; }
    }
}
