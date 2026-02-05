using System;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;

namespace ActionMethodsBmk
{
    [SimpleJob(RuntimeMoniker.Net481)]
    [MemoryDiagnoser]
    public class ActionsBmk
    {
        private const int IncrementAmount = 1000;

        public ActionsBmk()
        {
            MyClassWithPropActions.IncrementAction = Increment;
            MyClassWithPropActions.DecrementAction = Decrement;
        }
        
        [Benchmark]
        public int ExecuteActionAsClassProperty()
        {
            for (var i = 0; i < IncrementAmount; i++)
                MyClassWithPropActions.IncrementAction();
            for (var i = 0; i < IncrementAmount; i++)
                MyClassWithPropActions.DecrementAction();
            
            return MyClass.Counter;
        }
        
        [Benchmark(Baseline = true)]
        public int ExecuteActionAsClassMethod()
        {
            for (var i = 0; i < IncrementAmount; i++)
                MyClassWithMethods.Increment();
            for (var i = 0; i < IncrementAmount; i++)
                MyClassWithMethods.Decrement();
            
            return MyClass.Counter;
        }
        
        private static void Increment()
        {
            MyClass.Increment();
        }
        
        private static void Decrement()
        {
            MyClass.Decrement();
        }
    }
    
    internal static class MyClassWithPropActions
    {
        public static Action IncrementAction { get; set; }
        public static Action DecrementAction { get; set; }
    }
    
    internal static class MyClassWithMethods
    {
        public static void Increment()
        {
            MyClass.Increment();
        }
        
        public static void Decrement()
        {
            MyClass.Decrement();
        }
    }
    
    internal static class MyClass
    {
        internal static int Counter { get; private set; } = 0;

        public static void Increment()
        {
            Counter++;
        }
        
        public static void Decrement()
        {
            Counter--;
        }
    }
}