using BenchmarkDotNet.Running;
using System;

namespace BenchmarkExample
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Start");
            BenchmarkRunner.Run<DateParserBenchmarks>();
            Console.ReadKey(); ;
        }
    }
}
