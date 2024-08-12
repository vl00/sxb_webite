using System;
using Microsoft.Extensions.Options;

namespace ProductManagement.Framework.MongoDb
{
    public class MongoConfigOptions
    {
        public MongoConfigOptions(IOptionsSnapshot<MongoDbConfig> options)
        {
            ConnectionConfig = options.Value;
        }

        public MongoDbConfig ConnectionConfig { get; }
    }

    public class MongoConfigOptions<T> : MongoConfigOptions
    {
        public MongoConfigOptions(IOptionsSnapshot<MongoDbConfig<T>> options) : base(options)
        {
        }
    }
}
