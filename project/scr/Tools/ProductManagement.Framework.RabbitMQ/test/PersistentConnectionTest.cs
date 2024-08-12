using System;
using System.Collections.Generic;
using System.Text;
using Xunit;
using RabbitMQ.Client;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace ProductManagement.Framework.RabbitMQ.Test
{
    public class PersistentConnectionTest
    {
        private static DefaultRabbitMQPersistentConnection GetConnection()
        {
            var connfactory = new ConnectionFactory()
            {
                HostName = "192.168.50.99",
                UserName = "guest",
                Password = "guest"
            };
            var loggerFactory = new LoggerFactory();
            var option =  Options.Create<RabbitMQOption>(new RabbitMQOption
            {
                //HostName = connfactory.HostName,
                //Password = connfactory.Password,
                //UserName = connfactory.UserName
            });

            var conn = new DefaultRabbitMQPersistentConnection(connfactory, loggerFactory, (IOptionsSnapshot<RabbitMQOption>)option);
            return conn;
        }

        [Fact]
        public void TryConnect()
        {
            var conn = GetConnection();

            Assert.False(conn.IsConnected);

            conn.TryConnect();

            Assert.True(conn.IsConnected);
        }

        [Fact]
        public void CreateModelExtion()
        {
            var conn = GetConnection();

            Assert.Throws<InvalidOperationException>(() =>
            {
                conn.CreateModel();
            });
        }


        [Fact]
        public void CreateModel()
        {
            var conn = GetConnection();
            conn.TryConnect();

            var channel = conn.CreateModel();

            Assert.True(channel.IsOpen);
        }
    }
}
