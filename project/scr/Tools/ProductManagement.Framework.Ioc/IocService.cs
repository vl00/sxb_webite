using Microsoft.Practices.Unity.Configuration;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Text;
using Unity;
using Unity.Resolution;

namespace ProductManagement.Framework.Ioc
{
    public class IocService : IIocService
    {
        //接口映射容器
        private readonly IUnityContainer _container;

        //进行初始化ioc容器，全局配置好所有的接口映射关系
        public IocService()
        {

            if (_container == null)
            {
                _container = new UnityContainer();
            }

            //读取配置文件中的所有接口映射配置信息
            UnityConfigurationSection unityConfigurationSection = (UnityConfigurationSection)ConfigurationManager.GetSection("unity");
            //遍历该节点下的所有子节点，然后配置至unity容器中，进行初始化
            foreach (ContainerElement current in unityConfigurationSection.Containers)
            {
                try
                {
                    Microsoft.Practices.Unity.Configuration.UnityContainerExtensions.LoadConfiguration(_container, unityConfigurationSection, current.Name); 
                }
                catch (Exception ex)
                {
                    throw;
                }
            }
        }

        /// <summary>
        /// 释放当前容器中的所有资源
        /// </summary>
        public void Dispose()
        {
            _container.Dispose();
        }

        public T Inject<T>(T existing)
        {
            throw new NotImplementedException();
        }

        public T Inject<T>(T existing, string name)
        {
            throw new NotImplementedException();
        }

        public void Register<T>(T instance)
        {
            throw new NotImplementedException();
        }

        public T Resolve<T>(Type type)
        {
            throw new NotImplementedException();
        }

        public T Resolve<T>(Type type, string name)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 获取泛型接口映射对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T Resolve<T>()
        {
            return Unity.UnityContainerExtensions.Resolve<T>(this._container, Array.Empty<ResolverOverride>());
        }

        public T Resolve<T>(string name)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<T> ResolveAll<T>()
        {
            throw new NotImplementedException();
        }
    }
}
