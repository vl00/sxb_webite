using log4net;
using System;

namespace ProductManagement.Framework.Foundation
{
    public class Logger
    {
        private const string LogError = "Error";
        private const string LogDebug = "Debug";
        private const string DefaultName = "Info";
        private static readonly ILog log = LogManager.GetLogger("Change.Logger");
        public static void Debug(string message)
        {
            ILog logger = LogManager.GetLogger("Debug");
            if (logger.IsDebugEnabled)
            {
                logger.Debug(message);
            }
        }
        /// <summary>
        /// debug调试
        /// </summary>
        /// <param name="message"></param>
        /// <param name="ex"></param>
        public static void Debug(string message, Exception ex)
        {
            ILog logger = LogManager.GetLogger("Debug");
            if (logger.IsDebugEnabled)
            {
                logger.Debug(message, ex);
            }
        }
        /// <summary>
        /// 错误日志
        /// </summary>
        /// <param name="message"></param>
        public static void Error(string message)
        {
            ILog logger = LogManager.GetLogger("Error");
            if (logger.IsDebugEnabled)
            {
                logger.Error(message);
            }
        }
        public static void Error(string message, Exception ex)
        {

            ILog logger = LogManager.GetLogger("Error");
            if (logger.IsDebugEnabled)
            {
                logger.Error(message, ex);
            }
        }
        public static void Fatal(string message)
        {
            ILog logger = LogManager.GetLogger("Info");
            if (logger.IsDebugEnabled)
            {
                logger.Fatal(message);
            }
        }
        public static void Log(string message)
        {
            ILog logger = LogManager.GetLogger("Info");
            if (logger.IsDebugEnabled)
            {
                logger.Info(message);
            }
        }
        public static void Warn(string message)
        {
            ILog logger = LogManager.GetLogger("Info");
            if (logger.IsDebugEnabled)
            {
                logger.Warn(message);
            }
        }
    }
}
