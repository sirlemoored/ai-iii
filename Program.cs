using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace AI
{
    class Program
    {

        static void Main(string[] args)
        {
            NMMBoard b = new NMMBoard();
            b.moveColor = Color.white;

            for (int i = 0; i < 10; i++)
            {
                Stopwatch s = new Stopwatch();
                BoardEvaluator eval = new BoardEvaluator(3);
                s.Start();
                b = eval.EvaluateAlphaBeta(b);
                s.Stop();
                System.Console.WriteLine(i + " >>> " + s.Elapsed);
                System.Console.WriteLine(b.PrintBoard());
                
            }



            Console.Read();
        }
    }
}
