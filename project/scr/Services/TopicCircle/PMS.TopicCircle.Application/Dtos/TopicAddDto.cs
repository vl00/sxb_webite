using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using static PMS.TopicCircle.Domain.Enum.GlobalEnum;

namespace PMS.TopicCircle.Application.Dtos
{
    public class TopicAddDto
    {
        public Guid Id { get; set; }

        [Required]
        public Guid CircleId { get; set; }
        [Required]
        public Guid UserId { get; set; }
        [Required, StringLength(1000, ErrorMessage = "最多输入1000个汉字")]
        public string Content { get; set; }
        public bool IsOpen { get; set; }
        public bool IsQA { get; set; }
        public bool IsAutoSync { get; set; }
        public AttachmentDto Attachment { get; set; }
        [MaxLength(9, ErrorMessage ="最多选择9张图片")]
        public List<ImageDto> Images { get; set; }

        [MaxLength(3, ErrorMessage ="最多选择3个标签")]
        public  List<int> Tags { get; set; }

        public class AttachmentDto
        {
            public Guid Id { get; set; }
            public Guid? AttchId { get; set; }
            public string Content { get; set; }
            public string AttachUrl { get; set; }
            public TopicType Type { get; set; }
        }

        public class ImageDto
        {
            public Guid Id { get; set; }
            public string Url { get; set; }
            public int Sort { get; set; }
        }

    }
}
