using System;
using System.Diagnostics;

namespace AI
{
    class Program
    {
        static void Main(string[] args)
        {
            NMMBoard b = new NMMBoard();
            b.AddPawn(0, Color.white);
            b.AddPawn(2, Color.white);
            b.AddPawn(4, Color.white);
            b.AddPawn(7, Color.white);
            b.AddPawn(21, Color.black);
            b.AddPawn(22, Color.black);
            b.AddPawn(23, Color.black);
            b.moveColor = Color.white;
            foreach(var x in b.FindPositionsPlacingPawns())
            {
                Console.WriteLine(x.PrintBoard());
            }
        }
    }
}
