using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Chess6x6AI
{
    public class BitBoard
    {
        public int turn;
        public bool wturn;
        public ulong wPawn;
        public ulong bPawn;
        //public ulong wBishop;
        //public ulong bBishop;
        public ulong wKnight;
        public ulong bKnight;
        public ulong wRook;
        public ulong bRook;
        public ulong wQueen;
        public ulong bQueen;
        public ulong wKing;
        public ulong bKing;
        public ulong wall;
        public ulong ball;
        public bool wcheck;
        public bool bcheck;
        [MethodImpl(MethodImplOptions.AggressiveOptimization | MethodImplOptions.AggressiveInlining)]
        public BitBoard(bool wturn, /*ulong wBishop, ulong bBishop,*/ ulong wKnight, ulong bKnight, ulong wRook, ulong bRook, ulong wQueen, ulong bQueen, ulong wKing, ulong bKing, ulong wPawn = (1uL << 6) * 63, ulong bPawn = (1uL << 24) * 63, int turn = 0)
        {
            this.wturn = wturn;
            this.wPawn = wPawn;
            this.bPawn = bPawn;
            //this.wBishop = wBishop;
            //this.bBishop = bBishop;
            this.wKnight = wKnight;
            this.bKnight = bKnight;
            this.wRook = wRook;
            this.bRook = bRook;
            this.wQueen = wQueen;
            this.bQueen = bQueen;
            this.wKing = wKing;
            this.bKing = bKing;
            this.wall = wPawn | wKnight | wRook | wQueen | wKing;//| wBishop 
            this.ball = bPawn | bKnight | bRook | bQueen | bKing;//| bBishop 
            this.wcheck = false;
            this.bcheck = false;
            this.turn = turn;
        }
        [MethodImpl(MethodImplOptions.AggressiveOptimization | MethodImplOptions.AggressiveInlining)]
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            string yokosen = "-+-+-+-+-+-+-+";
            for (int i = 0; i < 6; i++)
            {
                sb.AppendLine(yokosen);
                sb.Append((6 - i).ToString() + "|");
                for (int j = 0; j < 6; j++)
                {
                    ulong position = 1uL << (30 - 6 * i + j);
                    position &= wall | ball | wKing | bKing;
                    char box = ' ';
                    if ((wPawn & position) > 0) box = 'P';
                    if ((bPawn & position) > 0) box = 'p';
                    //if ((wBishop & position) > 0) box = 'B';
                    //if ((bBishop & position) > 0) box = 'b';
                    if ((wKnight & position) > 0) box = 'N';
                    if ((bKnight & position) > 0) box = 'n';
                    if ((wRook & position) > 0) box = 'R';
                    if ((bRook & position) > 0) box = 'r';
                    if ((wQueen & position) > 0) box = 'Q';
                    if ((bQueen & position) > 0) box = 'q';
                    if ((wKing & position) > 0) box = 'K';
                    if ((bKing & position) > 0) box = 'k';
                    sb.Append(box);
                    sb.Append("|");
                }
                sb.AppendLine();
            }
            sb.AppendLine(yokosen);
            sb.Append(" |");
            for (int j = 0; j < 6; j++)
            {
                sb.Append((char)('A' + j) + "|");
            }
            return sb.ToString();
        }

        [MethodImpl(MethodImplOptions.AggressiveOptimization | MethodImplOptions.AggressiveInlining)]
        public override bool Equals(object? obj)
        {
            if(obj == null || !(obj is BitBoard)) return false;
            return 
                this.turn == ((BitBoard)obj).turn &
                this.wturn == ((BitBoard)obj).wturn &
                this.wPawn == ((BitBoard)obj).wPawn &
                this.bPawn == ((BitBoard)obj).bPawn &
                //this.wBishop == ((BitBoard)obj).wBishop&
                //this.bBishop == ((BitBoard)obj).bBishop&
                this.wKnight == ((BitBoard)obj).wKnight &
                this.bKnight == ((BitBoard)obj).bKnight &
                this.wRook == ((BitBoard)obj).wRook &
                this.bRook == ((BitBoard)obj).bRook &
                this.wQueen == ((BitBoard)obj).bQueen &
                this.bQueen == ((BitBoard)obj).bQueen &
                this.wKing == ((BitBoard)obj).wKing &
                this.bKing == ((BitBoard)obj).bKing 
                ;
        }

        [MethodImpl(MethodImplOptions.AggressiveOptimization | MethodImplOptions.AggressiveInlining)]
        public override int GetHashCode()
        {
            ulong all =wall | ball | wKing | bKing;
            return (int)(0xffff&((all >> 32) ^ all));
        }

        [MethodImpl(MethodImplOptions.AggressiveOptimization | MethodImplOptions.AggressiveInlining)]
        public (bool[],bool[]) halfKP()
        {
            //not yet
            bool[] p1 = new bool[36 * 36 * 8];
            bool[] p2 = new bool[36 * 36 * 8];
            int wkingpos = BitOperations.Log2(this.wKing);
            int bkingpos = BitOperations.Log2(this.bKing);
            if (wturn)
            {
                int p1offset = 36 * 8 * wkingpos;
                int p2offset = 36 * 8 * bkingpos;
                for(int i = 0; i < 36; i++)
                {
                    if ((this.wPawn & (1ul << i)) != 0)
                    {
                        p1[p1offset + i] = true;
                        p2[p2offset + i] = true;
                    }
                    if ((this.bPawn & (1ul << i)) != 0)
                    {
                        p1[p1offset + i + 36] = true;
                        p2[p2offset + i + 36] = true;
                    }
                    if ((this.wKnight & (1ul << i)) != 0)
                    {
                        p1[p1offset + i + 72] = true;
                        p2[p2offset + i + 72] = true;
                    }
                    if ((this.bKnight & (1ul << i)) != 0)
                    {
                        p1[p1offset + i + 108] = true;
                        p2[p2offset + i + 108] = true;
                    }
                    if ((this.wRook & (1ul << i)) != 0)
                    {
                        p1[p1offset + i + 144] = true;
                        p2[p2offset + i + 144] = true;
                    }
                    if ((this.bRook & (1ul << i)) != 0)
                    {
                        p1[p1offset + i + 180] = true;
                        p2[p2offset + i + 180] = true;
                    }
                    if ((this.wQueen & (1ul << i)) != 0)
                    {
                        p1[p1offset + i + 216] = true;
                        p2[p2offset + i + 216] = true;
                    }
                    if ((this.bQueen & (1ul << i)) != 0)
                    {
                        p1[p1offset + i + 252] = true;
                        p2[p2offset + i + 252] = true;
                    }
                }
            }
            else
            {
                int newbkingpos = (5-bkingpos / 6)*6 + bkingpos % 6;
                int newwkingpos = (5 - wkingpos / 6) * 6 + wkingpos % 6;
                int p1offset = 36 * 8 * newbkingpos;
                int p2offset = 36 * 8 * newwkingpos;
                for (int j = 0; j < 36; j++)
                {
                    int i = (5-j/6)*6+j%6;
                    if ((this.bPawn & (1ul << i)) != 0)
                    {
                        p1[p1offset + i] = true;
                        p2[p2offset + i] = true;
                    }
                    if ((this.wPawn & (1ul << i)) != 0)
                    {
                        p1[p1offset + i + 36] = true;
                        p2[p2offset + i + 36] = true;
                    }
                    if ((this.bKnight & (1ul << i)) != 0)
                    {
                        p1[p1offset + i + 72] = true;
                        p2[p2offset + i + 72] = true;
                    }
                    if ((this.wKnight & (1ul << i)) != 0)
                    {
                        p1[p1offset + i + 108] = true;
                        p2[p2offset + i + 108] = true;
                    }
                    if ((this.bRook & (1ul << i)) != 0)
                    {
                        p1[p1offset + i + 144] = true;
                        p2[p2offset + i + 144] = true;
                    }
                    if ((this.wRook & (1ul << i)) != 0)
                    {
                        p1[p1offset + i + 180] = true;
                        p2[p2offset + i + 180] = true;
                    }
                    if ((this.bQueen & (1ul << i)) != 0)
                    {
                        p1[p1offset + i + 216] = true;
                        p2[p2offset + i + 216] = true;
                    }
                    if ((this.wQueen & (1ul << i)) != 0)
                    {
                        p1[p1offset + i + 252] = true;
                        p2[p2offset + i + 252] = true;
                    }
                }
            }
            return (p1, p2);
        }
    }
}
