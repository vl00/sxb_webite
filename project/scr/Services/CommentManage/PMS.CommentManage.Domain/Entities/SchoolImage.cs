using PMS.CommentsManage.Domain.Common;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PMS.CommentsManage.Domain.Entities
{
    /// <summary>
    /// 图片上传
    /// </summary>
    [Table("SchoolImage")]
    public class SchoolImage
    {
        public SchoolImage()
        {
            Id = Guid.NewGuid();
        }

        [Key]
        public Guid Id { get; set; }
        /// <summary>
        /// 所属数据id
        /// </summary>
        public Guid DataSourcetId { get; set; }
        /// <summary>
        /// 上传图片类型
        /// </summary>
        public ImageType ImageType { get; set; }
        /// <summary>
        /// 图片地址
        /// </summary>
        public string ImageUrl { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime AddTime { get; set; }
    }
}
