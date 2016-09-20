using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileCopier
{
    static class IEnumerableExtensions
    {
        public static void ForEach<T>(this IEnumerable<T> enumerable, Action<T> action)
        {
            foreach (var element in enumerable)
            {
                action(element);
            }
        }
    }

    static class QueueExtension
    {
        public static T SafeDequeue<T>(this Queue<T> queue)
        {
            T dequeuedItem;
            lock(queue)
            {
                dequeuedItem = queue.Dequeue();
            }
            return dequeuedItem;
        }

        public static void SafeEnqueue<T>(this Queue<T> queue, T item)
        {
            lock(queue)
            {
                queue.Enqueue(item);
            }
        }
    }
}
