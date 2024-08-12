using System;
using ProductManagement.Framework.Cache.Redis.NumberCreater;
using Xunit;

namespace ProductManagement.Framework.Cache.Redis.Test.NumberCreater
{
    public class NumberRandomGreaterTest
    {
        [Fact]
        public void Generate_TotalWidth_Default_Test()
        {
            INumberCreater numberRandomGreater = new NumberRandomGreater();

            var number = numberRandomGreater.Generate(string.Empty);
            Assert.Equal(18, number.Length);
        }

        [Fact]
        public void Generate_TotalWidth_Format_Test()
        {
            INumberCreater numberRandomGreater = new NumberRandomGreater();

            var number = numberRandomGreater.Generate(string.Empty, 0, "yyyyMMddHHmm");
            Assert.Equal(DateTime.Now.ToString("yyyyMMddHHmm"), number);
        }

        [Fact]
        public void Generate_TotalWidth_FormatException_Test()
        {
            INumberCreater numberRandomGreater = new NumberRandomGreater();
            Assert.Throws<FormatException>(() => { numberRandomGreater.Generate(string.Empty, 0, "-"); });
        }

        [Fact]
        public void Generate_TotalWidth_LessThenZero_Test()
        {
            INumberCreater numberRandomGreater = new NumberRandomGreater();
            Assert.Throws<ArgumentOutOfRangeException>(() => { numberRandomGreater.Generate(string.Empty, -1); });
        }

        [Fact]
        public void Generate_TotalWidth_Zero_Test()
        {
            INumberCreater numberRandomGreater = new NumberRandomGreater();

            var number = numberRandomGreater.Generate(string.Empty, 0);
            Assert.Equal(DateTime.Now.ToString("yyMMddHHmm"), number);

            var goNumber = numberRandomGreater.Generate("go", 0);
            Assert.Equal($"go{DateTime.Now:yyMMddHHmm}", goNumber);
        }
    }
}