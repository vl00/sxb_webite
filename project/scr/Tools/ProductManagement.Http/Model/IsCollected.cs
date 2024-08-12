using System;
using System.Collections.Generic;
using System.Text;

namespace ProductManagement.API.Http.Model
{
    /// <summary>
    /// 是否已被收藏
    /// </summary>
    public class IsCollected
    {
        public Guid dataID { get; set; }
        public Guid userId { get; set; }
    }
}
