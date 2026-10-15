// Run in Release, without a debugger attached: dotnet run -c Release

// =============================================================================================
// PART 1 - PRELIMINARY THEORY
// =============================================================================================
//
// 1. Three different "orders"
//
//    - Program order:     the order of the statements in the C# source.
//    - Compiled order:    the order of the instructions the JIT emits. The JIT may reorder, merge,
//                         or remove memory accesses, or keep a value in a register.
//                         -> LoopHoistingDemo
//    - Visibility order:  the order in which OTHER cores see those accesses. The CPU may let one
//                         become visible later than another, even if the instructions ran in order.
//                         -> StoreBufferingDemo
//
// 2. Where a store goes: registers, store buffer, cache, RAM
//
//    Core 1                          Core 2
//    registers                       registers           private, the JIT decides what lives here
//       |                               |
//    store buffer                    store buffer        private, per core, NOT coherent
//       |                               |
//    L1 / L2 cache  <== coherent ==> L1 / L2 cache       shared view, kept in sync by MESI
//              \                     /
//               L3 cache  ->  RAM
//
//    - Store buffer: a write is first placed here so the core does not have to wait for the cache.
//      Only the owning core sees it (store forwarding). Other cores cannot.
//    - Cache: caches ARE coherent. Once a store reaches L1, every core sees it, even though it is
//      nowhere near RAM yet. "Flushing to main memory" is never the issue.
//    - The store buffer drains into the cache by itself a few nanoseconds later. The store is not
//      lost; a later load just does not wait for it.
//
// 3. Hardware memory models
//
//    - x64 (TSO, total store order): loads stay in order with loads, stores with stores, and a load
//      with a later store. The only reordering allowed is a store followed by a load from a
//      different address. This is the store buffer at work.
//    - ARM64: weakly ordered. Almost any pair of accesses can be reordered.
//
// 4. What volatile means in .NET
//
//    - Volatile.Read  = acquire: memory operations written after it cannot move before it.
//    - Volatile.Write = release: memory operations written before it cannot move after it.
//    - Store followed by load: neither rule applies, so volatile allows it to reorder.
//    - x64: every ordinary mov already has acquire/release behavior, so volatile emits the SAME
//      instructions as plain code. Here volatile only restricts the JIT: no keeping the value in a
//      register, no removing the access, no moving it across other accesses.
//    - ARM64: volatile emits real instructions (ldar / stlr / dmb) that constrain the CPU.
//    - So volatile does NOT make the core wait for its store buffer to drain.
//
// 5. Full fence
//
//    - Interlocked.MemoryBarrier(), any Interlocked.* operation, lock, Barrier, etc.
//    - x64: compiles to "lock or dword ptr [rsp], 0". The core cannot run any later load until its
//      store buffer has drained into the (coherent) cache. That rules out store -> load reordering.
//
// 6. Summary
//
//    Demo                Who reorders               Where                       Fix
//    StoreBufferingDemo  CPU                        store buffer -> cache       full fence
//    LoopHoistingDemo    JIT                        value kept in a register    volatile
//
// =============================================================================================
// PART 2 - CODE COMMENTS
// =============================================================================================

