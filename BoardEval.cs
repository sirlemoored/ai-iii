using System;
using System.Collections.Generic;

namespace AI
{
    public class BoardEvaluator
    {
        public byte maxDepth { get; private set; }
        public sbyte maximizerColor { get; private set; }
        public int nodesVisited { get; private set; }
        public BoardEvaluator(byte maxDepth)
        {
            this.maxDepth = maxDepth;
            this.maximizerColor = -1;
            this.nodesVisited = 0;
        }

        public NMMBoard EvaluateAlphaBeta(NMMBoard initial)
        {
            maximizerColor = (sbyte)initial.moveColor;
            nodesVisited = 0;
            return AlphaBetaRec(initial, 1, float.MinValue, float.MaxValue).bestBoard;
        }

        private (float bestVal, NMMBoard bestBoard) AlphaBetaRec(NMMBoard board, byte depth, float alpha, float beta)
        {
            List<NMMBoard> possibleMoves = board.FindPossibleMoves();
            if (depth == maxDepth || possibleMoves.Count == 0)
                return (HeuristicSimple.Calculate(board), board);
            
            nodesVisited++;
            if (board.moveColor == Color.white)
            {
                float value = float.MinValue;
                NMMBoard brd = null;
                foreach (var child in board.FindPossibleMoves())
                {
                    float best = AlphaBetaRec(child, (byte)(depth + 1), alpha, beta).bestVal;
                    if (best > value)
                    {
                        value = best;
                        brd = child;
                    }
                    alpha = MathF.Max(alpha, value);
                    if (alpha >= beta)
                        break;
                }
                return (value, brd);
            }
            else
            {
                float value = float.MaxValue;
                NMMBoard brd = null;
                foreach (var child in board.FindPossibleMoves())
                {
                    float best = AlphaBetaRec(child, (byte)(depth + 1), alpha, beta).bestVal;
                    if (best < value)
                    {
                        value = best;
                        brd = child;
                    }
                    beta = MathF.Min(beta, value);
                    if (alpha >= beta)
                        break;
                }
                return (value, brd);
            }
        }

        public float EvaluateMiniMax(NMMBoard initial)
        {
            maximizerColor = (sbyte)initial.moveColor;
            nodesVisited = 0;
            return MiniMaxRec(initial, 0);
        }

        private float MiniMaxRec(NMMBoard board, byte depth)
        {
            if (depth == maxDepth)
                return (float)(new Random().NextDouble());
            
            nodesVisited++;
            if (board.moveColor == maximizerColor)
            {
                float value = float.MinValue;
                foreach (var child in board.FindPossibleMoves())
                {
                    value = MathF.Max(value, MiniMaxRec(child, (byte)(depth + 1)));
                }
                return value;
            }
            else
            {
                float value = float.MaxValue;
                foreach (var child in board.FindPossibleMoves())
                {
                    value = MathF.Min(value, MiniMaxRec(child, (byte)(depth + 1)));
                }
                return value;
            }
        }
    }

    public class HeuristicSimple
    {
        public static float Calculate(NMMBoard board)
        {
            return board.freePawns[NMMBoard.ColorToIndex(Color.white)] - board.freePawns[NMMBoard.ColorToIndex(Color.black)]
                 + 5 * board.millsTotal[NMMBoard.ColorToIndex(Color.white)] - 5 * board.millsTotal[NMMBoard.ColorToIndex(Color.black)];
        }
    }
}