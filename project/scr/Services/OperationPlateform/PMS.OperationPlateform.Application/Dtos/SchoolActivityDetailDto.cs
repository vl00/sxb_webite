using PMS.OperationPlateform.Domain.DTOs;
using PMS.OperationPlateform.Domain.Entitys;
using System;
using System.Collections.Generic;
using System.Text;

namespace PMS.OperationPlateform.Application.Dtos
{
    public class SchoolActivityDetailDto : SchoolActivity
    {
        public List<SchoolActivityRegisteExtensionDto> Extensions { get; set; }

        public List<Image> Images { get; set; }

        /// <summary>
        /// 移动端分享需要使用school name
        /// 首次调取接口需要返回, 
        /// 第二次获取学校信息,可能会延迟,分享不到学校信息
        /// </summary>
        public string SchoolName { get; set; }

        public class Image
        {
            /// <summary>
            /// 原图
            /// </summary>
            public string Url { get; set; }

            /// <summary>
            /// 缩略图
            /// </summary>
            public string Thunmbnail { get; set; }

            /// <summary>
            /// 排序
            /// </summary>
            public int Sort { get; set; }
        }
    }
}
