// Run in Release, without a debugger attached: dotnet run -c Release

// ---------------------------------------------------------------------------------------------
// StoreBufferingDemo - reading guide
// ---------------------------------------------------------------------------------------------
//
// The CPU does not swap the instructions: each core executes them in program order (store, then
// load). What is delayed is the moment the store becomes VISIBLE to the other core, because it
// first waits in that core's private store buffer. To the other thread it looks as if the load
// had executed first.
//
// Hardware view (start: memory x=0, y=0)
//
//   #  Core 1 (t1)             Buffer 1   Memory      Buffer 2   Core 2 (t2)
//   1  _x = 1                  x=1        x=0, y=0
//   2                          x=1        x=0, y=0    y=1        _y = 1
//   3  _r1 = _y  -> 0          x=1        x=0, y=0    y=1
//      (no y in buffer 1, reads memory)
//   4                          x=1        x=0, y=0    y=1        _r2 = _x  -> 0
//                                                                (no x in buffer 2, reads memory)
//   5  (buffer drains)                    x=1, y=0    y=1
//   6                                     x=1, y=1               (buffer drains)
//   7  Round.SignalAndWait() -> barrier sees _r1 == 0 && _r2 == 0 -> _bothZero++
//
// Both loads (steps 3-4) ran while both stores were still sitting in the store buffers.
//
// The same thing, seen as reordered code (what the other thread observes):
//
//   Thread 1            Thread 2
//   _r1 = _y;  // 0     _r2 = _x;  // 0
//   _x = 1;             _y = 1;
//
// One delayed store is enough - thread 1 alone can cause it:
//
//   1. t1: _x = 1     -> sits in buffer 1, invisible to core 2
//   2. t1: _r1 = _y   -> 0  (t2 has not written yet)
//   3. t2: _y = 1     -> becomes visible normally
//   4. t2: _r2 = _x   -> 0  (t1's x=1 is still in its buffer)
//   5. buffer 1 drains: x = 1, too late
//
// Fix: Interlocked.MemoryBarrier() between the store and the load makes the core wait until its
// store buffer has drained before loading. Whichever load runs last then sees both stores and
// reads 1, so _bothZero stays 0.
//
// x64 (TSO) only allows this one reordering: a store followed by a load from a different address.
// Note: volatile / Volatile.Read / Volatile.Write do NOT fix it (acquire/release still allows it).
// ---------------------------------------------------------------------------------------------
internal static class StoreBufferingDemo
{
	// Each thread writes its own variable, then reads the other one.
	// In program order at least one thread must read 1, yet x64 lets a store be delayed past a later load.
	private static int _x, _y, _r1, _r2, _bothZero;

	// The barrier ends each round: once both threads arrive, it checks the result,
	// resets the variables, and releases both threads into the next round together.
	private static readonly Barrier Round = new(2, _ =>
	{
        // Impossible condition
		if (_r1 == 0 && _r2 == 0)
		{
			_bothZero++;
		}

        // Demo reset
		_x = 0;
		_y = 0;
	});

	public static void Run(int rounds)
	{
		var t1 = new Thread(() =>
		{
			for (int i = 0; i < rounds; i++)
			{
				_x = 1;
				// Interlocked.MemoryBarrier();   // uncomment in both threads: the count drops to 0
				_r1 = _y;
				Round.SignalAndWait(); // Force CPU buffers drains in memory
			}
		});

		var t2 = new Thread(() =>
		{
			for (int i = 0; i < rounds; i++)
			{
				_y = 1;
				// Interlocked.MemoryBarrier();
				_r2 = _x;
				Round.SignalAndWait(); // Force CPU buffers drains in memory
			}
		});

		t1.Start();
		t2.Start();
		t1.Join();
		t2.Join();

		Console.WriteLine($"[CPU] Both threads read 0 in {_bothZero} of {rounds} rounds");
	}
}

internal static class LoopHoistingDemo
{
	// The JIT reads the non-volatile flag once and hoists it out of the loop,
	// so the spinning thread never observes the write. Fix: Volatile.Read or a volatile field.
	private static bool _stop;

	public static void Run()
	{
		var spinner = new Thread(() =>
		{
			long n = 0;
			while (!_stop)
			{
				n++;
			}
		}) { IsBackground = true };

		spinner.Start();
		Thread.Sleep(1000);
		_stop = true;

		Console.WriteLine(spinner.Join(2000)
			? "[JIT] loop saw stop"
			: "[JIT] loop never saw stop (read hoisted by JIT)");
	}
}

internal static class Program
{
	private static void Main()
	{
		//StoreBufferingDemo.Run(1_000_000);
		LoopHoistingDemo.Run();
	}
}
