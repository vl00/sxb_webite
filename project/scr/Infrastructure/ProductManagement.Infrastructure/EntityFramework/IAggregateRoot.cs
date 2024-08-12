﻿using System;
using System.Collections.Generic;
using System.Text;

namespace ProductManagement.Infrastructure.EntityFramework
{
    /// <summary>
    /// 聚合根
    /// </summary>
    public interface IAggregateRoot : IEntityBase
    {
    }

    public abstract class AggregateRoot : EntityBase,IEntityBase,IAggregateRoot
    {

    }
}
