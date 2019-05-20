using System;
using System.Linq;
using System.Collections.Generic;

namespace AI
{
    public partial class NMMBoard
    {
        private const int _capacity = 24;
        public byte[] fields         { get; private set; }
        public sbyte[] pawnsPlaced   { get; private set; }
        public sbyte[] pawnsCaptured { get; private set; }
        public sbyte[] millsTotal    { get; private set; }
        public sbyte[] freePawns     { get; private set; }
        public sbyte[] previousMoves { get; private set; }
        public byte moveColor        { get; private set; }
        public byte millMoves        { get; private set; }

        public NMMBoard()
        {
            fields = new byte[_capacity];
            pawnsPlaced = new sbyte[] { 0, 0 };
            pawnsCaptured = new sbyte[] { 0, 0 };
            millsTotal = new sbyte[] { 0, 0 };
            freePawns = new sbyte[] { 0, 0 };
            previousMoves = new sbyte[] { -1, -1, -1, -1};
            moveColor = 0;
            millMoves = 0;
        }

        public NMMBoard(NMMBoard other)
        {
            this.fields = new byte[_capacity];
            this.pawnsPlaced = new sbyte[2];
            this.pawnsCaptured = new sbyte[2];
            this.millsTotal = new sbyte[2];
            this.freePawns = new sbyte[2];
            this.previousMoves = new sbyte[4];
            this.moveColor = other.moveColor;
            this.millMoves = other.millMoves;
            Buffer.BlockCopy(other.fields, 0, this.fields, 0, other.fields.Length);
            Buffer.BlockCopy(other.pawnsPlaced, 0, this.pawnsPlaced, 0, other.pawnsPlaced.Length);
            Buffer.BlockCopy(other.pawnsCaptured, 0, this.pawnsCaptured, 0, other.pawnsCaptured.Length);
            Buffer.BlockCopy(other.millsTotal, 0, this.millsTotal, 0, other.millsTotal.Length);
            Buffer.BlockCopy(other.freePawns, 0, this.freePawns, 0, other.freePawns.Length);
            Buffer.BlockCopy(other.previousMoves, 0, this.previousMoves, 0, other.previousMoves.Length);
        }

        public byte GetGameState(byte color)
        {
            if (pawnsPlaced[ColorToIndex(color)] < NMMBoardSetup.pawnsPerPlayer )
                return GameState.placing;
            else if (pawnsCaptured[ColorToIndex(ColorToEnemyColor(color))] == NMMBoardSetup.pawnsPerPlayer - NMMBoardSetup.flyingPhaseLimit)
                return GameState.flying;
            else if (pawnsCaptured[ColorToIndex(ColorToEnemyColor(color))] < NMMBoardSetup.pawnsPerPlayer - NMMBoardSetup.flyingPhaseLimit)
                return GameState.over;
            else
                return GameState.moving;
        }

        public void AddPawn(byte index, byte color)
        {
            millMoves = 0;
            sbyte arrIdx = ColorToIndex(color);
            byte millColor = ColorToMillColor(color);
            fields[index] = color;
            pawnsPlaced[arrIdx]++;
            freePawns[arrIdx]++;

            foreach (byte[] possibleMill in NMMBoardSetup.mills[index])
            {
                if (IsFieldPartOfMill(index, possibleMill, color))
                {
                    millMoves++;
                    millsTotal[arrIdx]++;
                    freePawns[arrIdx] -= 3;
                    if (freePawns[arrIdx] < 0)
                        freePawns[arrIdx] = 0;
                        
                    fields[possibleMill[0]] = millColor;
                    fields[possibleMill[1]] = millColor;
                    fields[index] = millColor;
                }
            }

        }

        public void RemovePawn(byte index, byte color)
        {
            bool isPartOfMill = false;
            
            foreach (byte[] possibleMill in NMMBoardSetup.mills[index])
            {
                byte placeholder = fields[index];
                if (IsFieldPartOfMill(index, possibleMill, color))
                {
                    isPartOfMill = true;
                    millsTotal[ColorToIndex(color)]--;
                }
                fields[index] = Color.empty;
                for (int i = 0; i < possibleMill.Length; i++)
                {
                    byte indexToCheck = possibleMill[i];
                    bool isFieldPartOfMill = false;
                    for (int j = 0; !isFieldPartOfMill && j < NMMBoardSetup.mills[indexToCheck].Length; j++)
                    {
                        isFieldPartOfMill = IsFieldPartOfMill(indexToCheck, NMMBoardSetup.mills[indexToCheck][j], color);
                    }

                    if (!isFieldPartOfMill && fields[indexToCheck] == ColorToMillColor(color))
                    {
                        freePawns[ColorToIndex(color)] ++;
                        fields[indexToCheck] = color;
                    }
                }
                fields[index] = placeholder;
            }
            
            fields[index] = Color.empty;
            if (!isPartOfMill)
                freePawns[ColorToIndex(color)]--;
        }
    
        public void MovePawn(byte origin, byte destination, byte color)
        {            
            RemovePawn(origin, color);
            AddPawn(destination, color);
            previousMoves[ColorToPreviousMoveOrigin(color)] = (sbyte) origin;
            previousMoves[ColorToPreviousMoveDestination(color)] = (sbyte) destination;
            
        }
    
        public List<NMMBoard> FindPositionsPlacingPawns()
        {
            List<NMMBoard> positions = new LinkedList<NMMBoard>();
            
            for (byte i = 0; i < fields.Length; i++)
            {
                if (fields[i] == Color.empty)
                {
                    NMMBoard board = new NMMBoard(this);
                    board.AddPawn(i, moveColor);
                    board.moveColor = ColorToEnemyColor(moveColor);
                    if (board.millMoves == 0)
                        positions.Add(board);
                    else if (board.millMoves == 1)
                        positions.AddRange(board.FindPositionsRemovingOnePawn());
                    else if (board.millMoves == 2)
                        positions.AddRange(board.FindPositionsRemovingTwoPawns());
                }
            }
        }

        public List<NMMBoard> FindPositionsRemovingOnePawn()
        {
            byte enemyColor = ColorToEnemyColor(moveColor);
            sbyte fPawns = freePawns[ColorToIndex(enemyColor)];
            if (fPawns == 0)
            {   
                List<NMMBoard> positions = new List<NMMBoard>();
                byte enemyMillColor = ColorToMillColor(enemyColor);
                sbyte limiter = 0;
                for(byte i = 0; limiter <= fPawns && i < fields.Length; i++)
                {
                    if (fields[i] == enemyMillColor)
                    {
                        NMMBoard board = new NMMBoard(this);
                        board.RemovePawn(i, enemyColor);
                        limiter++;
                    }
                }
                return positions;
            }
            else
            {
                sbyte mPawns = (sbyte)(pawnsPlaced[ColorToIndex(enemyColor)] - pawnsCaptured[ColorToIndex(moveColor)] - fPawns);
                List<NMMBoard> positions = new List<NMMBoard>();
                byte enemyMillColor = ColorToMillColor(enemyColor);
                sbyte limiter = 0;
                for(byte i = 0; limiter <= mPawns && i < fields.Length; i++)
                {
                    if (fields[i] == enemyColor)
                    {
                        NMMBoard board = new NMMBoard(this);
                        board.RemovePawn(i, enemyColor);
                        limiter++;
                    }
                }
                return positions;
            }
        }

        public List<NMMBoard> FindPositionsRemovingTwoPawns()
        {
            
        }
    }
}