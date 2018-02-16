using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApp.Extencions
{
    public static class DictionaryEx
    {
        public static List<T> Take<T>(this ConcurrentQueue<T> queue, int count)
        {
            List<T> list = new List<T>();

            for (int i = 0; i < count; i++)
            {
                if (!queue.TryDequeue(out T result))
                {
                    break;
                }
                list.Add(result);
            }
            return list;
        }
    }
}
