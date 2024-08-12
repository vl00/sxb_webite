using PMS.TopicCircle.Application.Dtos;
using ProductManagement.Framework.AspNetCoreHelper.RequestModel;
using Sxb.Web.RequestModel;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using static PMS.TopicCircle.Application.Dtos.TopicAddDto;

namespace Sxb.Web.Areas.TopicCircle.Models
{
    public class TopicAddRequest:WebContentRequest
    {
        public Guid Id { get; set; }

        [Required]
        public Guid CircleId { get; set; }
        [Required]
        public Guid UserId { get; set; }
        [Required, StringLength(1000, ErrorMessage = "最多输入1000个汉字")]
        public  string Content { get; set; }
        public bool IsOpen { get; set; }
        public bool IsQA { get; set; }
        public bool IsAutoSync { get; set; }
        public AttachmentDto Attachment { get; set; }
        [MaxLength(9, ErrorMessage = "最多选择9张图片")]
        public List<ImageDto> Images { get; set; }
        [MaxLength(3, ErrorMessage = "最多选择3个标签")]
        public List<int> Tags { get; set; }

        public override string GetContent()
        {
            return this.Content;
        }

        public static implicit operator TopicAddDto(TopicAddRequest origin)
        {
            return new TopicAddDto()
            {
                Id = origin.Id,
                Attachment = origin.Attachment,
                CircleId = origin.CircleId,
                Content = origin.Content,
                Images = origin.Images,
                IsAutoSync = origin.IsAutoSync,
                IsOpen = origin.IsOpen,
                IsQA = origin.IsQA,
                Tags = origin.Tags,
                UserId = origin.UserId
            };
        }

    }
}
