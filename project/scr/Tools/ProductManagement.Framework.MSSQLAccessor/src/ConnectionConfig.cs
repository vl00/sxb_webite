using System;
using System.Collections.Generic;
using System.Text;

namespace ProductManagement.Framework.MSSQLAccessor
{
    public class ConnectionConfig<T>
    {
        public string Name { get; set; }
        public string LoadBalancer { get; set; }

        public List<string> RealDbConnectionStrings { get; set; }

        public void Config(string name, CQRSConfig config)
        {
            if (config is null)
            {
                throw new ArgumentNullException(nameof(config));
            }

            Name = name ?? throw new ArgumentNullException(nameof(name));
            LoadBalancer = config.Master ?? throw new ArgumentNullException(nameof(config.Master));
            RealDbConnectionStrings = config.Slavers ?? new List<string>() { config.Master };
        }
    }


    public class CQRSConfig
    {
        public string Master { get; set; }

        public List<string> Slavers { get; set; }

    }
}
