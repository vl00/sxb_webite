using System;
using System.Collections.Generic;
using System.Text;

namespace ProductManagement.Identification
{
    /// <summary>
    /// 雪花生成器
    /// </summary>
    public class SnowFlakeGenerator
    {
        static readonly object _lockInstance = new object();

        private static SnowFlakeGenerator _instance;

        public static SnowFlakeGenerator Instance
        {
            get
            {

                if (_instance == null)
                {
                    lock (_lockInstance)
                    {
                        if (_instance == null)
                        {
                            _instance = new SnowFlakeGenerator(1, 1);
                        }
                    }
                }

                return _instance;
            }
        }
        //基准时间
        public const long Twepoch = 1288834974657L;
        //机器标识位数
        const int WorkerIdBits = 5;
        //数据标志位数
        const int DatacenterIdBits = 5;
        //序列号识位数
        const int SequenceBits = 12;
        //机器ID最大值
        const long MaxWorkerId = -1L ^ (-1L << WorkerIdBits);
        //数据标志ID最大值
        const long MaxDatacenterId = -1L ^ (-1L << DatacenterIdBits);
        //序列号ID最大值
        private const long SequenceMask = -1L ^ (-1L << SequenceBits);
        //机器ID偏左移12位
        private const int WorkerIdShift = SequenceBits;
        //数据ID偏左移17位
        private const int DatacenterIdShift = SequenceBits + WorkerIdBits;
        //时间毫秒左移22位
        public const int TimestampLeftShift = SequenceBits + WorkerIdBits + DatacenterIdBits;

        private long _sequence = 0L;
        private long _lastTimestamp = -1L;

        public long WorkerId { get; protected set; }
        public long DatacenterId { get; protected set; }
        public long Sequence
        {
            get { return _sequence; }
            internal set { _sequence = value; }
        }

        /// <summary>
        /// 雪花发生器
        /// </summary>
        /// <param name="workerId"></param>
        /// <param name="datacenterId"></param>
        /// <param name="sequence"></param>
         SnowFlakeGenerator(long workerId, long datacenterId, long sequence = 0L)
        {
            // 如果超出范围就抛出异常
            if (workerId > MaxWorkerId || workerId < 0)
            {
                throw new ArgumentException(string.Format("worker Id 必须大于0，且不能大于MaxWorkerId： {0}", MaxWorkerId));
            }

            if (datacenterId > MaxDatacenterId || datacenterId < 0)
            {
                throw new ArgumentException(string.Format("region Id 必须大于0，且不能大于MaxWorkerId： {0}", MaxDatacenterId));
            }

            //先检验再赋值
            WorkerId = workerId;
            DatacenterId = datacenterId;
            _sequence = sequence;
        }

        /// <summary>
        /// 类全局锁
        /// </summary>
        readonly object _lock = new Object();

        /// <summary>
        /// 下一个id
        /// </summary>
        /// <returns></returns>
        public virtual long NextId()
        {
            lock (_lock)
            {
                var timestamp = TimeGen();
                if (timestamp < _lastTimestamp)
                {
                    throw new Exception(string.Format("时间戳必须大于上一次生成ID的时间戳.  拒绝为{0}毫秒生成id", _lastTimestamp - timestamp));
                }

                // 如果上次生成时间和当前时间相同,在同一毫秒内
                if (_lastTimestamp == timestamp)
                {
                    // sequence自增，和sequenceMask相与一下，去掉高位
                    _sequence = (_sequence + 1) & SequenceMask;
                    // 判断是否溢出,也就是每毫秒内超过1024，当为1024时，与sequenceMask相与，sequence就等于0
                    if (_sequence == 0)
                    {
                        // 等待到下一毫秒
                        timestamp = TilNextMillis(_lastTimestamp);
                    }
                }
                else
                {
                    // 如果和上次生成时间不同,重置sequence，就是下一毫秒开始，sequence计数重新从0开始累加,
                    // 为了保证尾数随机性更大一些,最后一位可以设置一个随机数
                    _sequence = 0;//new Random().Next(10);
                }

                _lastTimestamp = timestamp;
                return ((timestamp - Twepoch) << TimestampLeftShift) | (DatacenterId << DatacenterIdShift) | (WorkerId << WorkerIdShift) | _sequence;
            }
        }

        /// <summary>
        /// 防止产生的时间比之前的时间还要小（由于NTP回拨等问题）,保持增量的趋势.
        /// </summary>
        /// <param name="lastTimestamp"></param>
        /// <returns></returns>
        protected virtual long TilNextMillis(long lastTimestamp)
        {
            var timestamp = TimeGen();
            while (timestamp <= lastTimestamp)
            {
                timestamp = TimeGen();
            }
            return timestamp;
        }

        /// <summary>
        /// 获取当前的时间戳
        /// </summary>
        /// <returns></returns>
        protected virtual long TimeGen()
        {
            return TimeExtensions.CurrentTimeMillis();
        }
    }

    /// <summary>
    /// 时间扩展
    /// </summary>
    public static class TimeExtensions
    {
        /// <summary>
        /// 当前时间方法
        /// </summary>
        public static Func<long> currentTimeFunc = InternalCurrentTimeMillis;

        /// <summary>
        /// 当前时间毫秒
        /// </summary>
        /// <returns></returns>
        public static long CurrentTimeMillis()
        {
            return currentTimeFunc();
        }

        /// <summary>
        /// StubCurrentTime Func<long> func
        /// </summary>
        /// <param name="func"></param>
        /// <returns></returns>
        public static IDisposable StubCurrentTime(Func<long> func)
        {
            currentTimeFunc = func;
            return new DisposableAction(() =>
            {
                currentTimeFunc = InternalCurrentTimeMillis;
            });
        }

        /// <summary>
        /// StubCurrentTime  long millis
        /// </summary>
        /// <param name="millis"></param>
        /// <returns></returns>
        public static IDisposable StubCurrentTime(long millis)
        {
            currentTimeFunc = () => millis;
            return new DisposableAction(() =>
            {
                currentTimeFunc = InternalCurrentTimeMillis;
            });
        }

        private static readonly DateTime Jan1st1970 = new DateTime
            (1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        private static long InternalCurrentTimeMillis()
        {
            return (long)(DateTime.UtcNow - Jan1st1970).TotalMilliseconds;
        }
    }

    /// <summary>
    /// 销毁类
    /// </summary>
    public class DisposableAction : IDisposable
    {
        readonly Action _action;

        /// <summary>
        /// 销毁
        /// </summary>
        /// <param name="action"></param>
        public DisposableAction(Action action)
        {
            if (action == null)
                throw new ArgumentNullException("action");
            _action = action;
        }

        /// <summary>
        /// Dispose
        /// </summary>
        public void Dispose()
        {
            _action();
        }
    }
}
