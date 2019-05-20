using System;
using System.Text;
using System.Linq;
using System.Collections.Generic;

namespace AI 
{
    public partial class NMMBoard
    {
        public static sbyte ColorToIndex(byte color)
        {
            if (color == Color.white)
                return 0;
            else if (color == Color.black)
                return 1;
            else
                return -1;
        }

        public static byte ColorToEnemyColor(byte color)
        {
            if (color == Color.white)
                return Color.black;
            else if (color == Color.black)
                return Color.white;
            else
                return Color.empty;
        }

        public static byte ColorToMillColor(byte color)
        {
            if (color == Color.white)
                return Color.whiteMill;
            else if (color == Color.black)
                return Color.blackMill;
            else return Color.empty;
        }

        public static sbyte ColorToPreviousMoveOrigin(byte color)
        {
            if (color == Color.white)
                return 0;
            else if (color == Color.black)
                return 2;
            else
                return -1;
        }

        public static sbyte ColorToPreviousMoveDestination(byte color)
        {
            if (color == Color.white)
                return 1;
            else if (color == Color.black)
                return 3;
            else
                return -1;
        }

        public bool IsFieldPartOfMill(byte index, byte[] mill, byte color)
        {
            if (fields[index] != color && fields[index] != ColorToMillColor(color))
                return false;
            bool isMillPossible = true;
                if (fields[mill[0]] != color && fields[mill[0]] != ColorToMillColor(color))
                    isMillPossible = false;
                else
                    if (fields[mill[1]] != color && fields[mill[1]] != ColorToMillColor(color))
                        isMillPossible = false;
            return isMillPossible;
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append($"Liczba białych wolnych pionów: {this.freePawns[0]}\n");
            sb.Append($"Liczba czarnych wolnych pionów: {this.freePawns[1]}\n");
            sb.Append($"Liczba białych młynków: {this.millsTotal[0]}\n");
            sb.Append($"Liczba czarnych młynków: {this.millsTotal[1]}\n");
            return sb.ToString();
        }

        public string PrintBoard()
        {
            StringBuilder sb = new StringBuilder();
            Func<int,string> f = (int a) => string.Format("{0, 2}", a);
            sb.Append(f(fields[21]) + "-----" + f(fields[22]) + "-----" + f(fields[23]) + "\n");
            sb.Append(" |" + f(fields[18]) + "---" + f(fields[19]) + "---" + f(fields[20]) + " |\n");
            sb.Append(" | |" + f(fields[15]) + "-" + f(fields[16]) + "-" + f(fields[17]) + " | |\n");
            sb.Append(f(fields[9]) + f(fields[10]) + f(fields[11]) + "    " + f(fields[12]) + f(fields[13]) + f(fields[14]) + "\n");
            sb.Append(" | |" + f(fields[6]) + "-" + f(fields[7]) + "-" + f(fields[8]) + " | |\n");
            sb.Append(" |" + f(fields[3]) + "---" + f(fields[4]) + "---" + f(fields[5]) + " |\n");
            sb.Append(f(fields[0]) + "-----" + f(fields[1]) + "-----" + f(fields[2]) + "\n");
            return sb.ToString();
        }

    }

    public class GameState
    {
        public const byte over      = 0;
        public const byte placing   = 1;
        public const byte moving    = 2;
        public const byte flying    = 3;
    }

    public class NMMBoardSetup
    {
        public const byte pawnsPerPlayer = 9;
        public const byte flyingPhaseLimit = 3;

        public static byte[][][] mills = new byte[][][]
        {
            new byte[][] { new byte[] { 1, 2 },    new byte[] { 9, 21 }},
            new byte[][] { new byte[] { 0, 2 },    new byte[] { 4, 7 }},
            new byte[][] { new byte[] { 0, 1 },    new byte[] { 14, 23 }},
            new byte[][] { new byte[] { 4, 5 },    new byte[] { 10, 18 }},
            new byte[][] { new byte[] { 3, 5 },    new byte[] { 1, 7 }},
            new byte[][] { new byte[] { 3, 4 },    new byte[] { 13, 20 }},
            new byte[][] { new byte[] { 7, 8 },    new byte[] { 11, 15 }},
            new byte[][] { new byte[] { 6, 8 },    new byte[] { 1, 4 }},
            new byte[][] { new byte[] { 6, 7 },    new byte[] { 12, 17 }},
            new byte[][] { new byte[] { 10, 11 },  new byte[] { 0, 21 }},
            new byte[][] { new byte[] { 9, 11 },  new byte[] { 3, 18 }},
            new byte[][] { new byte[] { 9, 10 },  new byte[] { 6, 15 }},
            new byte[][] { new byte[] { 13, 14 }, new byte[] { 8, 17 }},
            new byte[][] { new byte[] { 12, 14 }, new byte[] { 5, 20 }},
            new byte[][] { new byte[] { 12, 13 }, new byte[] { 2, 23 }},
            new byte[][] { new byte[] { 16, 17 }, new byte[] { 6, 11 }},
            new byte[][] { new byte[] { 15, 17 }, new byte[] { 19, 22 }},
            new byte[][] { new byte[] { 15, 16 }, new byte[] { 8, 12 }},
            new byte[][] { new byte[] { 19, 20 }, new byte[] { 3, 10 }},
            new byte[][] { new byte[] { 18, 20 }, new byte[] { 16, 22 }},
            new byte[][] { new byte[] { 18, 19 }, new byte[] { 5, 13 }},
            new byte[][] { new byte[] { 22, 23 }, new byte[] { 0, 9 }},
            new byte[][] { new byte[] { 21, 23 }, new byte[] { 16, 19 }},
            new byte[][] { new byte[] { 21, 22 },  new byte[] { 2, 14 }}
        };

        public static byte[][] connections = new byte[][]
        {
                new byte[]{ 1, 9 },
                new byte[]{ 0, 2, 4 },
                new byte[]{ 1, 14 },
                new byte[]{ 4, 10 },
                new byte[]{ 1, 3, 5, 7 },
                new byte[]{ 4, 13 },
                new byte[]{ 7, 11 },
                new byte[]{ 4, 6, 8 },
                new byte[]{ 7, 12 },
                new byte[]{ 0, 10, 21 },
                new byte[]{ 3, 9, 11, 18 },
                new byte[]{ 6, 10, 15 },
                new byte[]{ 8, 13, 17 },
                new byte[]{ 5, 14, 20 },
                new byte[]{ 2, 13, 23 },
                new byte[]{ 11, 16 },
                new byte[]{ 15, 17, 19 },
                new byte[]{ 12, 16 },
                new byte[]{ 10, 19 },
                new byte[]{ 16, 18, 20, 22 },
                new byte[]{ 13, 19 },
                new byte[]{ 9, 22 },
                new byte[]{ 19, 21, 23 },
                new byte[]{ 14, 22 }
        };
    }

    public class NMMBoardStateComparer : IEqualityComparer<NMMBoard>
    {
        public bool Equals(NMMBoard b1, NMMBoard b2)
        {
            return b1.fields.SequenceEqual(b2.fields);
        }

        public int GetHashCode(NMMBoard b1)
        {
            return 0;
        }
    }
}