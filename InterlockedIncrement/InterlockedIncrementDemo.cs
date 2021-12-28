using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace InterlockedIncrement
{
    public class InterlockedIncrementDemo
    {
        private static readonly object _lockObj = new object();

        static void Main(string[] args)
        {
            var src = Enumerable.Range(1, 100_000);
            Func<int, bool> predicate = n => n % 2 == 0;
            Measure(NoSynchronize, src, predicate, "WARM_UP");
            Measure(NoSynchronize, src, predicate, nameof(NoSynchronize));
            Measure(WithLock, src, predicate, nameof(WithLock));
            Measure(InterlockedIncrement, src, predicate, nameof(InterlockedIncrement));
        }

        public static void Measure(Func<IEnumerable<int>, Func<int, bool>, long> func,
            IEnumerable<int> src, Func<int, bool> predicate, string caseName)
        {
            var sw = new Stopwatch();
            sw.Start();
            var sum = func(src, predicate);
            sw.Stop();
            var ts = sw.ElapsedMilliseconds;
            Console.WriteLine($"Result for {caseName}: {sum}.Runtime: {ts}ms");
        }

        public static long InterlockedIncrement(IEnumerable<int> src, Func<int, bool> predicate)
        {
            long sum = 0;
            Parallel.ForEach(src, n =>
            {
                if (predicate(n))
                {
                    Interlocked.Increment(ref sum);
                }
            });
            return sum;
        }

        public static long WithLock(IEnumerable<int> src, Func<int, bool> predicate)
        {
            long sum = 0;
            Parallel.ForEach(src, n =>
            {
                if (predicate(n))
                {
                    lock (_lockObj)
                    {
                        sum++;
                    }
                }
            });
            return sum;
        }

        public static long NoSynchronize(IEnumerable<int> src, Func<int, bool> predicate)
        {
            long sum = 0;
            Parallel.ForEach(src, n => {
                if (predicate(n))
                {
                    sum++;
                }
            });
            return sum;
        }
    }
}
