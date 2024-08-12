using System;
using ProductManagement.Framework.Cache.Redis.NumberCreater;
using Moq;
using StackExchange.Redis;
using Xunit;

namespace ProductManagement.Framework.Cache.Redis.Test.NumberCreater
{
    public class NumberRedisCreaterTest
    {
        [Fact]
        public void Generate_Default_Test()
        {
            var dataBaseMock = new Mock<IDatabase>();
            dataBaseMock.Setup(d => d.StringIncrement(It.IsAny<RedisKey>(),
                It.IsAny<long>(),
                It.IsAny<CommandFlags>())).Returns(1L);


            var redisClientMock = new Mock<ICacheRedisClient>();
            redisClientMock.SetupGet(c => c.Database).Returns(dataBaseMock.Object);

            INumberCreater numberRedisCreater = new NumberRedisCreater(null, redisClientMock.Object);

            var number = numberRedisCreater.Generate();
            Assert.Equal(DateTime.Now.ToString("yyMMddHHmm00000001"), number);
        }

        [Fact]
        public void Generate_NoNextThrowException_Test()
        {
            var dataBaseMock = new Mock<IDatabase>();
            dataBaseMock.Setup(d => d.StringIncrement(It.IsAny<RedisKey>(),
                It.IsAny<long>(),
                It.IsAny<CommandFlags>())).Throws<Exception>();


            var redisClientMock = new Mock<ICacheRedisClient>();
            redisClientMock.SetupGet(c => c.Database).Returns(dataBaseMock.Object);

            INumberCreater numberRedisCreater = new NumberRedisCreater(null, redisClientMock.Object);

            Assert.Throws<Exception>(() => { numberRedisCreater.Generate(); });
        }

        [Fact]
        public void Generate_Prefix_Test()
        {
            var dataBaseMock = new Mock<IDatabase>();
            dataBaseMock.Setup(d => d.StringIncrement(It.IsAny<RedisKey>(),
                It.IsAny<long>(),
                It.IsAny<CommandFlags>())).Returns(1L);


            var redisClientMock = new Mock<ICacheRedisClient>();
            redisClientMock.SetupGet(c => c.Database).Returns(dataBaseMock.Object);

            INumberCreater numberRedisCreater = new NumberRedisCreater(null, redisClientMock.Object);

            var number = numberRedisCreater.Generate("go");
            Assert.Equal("go" + DateTime.Now.ToString("yyMMddHHmm00000001"), number);
        }

        [Fact]
        public void Generate_TotalWidth_LessThenZero_Test()
        {
            var redisClientMock = new Mock<ICacheRedisClient>();

            INumberCreater numberRedisCreater = new NumberRedisCreater(null, redisClientMock.Object);

            Assert.Throws<ArgumentOutOfRangeException>(() => { numberRedisCreater.Generate(string.Empty, -1); });
        }

        [Fact]
        public void Generate_TotalWidth_Zero_Test()
        {
            var dataBaseMock = new Mock<IDatabase>();
            dataBaseMock.Setup(d => d.StringIncrement(It.IsAny<RedisKey>(),
                It.IsAny<long>(),
                It.IsAny<CommandFlags>())).Returns(1L);


            var redisClientMock = new Mock<ICacheRedisClient>();
            redisClientMock.SetupGet(c => c.Database).Returns(dataBaseMock.Object);

            INumberCreater numberRedisCreater = new NumberRedisCreater(null, redisClientMock.Object);

            var number = numberRedisCreater.Generate(string.Empty, 0);
            Assert.Equal(DateTime.Now.ToString("yyMMddHHmm"), number);
        }

        [Fact]
        public void Generate_TryTwo_Test()
        {
            var dataBaseMockFirst = new Mock<IDatabase>();
            dataBaseMockFirst.Setup(d => d.StringIncrement(It.IsAny<RedisKey>(),
                It.IsAny<long>(),
                It.IsAny<CommandFlags>())).Throws<Exception>();


            var redisClientMockFirst = new Mock<ICacheRedisClient>();
            redisClientMockFirst.SetupGet(c => c.Database).Returns(dataBaseMockFirst.Object);

            var dataBaseMockSecond = new Mock<IDatabase>();
            dataBaseMockSecond.Setup(d => d.StringIncrement(It.IsAny<RedisKey>(),
                It.IsAny<long>(),
                It.IsAny<CommandFlags>())).Returns(1L);


            var redisClientMockSecond = new Mock<ICacheRedisClient>();
            redisClientMockSecond.SetupGet(c => c.Database).Returns(dataBaseMockSecond.Object);

            INumberCreater numberRedisCreaterSecond = new NumberRedisCreater(null, redisClientMockSecond.Object);

            INumberCreater numberRedisCreater =
                new NumberRedisCreater(numberRedisCreaterSecond, redisClientMockFirst.Object);

            var number = numberRedisCreater.Generate();
            Assert.Equal(DateTime.Now.ToString("yyMMddHHmm00000001"), number);
        }
    }
}