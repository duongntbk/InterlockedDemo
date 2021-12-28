This is the sample code for my article about synchronization with Interlocked in C#. You can find it at the following link.

[https://duongnt.com/interlocked-synchronization](https://duongnt.com/interlocked-synchronization)

# Usage

Run the following command to start the benchmark.
```
dotnet run --configuration Release
```

You should then see the following list of tests
```
Available Benchmarks:
  #0 InterlockedAdd
  #1 InterlockedExchange
  #2 InterlockedIncrement
```

Then select the test you want to run by typing its name or number. It's possible to run multiple tests back to back. For example, you can run `InterlockedAdd` and `InterlockedExchange` by entering this.
```
0 1
```

Or

```
InterlockedAdd InterlockedExchange
```

# License

MIT License

https://opensource.org/licenses/MIT
