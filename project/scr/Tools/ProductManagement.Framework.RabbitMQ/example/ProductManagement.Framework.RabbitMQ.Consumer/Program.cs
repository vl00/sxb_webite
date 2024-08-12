using ProductManagement.Framework.RabbitMQ.Publisher;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ProductManagement.Framework.RabbitMQ.EventBus;
using ProductManagement.Framework.Serialize.Json;

namespace ProductManagement.Framework.RabbitMQ.Consumer
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Comsumer");

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
            },new NewtonsoftSerializer());

            serviceCollection.Scan(scan => scan
                .FromAssembliesOf(typeof(MessageEventHandler))
                .AddClasses()
                .AsImplementedInterfaces());

            serviceCollection.AddLogging();
            var f = new DefaultServiceProviderFactory();
            var serviceProvider = f.CreateServiceProvider(serviceCollection);

            var subscribe = serviceProvider.GetService<ISubscribe>();

            subscribe.Subscribe<SendMessageEvent>();
        }

        public class MessageEventHandler : IEventHandler<SendMessageEvent>
        {
            public Task Handle(SendMessageEvent message)
            {
                return Task.Run(() =>
                {
                    Console.WriteLine("等10秒");
                    Thread.Sleep(10000);
                    Console.WriteLine($"Comsumer {message.Message}");
                });
            }
        }

        public class MessageEventHandler1 : IEventHandler<SendMessageEvent>
        {
            public Task Handle(SendMessageEvent message)
            {
                return Task.Run(() =>
                {
                    Console.WriteLine("等10秒111");
                    Thread.Sleep(10000);
                    Console.WriteLine($"Comsumer1 {message.Message}");
                });
            }
        }

        public static string PrintMessageFor(object sender, string message)
        {
            var ne = Newtonsoft.Json.JsonConvert.DeserializeObject<SendMessageEvent>(message);
            Console.WriteLine($"Comsumer tooooooooooo {ne.Message}");

            return "";
        }
    }
}