using System;
using System.Collections.Generic;
using System.Text;

namespace ProductManagement.Framework.Ioc
{
    public interface IIocService : IDisposable
    {
        void Register<T>(T instance);
        T Inject<T>(T existing);
        T Inject<T>(T existing, string name);
        T Resolve<T>(Type type);
        T Resolve<T>(Type type, string name);
        T Resolve<T>();
        T Resolve<T>(string name);
        IEnumerable<T> ResolveAll<T>();
    }
}