// ---------------------------------------------------------------------------------------------
// StoreBufferingDemo - reading guide
// ---------------------------------------------------------------------------------------------
//
// Each thread writes its own variable, then reads the other one:
//
//   Thread 1            Thread 2
//   _x = 1;             _y = 1;
//   _r1 = _y;           _r2 = _x;
//
// In program order, whichever load runs last comes after both stores, so it reads 1.
// _r1 == 0 && _r2 == 0 should therefore be impossible.
//
// Neither the JIT nor the CPU swaps the instructions: the generated code is "mov [x],1 / mov eax,[y]",
// executed in that order. What is delayed is the moment the store becomes VISIBLE to the other
// core, because it first waits in that core's store buffer.
//
// Hardware view (start: cache x=0, y=0)
//
//   #  Core 1 (t1)             Buffer 1   L1 cache    Buffer 2   Core 2 (t2)
//                                         (coherent)
//   1  _x = 1                  x=1        x=0, y=0
//   2                          x=1        x=0, y=0    y=1        _y = 1
//   3  _r1 = _y  -> 0          x=1        x=0, y=0    y=1
//      (no y in buffer 1, reads cache)
//   4                          x=1        x=0, y=0    y=1        _r2 = _x  -> 0
//                                                                (no x in buffer 2, reads cache)
//   5  (buffer drains)                    x=1, y=0    y=1
//   6                                     x=1, y=1               (buffer drains)
//   7  Round.SignalAndWait() -> barrier sees _r1 == 0 && _r2 == 0 -> _bothZero++
//
// Both loads (steps 3-4) ran while both stores were still in the store buffers. The cache was
// always consistent; it simply did not have the new values yet.
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
//   3. t2: _y = 1     -> reaches the cache normally
//   4. t2: _r2 = _x   -> 0  (t1's x=1 is still in its buffer)
//   5. buffer 1 drains: x = 1, too late
//
// Fixes:
//   - Interlocked.MemoryBarrier() between store and load: the core waits until its store buffer
//     has drained before loading. Whichever load runs last then sees both stores, so _bothZero
//     stays 0.
//   - volatile / Volatile.Read / Volatile.Write: NO effect. On x64 they emit exactly the same
//     instructions, and acquire/release allows store -> load reordering anyway (see theory, 4).
// ---------------------------------------------------------------------------------------------
internal static class StoreBufferingDemo
{
	private static int _x, _y, _r1, _r2, _bothZero;

	// The barrier ends each round: once both threads arrive, it checks the result,
	// resets the variables, and releases both threads into the next round together.
	private static readonly Barrier Round = new(2, _ =>
	{
		// The "impossible" outcome
		if (_r1 == 0 && _r2 == 0)
		{
			_bothZero++;
		}

		// Reset for the next round
		_x = 0;
		_y = 0;
	});

	public static void Run(int rounds)
	{
		var t1 = new Thread(() =>
		{
			for (int i = 0; i < rounds; i++)
			{
				_x = 1;                        // goes to core 1's store buffer
				// Interlocked.MemoryBarrier();   // uncomment in both threads: the count drops to 0
				_r1 = _y;                      // may run before _x = 1 is visible to core 2
				Round.SignalAndWait();         // full fence: buffers drained, results visible to the check
			}
		});

		var t2 = new Thread(() =>
		{
			for (int i = 0; i < rounds; i++)
			{
				_y = 1;                        // goes to core 2's store buffer
				// Interlocked.MemoryBarrier();
				_r2 = _x;                      // may run before _y = 1 is visible to core 1
				Round.SignalAndWait();         // full fence: buffers drained, results visible to the check
			}
		});

		t1.Start();
		t2.Start();
		t1.Join();
		t2.Join();

		Console.WriteLine($"[CPU] Both threads read 0 in {_bothZero} of {rounds} rounds");
	}
}

// ---------------------------------------------------------------------------------------------
// LoopHoistingDemo - reading guide
// ---------------------------------------------------------------------------------------------
//
// This one is a compiler (JIT) reordering, not a CPU one. The JIT sees that nothing in the loop
// writes _stop, so it reads the field once before the loop and keeps the value in a register:
//
//   if (!_stop) { while (true) { n++; } }
//
// The write from the main thread reaches the coherent cache normally, but the loop never reads
// memory again. Fix: Volatile.Read(ref _stop) or a volatile field. This forces the JIT to emit a
// real load on every iteration (see theory, 4).
// ---------------------------------------------------------------------------------------------
internal static class LoopHoistingDemo
{
	private static bool _stop;

	public static void Run()
	{
		var spinner = new Thread(() =>
		{
			long n = 0;
			while (!_stop)                     // read once and hoisted out of the loop by the JIT
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
		StoreBufferingDemo.Run(1_000_000);
		LoopHoistingDemo.Run();
	}
}
