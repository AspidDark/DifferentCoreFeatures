#define UniverseFeature

using System;
using System.Diagnostics;

namespace ConDemo
{
    //https://www.youtube.com/watch?v=4wHegx1Xy44&t=405s
    public class Program
    {
        public static void Main(string[] args)
        {
            Foo();
            Console.WriteLine("Hello, World!");
        }

        [Conditional("UniverseFeature")]
        static void Foo() =>
            Console.WriteLine("Hello, Universe!");

#if feture
        static void Foo2() =>
          Console.WriteLine("Hello, Universe!");
#endif
    }
}