using System;

namespace TestNetStandrdLib
{
    internal class Program
    {
        static void Main(string[] args)
        {
            
            Console.WriteLine("Hello World!");
            NetStandardLib.Connect2Hub.Connect();
        }
    }
}
