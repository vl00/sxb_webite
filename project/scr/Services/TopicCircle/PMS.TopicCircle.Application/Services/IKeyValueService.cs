using PMS.TopicCircle.Domain.Entities;
using Remotion.Linq.Clauses;
using System;
using System.Collections.Generic;
using System.Text;

namespace PMS.TopicCircle.Application.Services
{
    /// <summary>
    /// 系统的全局变量配置
    /// </summary>
    public interface IKeyValueService
    {

        string Get(string key);

        bool Set(string key, string value);




    }
}
