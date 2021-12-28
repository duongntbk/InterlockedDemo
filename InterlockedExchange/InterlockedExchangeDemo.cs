using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace InterlockedExchange
{
    public class InterlockedExchangeDemo
    {
        private static int _isSafe = 1;
        private static bool _isSafeBool = true;
        private static readonly object _lockObj = new object();

        static void Main(string[] args)
        {
            Measure(() => CheckWithLock(100), "WARM UP");
            Measure(() => CheckWithLock(500_000), nameof(CheckWithLock));
            Measure(() => CheckWithInterlocked(500_000), nameof(CheckWithInterlocked));
        }

        public static void Measure(Action act, string caseName)
        {
            var sw = new Stopwatch();
            sw.Start();
            act();
            sw.Stop();
            var ts = sw.ElapsedMilliseconds;
            Console.WriteLine($"Result for {caseName}: {ts}ms");
        }

        public static void CheckWithLock(int load)
        {
            Parallel.For(0, load, (i, loop) =>
            {
                lock (_lockObj)
                {
                    if (!_isSafeBool)
                    {
                        return;
                    }
                    else
                    {
                        _isSafeBool = false;
                    }
                }
                DummyDoWork();

                lock (_lockObj)
                {
                    _isSafeBool = true;
                }
            });
        }

        public static void CheckWithInterlocked(int load)
        {
            Parallel.For(0, load, (i, loop) =>
            {
                // Try to set _isSafe to 0 and check the original value.
                // If the original value is 1 then we can safely call the method.
                if (Interlocked.Exchange(ref _isSafe, 0) == 1)
                {
                    DummyDoWork();

                    // Remember to set _isSafe back to 1 so that other threads can call the method.
                    Interlocked.Exchange(ref _isSafe, 1);
                }
            });
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        static void DummyDoWork()
        {
            var a = 1;
        }
    }
}
