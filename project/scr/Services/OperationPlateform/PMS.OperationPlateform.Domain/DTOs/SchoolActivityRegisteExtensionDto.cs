using PMS.OperationPlateform.Domain.Entitys;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace PMS.OperationPlateform.Domain.DTOs
{
    public class SchoolActivityRegisteExtensionDto : SchoolActivityRegisteExtension
    {
        /// <summary>
        /// 多选数据源,  根据Extension.Value用##分隔拆开
        /// </summary>
        public List<string> DataSource
        {
            get
            {
                if (!string.IsNullOrWhiteSpace(Value))
                {
                    return Value.Split(new string[] { "##" }, StringSplitOptions.RemoveEmptyEntries).ToList();
                }
                return new List<string>();
            }
        }
    }
}
