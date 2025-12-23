using System;
using System.Collections.Immutable;

namespace ImmutableObjectsReferencesSamples
{
    internal class Program
    {
        /// <summary>
        /// Demonstrating the immutable object usages in .NET 4.8
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {
            var imSharedObject = new ImmutableArray<int> { 1, 2, 3, 4, 5 };

            var shared = new SharedObject(imSharedObject);
            var contenders = new SampleContender[]
            {
                new SampleContender(shared),
                new SampleContender(shared)
            };

            var cont1 = contenders[0];
            var cont2 = contenders[1];

            Console.WriteLine($"{shared}");
        }
    }
}
