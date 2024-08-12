using System;
using System.Collections.Generic;
using System.Text;

namespace PMS.OperationPlateform.Application.Dtos
{
    public class ArticleGetDetailRequestDto
    {
        /// <summary>
        /// 文章ID
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// 文章自增编号
        /// </summary>
        public int No { get; set; }
    }
}
