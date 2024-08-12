using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using MongoDB.Driver;

namespace ProductManagement.Framework.MongoDb
{
    public interface IMongoProvider {
    
    }

    /// <summary>
    /// 埋点日志只有正式环境有
    /// </summary>
    public interface IStatisticsMongoProvider : IMongoProvider
    {

    }

}