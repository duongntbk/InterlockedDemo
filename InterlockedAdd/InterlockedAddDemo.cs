using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace InterlockedAdd
{
    public class InterlockedAddDemo
    {
        private static readonly object _lockObj = new object();

        static void Main(string[] args)
        {
            var src = Enumerable.Range(1, 100_000);
            Measure(NoSynchronize, src, "WARM_UP");
            Measure(NoSynchronize, src, nameof(NoSynchronize));
            Measure(WithLock, src, nameof(WithLock));
            Measure(WithLockLocalVar, src, nameof(WithLockLocalVar));
            Measure(InterlockedAdd, src, nameof(InterlockedAdd));
            Measure(InterlockedAddLocalVar, src, nameof(InterlockedAddLocalVar));
        }

        public static void Measure(Func<IEnumerable<int>, long> func, IEnumerable<int> src, string caseName)
        {
            var sw = new Stopwatch();
            sw.Start();
            var sum = func(src);
            sw.Stop();
            var ts = sw.ElapsedMilliseconds;
            Console.WriteLine($"Result for {caseName}: {sum}.Runtime: {ts}ms");
        }

        public static long InterlockedAdd(IEnumerable<int> src)
        {
            long sum = 0;
            Parallel.ForEach(src, n => Interlocked.Add(ref sum, n));
            return sum;
        }

        public static long InterlockedAddLocalVar(IEnumerable<int> src)
        {
            long sum = 0;
            Parallel.ForEach<int, long>(
                src,
                () => 0,
                (n, _, localSum) => localSum += n,
                threadSum => Interlocked.Add(ref sum, threadSum));
            return sum;
        }

        public static long WithLock(IEnumerable<int> src)
        {
            long sum = 0;
            Parallel.ForEach(src, n =>
            {
                lock (_lockObj)
                {
                    sum += n;
                }
            });
            return sum;
        }

        public static long WithLockLocalVar(IEnumerable<int> src)
        {
            long sum = 0;
            Parallel.ForEach<int, long>(
                src,
                () => 0,
                (n, _, localSum) => localSum += n,
                threadSum =>
                {
                    lock (_lockObj)
                    {
                        sum += threadSum;
                    }
                });
            return sum;
        }

        public static long NoSynchronize(IEnumerable<int> src)
        {
            long sum = 0;
            Parallel.ForEach(src, n => sum += n);
            return sum;
        }
    }
}
