using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProductManagement.Framework.Foundation
{
    /// <summary>
    /// 一个提供轮询接口操作的工具
    /// </summary>
    /// <typeparam name="TResult"></typeparam>
    public class PollingTool<TResult>
    {


        /// <summary>
        /// 轮询计数器
        /// </summary>
        public int Count { get; set; }

        //是否结束
        public bool Stop { get; set; }

        //返回的结果对象
        public TResult Data { get; set; }

        /// <summary>
        /// 开启轮询操作，具体业务通过func传入。
        /// </summary>
        /// <param name="func"></param>
        /// <param name="speed">速率，单位为秒</param>
        /// <returns></returns>
        public static async Task<TResult> StartPolling( Func<PollingTool<TResult>, Task<PollingTool<TResult>>> func, int speed = 1)
        {
            return await Task.Run<TResult>(async () =>
            {
                PollingTool<TResult> pollingTool = new PollingTool<TResult>()
                {
                    Count = 0,
                    Stop = false
                };
                while (true)
                {
                    pollingTool.Count++;
                    var result = await func(pollingTool);
                    if (result.Stop)
                    {
                        return result.Data;
                    }
                    await Task.Delay(1000 * speed);
                }


            });
        }

        /// <summary>
        /// 开启轮询操作，具体业务通过func传入。
        /// </summary>
        /// <param name="func"></param>
        /// <param name="speed">速率，单位为秒</param>
        /// <returns></returns>
        public static async Task<TResult> StartPolling(Func<PollingTool<TResult>, PollingTool<TResult>> func, int speed = 1)
        {
            return await Task.Run<TResult>(async () =>
            {
                PollingTool<TResult> pollingTool = new PollingTool<TResult>()
                {
                    Count = 0,
                    Stop = false
                };
                while (true)
                {
                    pollingTool.Count++;
                    var result =  func(pollingTool);
                    if (result.Stop)
                    {
                        return result.Data;
                    }
                    await Task.Delay(1000 * speed);
                }


            });
        }


    }
}
