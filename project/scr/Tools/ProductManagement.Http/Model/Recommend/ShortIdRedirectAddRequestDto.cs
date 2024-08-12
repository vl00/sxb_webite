
using System;
using System.Collections.Generic;
using System.Text;

namespace ProductManagement.API.Http.Model.Recommend
{
    public class ShortIdRedirectAddRequestDto
    {
        /// <summary>
        /// 源学校/文章页面的学部id
        /// </summary>
        public string ReferShortId { get; set; }

        /// <summary>
        /// 目标学校/文章页面的学部id
        /// </summary>
        public string CurrentShortId { get; set; }
    }

}
