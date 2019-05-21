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
            b.AddPawn(1, Color.white);
            b.AddPawn(10, Color.white);
            b.AddPawn(22, Color.black);
            b.AddPawn(23, Color.black);
            b.moveColor = Color.white;
            System.Console.WriteLine(b.PrintBoard());
            foreach(var x in b.FindPositionsFlyingPawns())
            {
                if (x.millMoves == 1)
                Console.WriteLine(x.PrintBoard());
            }
        }
    }
}
