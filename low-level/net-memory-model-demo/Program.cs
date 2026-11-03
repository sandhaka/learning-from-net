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
//    - ARM64: volatile emits real instructions that constrain the CPU:
//        ldar   load-acquire. Volatile.Read; later accesses cannot become visible before it.
//        ldapr  weaker load-acquire, used instead of ldar when the CPU supports it (ARMv8.3+).
//               Same guarantee for later accesses, but it does not wait for an earlier stlr.
//        stlr   store-release. Volatile.Write; earlier accesses become visible before it.
//        dmb    data memory barrier, a full fence (Interlocked.MemoryBarrier).
//    - So volatile does NOT make the core wait for its store buffer to drain.
//    - In IL, volatile is only a marker for the JIT; IL itself has no notion of reordering:
//
//        C#                          IL
//        _stop                       ldsfld   bool _stop
//        volatile field _stop        volatile. ldsfld bool modreq(IsVolatile) _stop
//        Volatile.Read(ref _stop)    ldsflda  bool _stop
//                                    call     bool Volatile::Read(bool&)    // JIT intrinsic
//
//      The IL keeps every access in program order. All the reordering discussed here happens
//      later, when the JIT turns IL into machine code (compiled order), or in the CPU (visibility
//      order). That is why the demos below show the x64 code, not just the IL.
//
// 5. Full fence
//
//    - Interlocked.MemoryBarrier(), any Interlocked.* operation, lock, Barrier, etc.
//    - x64: compiles to "lock or dword ptr [rsp], 0". The core cannot run any later load until its
//      store buffer has drained into the (coherent) cache. That rules out store -> load reordering.
//
// 6. Reading the listings in Part 2
//
//    IL is stack based: instructions push values onto an evaluation stack and pop them.
//        ldc.i4.1         push the int constant 1 (ldc.i4.0 pushes 0)
//        ldsfld  / stsfld push the value of a static field / pop a value into it
//        ldsflda          push the ADDRESS of a static field (a "ref" argument)
//        ldloc.0/stloc.0  push / pop local variable 0
//        conv.i8          convert the top of the stack to a 64-bit integer (long)
//        call             call a method, arguments taken from the stack
//        br.s, brfalse.s  jump always / jump if the top of the stack is false (0)
//        volatile.        prefix: the next field access is volatile
//
//    x64 operates on registers (eax, ecx, ...) and memory; [_x] means "the memory at _x".
//        mov  a, b        copy b into a (a load if b is memory, a store if a is memory)
//        movzx a, b       load a byte and zero-extend it into a register
//        cmp / test       compare two values / AND them, setting flags without storing a result
//        je / jne / jmp   jump if equal (zero) / if not equal / always
//        lock or [rsp],0  "lock" makes the instruction a full fence; "or ...,0" changes nothing.
//                         Its only purpose is the fence.
//
//    Tier0 / Tier1 / OSR: .NET first compiles a method quickly without optimizations (Tier0),
//    then recompiles hot code with optimizations (Tier1). OSR (on-stack replacement) switches a
//    loop that is already running to the Tier1 version. The listings show Tier1 code.
//
// 7. Summary
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
// Neither the JIT nor the CPU swaps the instructions. Thread 1's loop body in C#, IL and optimized
// x64 (DOTNET_JitDisasm; addresses replaced by names, "dword ptr" omitted, registers normalized):
//
//   C#                                 IL                                x64 (Tier1)
//   ---------------------------------  --------------------------------  ------------------------
//   [plain fields]
//   _x = 1;                            ldc.i4.1                          mov  [_x], 1
//                                      stsfld   int32 _x
//   _r1 = _y;                          ldsfld   int32 _y                 mov  ecx, [_y]
//                                      stsfld   int32 _r1                mov  [_r1], ecx
//
//   [Volatile.Write / Volatile.Read]
//   Volatile.Write(ref _x, 1);         ldsflda  int32 _x                 mov  [_x], 1
//                                      ldc.i4.1
//                                      call     Volatile::Write
//   _r1 = Volatile.Read(ref _y);       ldsflda  int32 _y                 mov  ecx, [_y]
//                                      call     Volatile::Read
//                                      stsfld   int32 _r1                mov  [_r1], ecx
//
//   [Interlocked.MemoryBarrier()]
//   _x = 1;                            ldc.i4.1                          mov  [_x], 1
//                                      stsfld   int32 _x
//   Interlocked.MemoryBarrier();       call  Interlocked::MemoryBarrier  lock or [rsp], 0
//   _r1 = _y;                          ldsfld   int32 _y                 mov  ecx, [_y]
//                                      stsfld   int32 _r1                mov  [_r1], ecx
//
// - The store comes before the load at every level, in every variant.
// - Volatile changes the IL (a call instead of a field access) but not the x64 code.
// - The fence is the only variant that adds an instruction.
//
// What is delayed is the moment the store becomes VISIBLE to the other core, because it first
// waits in that core's store buffer.
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
//     stays 0. On x64 this is the "lock or" in the table above.
//   - volatile / Volatile.Read / Volatile.Write: NO effect. On x64 they emit exactly the same
//     instructions (see the table), and acquire/release allows store -> load reordering anyway
//     (see theory, 4).
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
//
// The spinner thread in C#, IL and optimized x64 (same conventions as StoreBufferingDemo):
//
//   C#                                 IL                                x64 (Tier1)
//   ---------------------------------  --------------------------------  ------------------------
//   [plain field]
//   long n = 0;                        ldc.i4.0 / conv.i8 / stloc.0      (n removed)
//                                      br.s     COND
//   while (!_stop)                     COND: ldsfld bool _stop           movzx eax, byte [_stop]
//                                            brfalse.s LOOP              test  eax, eax
//                                                                        jne   EXIT
//   { n++; }                           LOOP: ldloc.0 / ... / stloc.0     SPIN: jmp SPIN
//
//   [Volatile.Read]
//   long n = 0;                        ldc.i4.0 / conv.i8 / stloc.0      (n removed)
//                                      br.s     COND
//   while (!Volatile.Read(ref _stop))  COND: ldsflda bool _stop          cmp   byte [_stop], 0
//                                            call  Volatile::Read        jne   EXIT
//                                            brfalse.s LOOP
//   { n++; }                           LOOP: ldloc.0 / ... / stloc.0     SPIN: cmp byte [_stop], 0
//                                                                              je  SPIN
//
// - In IL, both variants load _stop on every iteration (COND runs after each LOOP).
// - Plain: the JIT reads _stop once before the loop; SPIN jumps to itself with no load. n is
//   removed because nothing reads it.
// - Volatile.Read: SPIN reloads _stop on every iteration. It is still a plain cmp, no fence.
// - A volatile field gives the same x64 as Volatile.Read; only the IL differs (theory, 4).
// - The first (Tier0) code is unoptimized and does reload _stop. The loop is then moved to
//   optimized code (OSR) while it runs, and from that point it never sees the write.
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
