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
            b.moveColor = Color.black;
            int i = 0;
            while(b.GetGameState(b.moveColor) != GameState.over)
            {   
                i++;
                Stopwatch s = new Stopwatch();
                BoardEvaluator eval = new BoardEvaluator(5);

                System.Console.WriteLine(">>> " + i);
                s.Start();
                b = eval.EvaluateAlphaBeta(b);
                s.Stop();
                System.Console.WriteLine(">>> " + s.Elapsed);
                System.Console.WriteLine(b.PrintBoard());
                
            }


            Console.Read();
        }
    }
}
