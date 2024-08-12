using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Console;
using System;
using Xunit;
using System.Threading;
using Microsoft.Extensions.Configuration;

namespace ProductManagement.Framework.RabbitMQ.Test
{
    public class EventBusRabbitMqTest
    {
        //[Fact]
        //public void Publicsh()
        //{
        //    var eventbus = GetEventBus();

        //    for (var i = 0; i < 2; i++)
        //    {

        //        eventbus.Publish(new TestEvent { Message = i.ToString() });
        //    }
        //}

        //[Fact]
        //public void Subscribe()
        //{
        //    var eventbus = GetEventBus();
        //    eventbus.Subscribe<TestEvent>(new TestEventHandler());
        //}

        //[Fact]
        //public void Unsubscribe()
        //{
        //    var eventbus = GetEventBus();
        //    eventbus.Subscribe<TestEvent>(new TestEventHandler());
        //    eventbus.Unsubscribe<TestEvent>(new TestEventHandler());

        //}

        //private  IEventBus GetEventBus()
        //{
        //    var serviceCollection = new ServiceCollection();
        //    var builder = new ConfigurationBuilder()
        //   .AddJsonFile("rabbitMQSetting.json", false, true)
        //   .AddEnvironmentVariables();
        //    var root = builder.Build();
        //    var config = root.GetSection("rabbitMQSetting").Get<RabbitMQOption>();

        //    serviceCollection.AddOptions();

        //    serviceCollection.AddProductManagementRabbitMQ(option =>
        //    {
        //        option.HostName = config.HostName;
        //        option.Password = config.Password;
        //        option.UserName = config.UserName;
        //    });
        //    serviceCollection.AddLogging();
        //    var f = new DefaultServiceProviderFactory();
        //    var serviceProvider = f.CreateServiceProvider(serviceCollection);
        //    var eventbus = serviceProvider.GetService<IEventBus>();
        //    return eventbus;
        //}
    }
}
