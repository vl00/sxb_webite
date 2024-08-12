using Newtonsoft.Json;
using NUnit.Framework;
using System;
using System.Diagnostics;

namespace ProductManagement.Tool.HtmlAnalysis.Test
{
    public class BaseUnitTest
    {
        public static void WriteLine<T>(T t)
        {
            var msg = JsonConvert.SerializeObject(t);
            WriteLine(msg);
        }

        public static void WriteLine(string msg)
        {
            //Debug.WriteLine(msg);
            //Trace.WriteLine(msg);
            Console.WriteLine(msg);
        }

    }
}