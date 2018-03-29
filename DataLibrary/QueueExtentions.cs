using System.Collections.Generic;

namespace System.Collections.Generic
{
    public static class QueueExtentions
    {
        public static void Add<T>(Queue<T> list, T obj)
        {
            list.Enqueue(obj);
        }

        public static T Pop<T>(Queue<T> list)
        {
            return list.Dequeue();
        }
    }
}