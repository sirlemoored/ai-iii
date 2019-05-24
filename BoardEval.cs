using System;
using System.Linq;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace AI
{
    public class BoardEvaluator
    {
        public byte maxDepth { get; private set; }
        public byte maximizerColor { get; private set; }
        public int nodesVisited { get; private set; }
        public Stopwatch stopwatch { get; private set; }
        public IHeuristic whiteHeuristic { get; private set; }
        public IHeuristic blackHeuristic { get; private set; }

        public BoardEvaluator(IHeuristic whiteHeuristic, IHeuristic blackHeuristic)
        {
            this.stopwatch = new Stopwatch();
            this.maxDepth = 1;
            this.nodesVisited = 0;
            this.whiteHeuristic = whiteHeuristic;
            this.blackHeuristic = blackHeuristic;
        }

        public NMMBoard EvaluateAlphaBeta(NMMBoard initial, byte depth)
        {
            this.maxDepth = depth;
            this.maximizerColor = initial.moveColor;
            nodesVisited = 0;
            stopwatch.Restart();
            NMMBoard best = AlphaBetaRec(initial, 1, float.MinValue, float.MaxValue).bestBoard;
            stopwatch.Stop();
            return best;
        }

        private (float bestVal, NMMBoard bestBoard) AlphaBetaRec(NMMBoard board, byte depth, float alpha, float beta)
        {
            List<NMMBoard> possibleMoves = board.FindPossibleMoves();
            if (depth == maxDepth || possibleMoves.Count == 0)
                return (EvaluateBoard(board), board);
            
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

        public NMMBoard EvaluateAlphaBetaTimeLimit(NMMBoard initial, float seconds)
        {
            stopwatch.Restart();
            nodesVisited = 0;
            this.maximizerColor = initial.moveColor;
            NMMBoard best = null;
            for (int i = 1; stopwatch.ElapsedMilliseconds < seconds * 1000; i++)
            {
                this.maxDepth = (byte)i;
                var res = AlphaBetaTimeLimitRec(initial, 1, float.MinValue, float.MaxValue, (int)(seconds * 1000), 0);
                if (res.HasValue)
                    best = res.Value.bestBoard;
            }
            stopwatch.Stop();
            return best;
        }

        private (float bestVal, NMMBoard bestBoard)? AlphaBetaTimeLimitRec(NMMBoard board, byte depth, float alpha, float beta, int limit, long timeStamp)
        {
            if (timeStamp > limit)
                return null;

            List<NMMBoard> possibleMoves = board.FindPossibleMoves();
            if (depth == maxDepth || possibleMoves.Count == 0)
                return (EvaluateBoard(board), board);
            
            nodesVisited++;
            if (board.moveColor == Color.white)
            {
                float value = float.MinValue;
                NMMBoard brd = null;
                foreach (var child in board.FindPossibleMoves())
                {
                    var res = AlphaBetaTimeLimitRec(child, (byte)(depth + 1), alpha, beta, limit, stopwatch.ElapsedMilliseconds);
                    if (!res.HasValue)
                        return null;
                    else
                    {
                        float best = res.Value.bestVal;
                        if (best > value)
                        {
                            value = best;
                            brd = child;
                        }
                        alpha = MathF.Max(alpha, value);
                        if (alpha >= beta)
                            break;
                    }
                }
                return (value, brd);
            }
            else
            {
                float value = float.MaxValue;
                NMMBoard brd = null;
                foreach (var child in board.FindPossibleMoves())
                {
                    var res = AlphaBetaTimeLimitRec(child, (byte)(depth + 1), alpha, beta, limit, stopwatch.ElapsedMilliseconds);
                    if (!res.HasValue)
                        return null;
                    else
                    {
                        float best = res.Value.bestVal;
                        if (best < value)
                        {
                            value = best;
                            brd = child;
                        }
                        beta = MathF.Min(beta, value);
                        if (alpha >= beta)
                            break;
                    }
                }
                return (value, brd);
            }
        }

        private float EvaluateBoard(NMMBoard board)
        {
            if (maximizerColor == Color.white)
                return whiteHeuristic.Evaluate(board);
            else
                return blackHeuristic.Evaluate(board);
        }
    }

    public interface IHeuristic
    {
        float Evaluate(NMMBoard board);
    }

    public class PawnDifferenceHeuristic : IHeuristic
    {
        public float Evaluate(NMMBoard board)
        {
            float result = 0;
            foreach (byte field in board.fields)
            {
                if (field == Color.white || field == Color.whiteMill)
                    result++;
                else if (field == Color.black || field == Color.blackMill)
                    result--;
            }
            return result;
        }
    }

    public class MillDifferenceHeuristic : IHeuristic
    {
        public int k { get; private set;}
        public MillDifferenceHeuristic(int k)
        {
            this.k = k;
        }
        public float Evaluate(NMMBoard board)
        {
            return board.freePawns[NMMBoard.ColorToIndex(Color.white)] - board.freePawns[NMMBoard.ColorToIndex(Color.black)]
                 + k * board.millsTotal[NMMBoard.ColorToIndex(Color.white)] - k * board.millsTotal[NMMBoard.ColorToIndex(Color.black)];
        }
    }

    public class PawnsWithNeighborsDifferenceHeuristic : IHeuristic
    {
        public int k { get; private set; }
        public PawnsWithNeighborsDifferenceHeuristic(int k)
        {
            this.k = k;
        }
        public float Evaluate(NMMBoard board)
        {
            float result = 0;
            for (byte i = 0; i < board.fields.Length; i++)
            {
                int neighbors = board.FindFreeNeighborSpaces(i).Count;
                if (board.fields[i] == Color.white || board.fields[i] == Color.whiteMill)
                {
                    if (neighbors == 0)
                        result++;
                    else
                        result += k;
                }
                else if (board.fields[i] == Color.black || board.fields[i] == Color.blackMill)
                {
                    if (neighbors == 0)
                        result--;
                    else
                        result -= k;
                }
            }
            return result;
        }
    }

    public class PossibleMoveNumberDiffefrenceHeuristic : IHeuristic
    {
        public float Evaluate(NMMBoard board)
        {
            NMMBoard copy = new NMMBoard(board);
            copy.moveColor = Color.white;
            float possibleWhites = 0;
            float possibleBlacks = 0;
            if (board.GetGameState(board.moveColor) != GameState.flying && board.GetGameState(board.moveColor) != GameState.placing)
                possibleWhites = copy.FindPossibleMoves().Count;
            else
                possibleWhites = board.fields.Where(x => x == Color.empty).ToArray().Length * 3 - 1;

            copy.moveColor = Color.black;
            if (board.GetGameState(board.moveColor) != GameState.flying && board.GetGameState(board.moveColor) != GameState.placing)
                possibleBlacks = copy.FindPossibleMoves().Count;
            else
                possibleBlacks = board.fields.Where(x => x == Color.empty).ToArray().Length * 3 - 1;

            return possibleWhites - possibleBlacks;
        }
    }

    public class OccupiedFieldsWeightDifferenceHeuristic : IHeuristic
    {
        public float[] weights;
        public OccupiedFieldsWeightDifferenceHeuristic(float a, float b, float c)
        {
            weights = new float[3];
            weights[0] = a;
            weights[1] = b;
            weights[2] = c;
        }
        public float Evaluate(NMMBoard board)
        {
            float sumWhite = 0, sumBlack = 0;
            for (int i = 0; i < board.fields.Length; i++)
            {
               int numberOfConnections = NMMBoardSetup.connections[i].Length;
               if (board.fields[i] == Color.white || board.fields[i] == Color.whiteMill)
               {
                   sumWhite += weights[numberOfConnections - 2];
               }
               else if (board.fields[i] == Color.black || board.fields[i] == Color.blackMill)
               {
                   sumBlack += weights[numberOfConnections - 2];
               }
            } 
            return sumWhite - sumBlack;
        }
    }
}