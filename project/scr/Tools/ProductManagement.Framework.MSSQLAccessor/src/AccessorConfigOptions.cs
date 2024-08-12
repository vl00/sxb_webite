using Microsoft.Extensions.Options;

namespace ProductManagement.Framework.MSSQLAccessor
{
    public class AccessorConfigOptions<T>
    {
        public AccessorConfigOptions(IOptionsSnapshot<ConnectionConfig<T>> options)
        {
            ConnectionConfig = options.Value;
        }

        public ConnectionConfig<T> ConnectionConfig { get; }
    }
}
