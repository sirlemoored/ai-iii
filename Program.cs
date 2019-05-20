using System;
using System.Diagnostics;

namespace AI
{
    class Program
    {
        static void Main(string[] args)
        {
            NMMBoard b = new NMMBoard();
            b.AddPawn(2, Color.white);
            b.AddPawn(4, Color.white);
            b.AddPawn(7, Color.white);
            b.AddPawn(1, Color.white);
            b.AddPawn(0, Color.white);
            b.AddPawn(9, Color.black);
            b.AddPawn(21, Color.black);
            Stopwatch sw = new Stopwatch();
            sw.Start();
            for (int i = 0; i < 10000000; i++)
            {
                NMMBoard b2 = new NMMBoard(b);
            }
            sw.Stop();
            Console.WriteLine(sw.ElapsedMilliseconds);
            Console.WriteLine(b.PrintBoard());
            Console.WriteLine(b.ToString());
            Console.Read();
        }
    }
}
