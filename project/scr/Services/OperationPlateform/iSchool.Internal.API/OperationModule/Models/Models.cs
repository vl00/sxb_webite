using System;
using System.Collections.Generic;
using System.Text;

namespace iSchool.Internal.API.OperationModule.Models
{
    /// <summary>
    /// 工具模块
    /// </summary>
    public class ToolModule
    {
        public string moduleName { get; set; }
        public List<Tooltype> toolTypes { get; set; }

        /// <summary>
        /// 工具类型
        /// </summary>
        public class Tooltype
        {
            public int id { get; set; }
            public string name { get; set; }
            public int moduleId { get; set; }
            public string icon { get; set; }

            public int sort { get; set; }
        }
    }



    /// <summary>
    /// 工具
    /// </summary>
    public class Tool
    {
        public int id { get; set; }
        public string name { get; set; }
        public int? cityId { get; set; }
        public int toolTypeId { get; set; }
        public string linkUrl { get; set; }
        public bool isShow { get; set; }
    }

}
