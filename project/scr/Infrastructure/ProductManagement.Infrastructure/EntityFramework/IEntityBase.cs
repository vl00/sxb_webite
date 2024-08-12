using System;
using System.Collections.Generic;
using System.Text;

namespace ProductManagement.Infrastructure.EntityFramework
{
    /// <summary>
    /// 实体基类
    /// </summary>
    public interface IEntityBase
    {
        Guid Id { get; set; }
    }
}
