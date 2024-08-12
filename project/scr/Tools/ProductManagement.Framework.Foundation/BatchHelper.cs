using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProductManagement.Framework.Foundation
{
    public class BatchHelper
    {
        public class Batch<T>
        {
            public int BatchSize { get; set; }
            public IEnumerable<T> Entities { get; set; }

            public Batch(IEnumerable<T> entities, int batchSize = 1000)
            {
                Entities = entities ?? throw new ArgumentNullException(nameof(entities));
                BatchSize = batchSize;
            }

            public async Task RunAsync(Func<IEnumerable<T>, Task> action)
            {
                int i = 0;
                List<T> items = null;
                foreach (var item in Entities)
                {
                    if (items == null)
                    {
                        items = new List<T>(BatchSize);
                    }

                    items.Add(item);
                    if (++i >= BatchSize)
                    {
                        await action.Invoke(items);
                        //recalc
                        i = 0;
                        items = null;
                    }
                }

                if (items != null && items.Any())
                {
                    await action.Invoke(items);
                }
            }


            public void RunParallel(Func<IEnumerable<T>, Task> action)
            {
                int i = 0;
                List<T> items = null;
                List<Task> tasks = new List<Task>();
                foreach (var item in Entities)
                {
                    if (items == null)
                    {
                        items = new List<T>(BatchSize);
                    }

                    items.Add(item);
                    if (++i >= BatchSize)
                    {
                        tasks.Add(action.Invoke(items));
                        //recalc
                        i = 0;
                        items = null;
                    }
                }

                if (items != null && items.Any())
                {
                    tasks.Add(action.Invoke(items));
                }
                Task.WaitAll(tasks.ToArray());
            }
        }

        public static async Task RunAsync<T>(IEnumerable<T> entities, Func<IEnumerable<T>, Task> action)
        {
           await new Batch<T>(entities).RunAsync(action);
        }
    }
}
