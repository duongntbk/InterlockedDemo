using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using InterlockedAdd;
using InterlockedExchange;
using InterlockedIncrement;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Benchmark
{
    class Program
    {
        static void Main(string[] args)
        {
            BenchmarkSwitcher.FromAssembly(typeof(Program).Assembly).Run(args);
        }
    }

    [MemoryDiagnoser]
    public class InterlockedAdd
    {
        private IEnumerable<int> _src;

        [GlobalSetup]
        public void Setup()
        {
            _src = Enumerable.Range(1, 1_000);
        }

        [Benchmark]
        public long NoSynchronize() => InterlockedAddDemo.NoSynchronize(_src);

        [Benchmark]
        public long Lock() => InterlockedAddDemo.WithLock(_src);

        [Benchmark]
        public long LockLocalVar() => InterlockedAddDemo.WithLockLocalVar(_src);

        [Benchmark]
        public long Interlocked() => InterlockedAddDemo.InterlockedAdd(_src);

        [Benchmark]
        public long InterlockedLocalVar() => InterlockedAddDemo.InterlockedAddLocalVar(_src);
    }

    [MemoryDiagnoser]
    public class InterlockedIncrement
    {
        private IEnumerable<int> _src;
        private Func<int, bool> _predicate;

        [GlobalSetup]
        public void Setup()
        {
            _src = Enumerable.Range(1, 1_000);
            _predicate = n => n % 2 == 0;
        }

        [Benchmark]
        public long NoSynchronize() => InterlockedIncrementDemo.NoSynchronize(_src, _predicate);

        [Benchmark]
        public long Lock() => InterlockedIncrementDemo.WithLock(_src, _predicate);

        [Benchmark]
        public long Interlocked() => InterlockedIncrementDemo.InterlockedIncrement(_src, _predicate);
    }

    [MemoryDiagnoser]
    public class InterlockedExchange
    {
        [Benchmark]
        public void Lock() => InterlockedExchangeDemo.CheckWithLock(1_000);

        [Benchmark]
        public void Interlocked() => InterlockedExchangeDemo.CheckWithInterlocked(1_000);
    }
}
