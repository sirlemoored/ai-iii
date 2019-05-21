using System;

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

        public float EvaluateAlphaBeta(NMMBoard initial)
        {
            maximizerColor = (sbyte)initial.moveColor;
            nodesVisited = 0;
            return AlphaBetaRec(initial, 0, float.MinValue, float.MaxValue);
        }

        private float AlphaBetaRec(NMMBoard board, byte depth, float alpha, float beta)
        {
            if (depth == maxDepth)
                return (float)(new Random().NextDouble());
            
            nodesVisited++;
            if (board.moveColor == maximizerColor)
            {
                float value = float.MinValue;
                foreach (var child in board.FindPossibleMoves())
                {
                    value = MathF.Max(value, AlphaBetaRec(child, (byte)(depth + 1), alpha, beta));
                    alpha = MathF.Max(alpha, value);
                    if (alpha >= beta)
                        break;
                }
                return value;
            }
            else
            {
                float value = float.MaxValue;
                foreach (var child in board.FindPossibleMoves())
                {
                    value = MathF.Min(value, AlphaBetaRec(child, (byte)(depth + 1), alpha, beta));
                    beta = MathF.Min(beta, value);
                    if (alpha >= beta)
                        break;
                }
                return value;
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
}