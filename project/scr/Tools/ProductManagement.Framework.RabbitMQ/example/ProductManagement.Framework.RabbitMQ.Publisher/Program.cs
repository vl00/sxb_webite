using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading;
using ProductManagement.Framework.Serialize.Json;

namespace ProductManagement.Framework.RabbitMQ.Publisher
{
    class Program
    {
        static void Main(string[] args)
        {

            Console.WriteLine("Publisher");

            var serviceCollection = new ServiceCollection();

            var builder = new ConfigurationBuilder()
             .AddJsonFile("rabbitMQSetting.json", false, true)
             .AddEnvironmentVariables();
            var root = builder.Build();
            var config = root.GetSection("rabbitMQSetting").Get<RabbitMQOption>();

            serviceCollection.AddOptions();

            serviceCollection.AddProductManagementRabbitMQ(option =>
            {
                option.AmqpUris = config.AmqpUris;
                option.Uri = config.Uri;
            }, new NewtonsoftSerializer());
            serviceCollection.AddLogging();
            var f = new DefaultServiceProviderFactory();
            var serviceProvider = f.CreateServiceProvider(serviceCollection);
            var eventbus = serviceProvider.GetService<IEventBus>();

            //System.Threading.Tasks.Parallel.For(0, 100, (a) =>
            //{
            SendMessage(eventbus);
            //});



        }

        static void SendMessage(IEventBus eventbus)
        {
            var i = 0;
            while (true)
            {
                Console.WriteLine($"Publisher {i}");

                eventbus.Publish(new SendMessageEvent
                {
                    Message = i.ToString()
                });

                i++;

                if (i == 10000) break;
            }

        }
    }
}