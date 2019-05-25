using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace AI
{
    class Program
    {

        static void Main(string[] args)
        {
            BoardEvaluator eval = new BoardEvaluator(new MillDifferenceHeuristic(3), new PawnDifferenceHeuristic());

            byte depth = 6;
            float timeLimit = 2;
            bool threefoldRepetition = false;

            NMMBoard b = new NMMBoard();
            b.moveColor = Color.white;

            List<NMMBoard> history = new List<NMMBoard>();
            long[] timeSpent = new long[] { 0, 0 };
            long[] nodesVisited = new long[] { 0, 0 };
            int totalNumberOfMoves = 0;
            int gameState = -1;

            while(!threefoldRepetition && (gameState = b.GetGameState(b.moveColor)) != GameState.over)
            {   
                b = eval.EvaluateAlphaBetaTimeLimit(b, timeLimit);
                if (gameState != GameState.placing)
                {
                    if (!NMMBoard.CheckForThreefoldRepetition(history, b))
                        history.Add(new NMMBoard(b));
                    else
                        threefoldRepetition = true;
                }

                timeSpent[NMMBoard.ColorToIndex(NMMBoard.ColorToEnemyColor(b.moveColor))] += eval.stopwatch.ElapsedMilliseconds;
                nodesVisited[NMMBoard.ColorToIndex(NMMBoard.ColorToEnemyColor(b.moveColor))] += eval.nodesVisited;
                totalNumberOfMoves++;

                System.Console.WriteLine(b.PrintBoard());
                
            }

            System.Console.WriteLine("Koniec gry!");
            if (threefoldRepetition)
                System.Console.WriteLine("Remis przez trzykrotne powtórzenie!");
            System.Console.WriteLine("Całkowita liczba ruchów: " + totalNumberOfMoves);
            System.Console.WriteLine("Łapsnięte piony przeciwnika: " + b.pawnsCaptured[1] + " / " + b.pawnsCaptured[0]);
            System.Console.WriteLine("Czas (ms): " + timeSpent[0] + " / " + timeSpent[1]);
            System.Console.WriteLine("Węzły: " + nodesVisited[0] + " / " + nodesVisited[1]);

            Console.Read();
        }
    }
}
