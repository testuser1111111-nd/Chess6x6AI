using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http.Headers;
using System.Net.NetworkInformation;
using System.Numerics;
using System.Reflection.Metadata.Ecma335;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Chess6x6AI
{
    public class BoardOps
    {
        //10^-6secを目指したい
        [MethodImpl(MethodImplOptions.AggressiveOptimization | MethodImplOptions.AggressiveInlining)]
        static int CheckLine(BitBoard board, int cur, int fin)
        {
            int d = 0;
            if (fin/6+fin%6-cur/6-cur%6==0)// \
            {
                d = 5;
            }
            else if (fin / 6 - fin % 6 - cur / 6 + cur % 6 == 0)//   /
            {
                d = 7;
            }
            else if ((fin - cur) % 6 == 0)// |
            {
                d = 6;
            }
            else if (fin / 6 == cur / 6)// -
            {
                d = 1;
            }
            else
            {
                return 0;
            }


            ulong check = 0;
            if (fin > cur)
            {
                check = (((1ul << fin) - (1ul << cur)) / ((1ul << d) - 1)) - (1ul << cur);
            }
            else
            {
                check = (((1ul << cur) - (1ul << fin)) / ((1ul << d) - 1)) - (1ul << fin);
            }
            if (((board.wall | board.ball | board.wKing | board.bKing) & check) > 0)
            {
                return 0;
            }
            else
            {
                return d;
            }
        }
        [MethodImpl(MethodImplOptions.AggressiveOptimization | MethodImplOptions.AggressiveInlining)]
        static public BitBoard? NextBoard(BitBoard board, int cur,int fin,char pp)
        {
            Stopwatch sw = Stopwatch.StartNew();
            BitBoard temp = DeepCopy(board);
            ulong curpos = 1;
            ulong finpos = 1;
            curpos <<= cur;
            finpos <<= fin;
            if (curpos == finpos) return null;
            char piece = GetPiece(board, curpos);
            char nextpiece = GetPiece(board, finpos);
            if (piece == ' ') return null;
            if ((char.IsLower(piece) == char.IsLower(nextpiece)) & nextpiece != ' ') return null;
            if (char.IsUpper(piece) ^ board.wturn)return null; 
            if (piece == 'P')
            {
                if (((cur+5 == fin & cur%6!=0)|(cur+7 ==fin & cur%6!=5)) & nextpiece != ' ')
                {
                    Change(ref temp, nextpiece, finpos);
                }
                else if (cur+ 6 != fin | nextpiece != ' ')
                {
                    return null;
                }
                temp.wPawn ^= curpos ^ finpos;
                temp.wall ^= curpos ^ finpos;
                if (fin/6==5)
                {
                    if (char.ToUpper(pp)=='P') return null;
                    Change(ref temp, char.ToUpper(pp), finpos);
                }
            }
            else if (piece == 'p')
            {
                if (((cur - 5 == fin & cur % 6 != 5) | (cur - 7 == fin & cur % 6 != 0)) & nextpiece != ' ')
                {
                    Change(ref temp, nextpiece, finpos);
                }
                else if (cur - 6 != fin | nextpiece != ' ')
                {
                    return null;
                }
                temp.bPawn ^= curpos ^ finpos;
                temp.ball ^= curpos ^ finpos;
                if (fin/6 == 0)
                {
                    if (char.ToUpper(pp) == 'P') return null;
                    Change(ref temp, char.ToLower(pp), finpos);
                }
            }
            else if (piece == 'N')
            {
                if ((Math.Abs(fin/6-cur/6)==2&Math.Abs(fin%6-cur%6)==1)|(Math.Abs(fin/6-cur/6)==1&Math.Abs(fin%6-cur%6)==2))
                {
                    temp.wKnight ^= curpos ^ finpos;
                    temp.wall ^= curpos ^ finpos;
                }
                else
                {
                    return null;
                }
                if(nextpiece !=' ')Change(ref temp, nextpiece,finpos);
            }
            else if(piece == 'n')
            {
                if ((Math.Abs(fin/6-cur/6)==2&Math.Abs(fin%6-cur%6)==1)|(Math.Abs(fin/6-cur/6)==1&Math.Abs(fin%6-cur%6)==2))
                {
                    temp.bKnight ^= curpos ^ finpos;
                    temp.ball ^= curpos ^ finpos;
                }
                else
                {
                    return null;
                }
                if (nextpiece != ' ') Change(ref temp, nextpiece, finpos);
            }
            else if(piece == 'K')
            {
                if (Math.Abs(fin % 6 - cur % 6) <= 1 & Math.Abs(fin / 6 - cur / 6) <= 1)
                {
                    temp.wKing ^= curpos ^ finpos;
                    if (nextpiece != ' ') Change(ref temp, nextpiece, finpos);
                }
                else
                {
                    return null;
                }
            }
            else if(piece == 'k')
            {

                if (Math.Abs(fin % 6 - cur % 6) <= 1 & Math.Abs(fin / 6 - cur / 6) <= 1)
                {
                    temp.bKing ^= curpos ^ finpos;
                    if (nextpiece != ' ') Change(ref temp, nextpiece, finpos);
                }
                else
                {
                    return null;
                }
            }
            else if (piece == 'R')
            {
                int d = CheckLine(board, cur, fin);
                switch (d)
                {
                    case 0: return null;
                    case 5: return null;
                    case 7: return null;
                    default:
                        temp.wRook ^= curpos ^ finpos;
                        temp.wall ^= curpos ^ finpos;
                        if(nextpiece != ' ')Change(ref temp, nextpiece, finpos);
                        break;
                }
            }
            else if (piece == 'r')
            {
                int d = CheckLine(board, cur, fin);
                switch (d)
                {
                    case 0: return null;
                    case 5: return null;
                    case 7: return null;
                    default:
                        temp.bRook ^= curpos ^ finpos;
                        temp.ball ^= curpos ^ finpos;
                        if (nextpiece != ' ') Change(ref temp, nextpiece, finpos);
                        break;
                }
            }
            else if (piece == 'B')
            {
                int d = CheckLine(board, cur, fin);
                switch (d)
                {
                    case 0: return null;
                    case 1: return null;
                    case 6: return null;
                    default:
                        temp.wBishop ^= curpos ^ finpos;
                        temp.wall ^= curpos ^ finpos;
                        if (nextpiece != ' ') Change(ref temp, nextpiece, finpos);
                        break;
                }
            }
            else if(piece == 'b')
            {
                int d = CheckLine(board, cur, fin);
                switch (d)
                {
                    case 0: return null;
                    case 1: return null;
                    case 6: return null;
                    default:
                        temp.bBishop ^= curpos ^ finpos;
                        temp.ball ^= curpos ^ finpos;
                        if (nextpiece != ' ') Change(ref temp, nextpiece, finpos);
                        break;
                }
            }
            else if(piece == 'Q')
            {
                int d = CheckLine(board, cur, fin);
                switch (d)
                {
                    case 0: return null;
                    default:
                        temp.wQueen ^= curpos ^ finpos;
                        temp.wall ^= curpos ^ finpos;
                        if (nextpiece != ' ') Change(ref temp, nextpiece, finpos);
                        break;
                }
            }
            else
            {
                int d = CheckLine(board, cur, fin);
                switch (d)
                {
                    case 0: return null;
                    default:
                        temp.bQueen ^= curpos ^ finpos;
                        temp.ball ^= curpos ^ finpos;
                        if (nextpiece != ' ') Change(ref temp, nextpiece, finpos);
                        break;
                }
            }
            //check whether the player is in check or not
            //*
            bool bcheck = false;
            int bkingpos = BitOperations.Log2(temp.bKing);
            int x1 = bkingpos % 6;
            int y1 = bkingpos / 6;
            (int, int)[] Nd = { (2, 1), (1, 2), (2, -1), (1, -2), (-2, 1), (-1, 2), (-2, -1), (-1, -2) };
            for (int i = 0; i < 8; i++)
            {
                int x2 = x1 + Nd[i].Item1;
                int y2 = y1 + Nd[i].Item2;
                if (0 <= x2 && x2 < 6 && 0 <= y2 && y2 < 6 && (temp.wKnight & (1ul << (y2 * 6 + x2))) > 0) bcheck = true;
            }
            (int, int)[] RQd = { (0, 1), (1, 0), (0, -1), (-1, 0) };
            (int, int)[] BQd = { (1, 1), (1, -1), (-1, -1), (-1, 1) };
            for (int i = 0; i < 4; i++)
            {
                for (int j = 1; j < 6; j++)
                {
                    int x2 = x1 + RQd[i].Item1 * j;
                    int y2 = y1 + RQd[i].Item2 * j;
                    if (x2 < 0 | x2 >= 6 | y2 < 0 | y2 >= 6) break;
                    if (((temp.wRook | temp.wQueen) & (1ul << (y2 * 6 + x2))) > 0) bcheck = true;
                    if (((temp.wall | temp.ball | temp.wKing | temp.bKing) & (1ul << (y2 * 6 + x2))) > 0) break;
                }
            }
            for (int i = 0; i < 4; i++)
            {
                for (int j = 1; j < 6; j++)
                {
                    int x2 = x1 + BQd[i].Item1 * j;
                    int y2 = y1 + BQd[i].Item2 * j;
                    if (x2 < 0 | x2 >= 6 | y2 < 0 | y2 >= 6) break;
                    if (((temp.wBishop | temp.wQueen) & (1ul << (y2 * 6 + x2))) > 0) bcheck = true;
                    if (((temp.wall | temp.ball | temp.wKing | temp.bKing) & (1ul << (y2 * 6 + x2))) > 0) break;
                }
            }
            for (int i = -1; i <= 1; i++)
            {
                for (int j = -1; j <= 1; j++)
                {
                    int x2 = x1 + i;
                    int y2 = y1 + j;
                    if (x2 < 0 | x2 >= 6 | y2 < 0 | y2 >= 6) continue;
                    if ((temp.wKing & (1ul << (y2 * 6 + x2))) > 0) bcheck = true;
                }
            }
            if (y1 > 0 && x1 > 1 && (temp.wPawn & (1ul << (bkingpos - 7))) > 0) bcheck = true;
            if (y1 > 0 && x1 < 5 && (temp.wPawn & (1ul << (bkingpos - 5))) > 0) bcheck = true;

            bool wcheck = false;
            int wkingpos = BitOperations.Log2(temp.wKing);
            x1 = wkingpos % 6;
            y1 = wkingpos / 6;
            for (int i = 0; i < 8; i++)
            {
                int x2 = x1 + Nd[i].Item1;
                int y2 = y1 + Nd[i].Item2;
                if (0 <= x2 && x2 < 6 && 0 <= y2 && y2 < 6 && (temp.bKnight & (1ul << (y2 * 6 + x2))) > 0) wcheck = true;
            }
            for (int i = 0; i < 4; i++)
            {
                for (int j = 1; j < 6; j++)
                {
                    int x2 = x1 + RQd[i].Item1 * j;
                    int y2 = y1 + RQd[i].Item2 * j;
                    if (x2 < 0 | x2 >= 6 | y2 < 0 | y2 >= 6) break;
                    if (((temp.bRook | temp.bQueen) & (1ul << (y2 * 6 + x2))) > 0) wcheck = true;
                    if (((temp.wall | temp.ball | temp.wKing | temp.bKing) & (1ul << (y2 * 6 + x2))) > 0) break;
                }
            }
            for (int i = 0; i < 4; i++)
            {
                
                for (int j = 1; j < 6; j++)
                {
                    int x2 = x1 + BQd[i].Item1 * j;
                    int y2 = y1 + BQd[i].Item2 * j;
                    if (x2 < 0 | x2 >= 6 | y2 < 0 | y2 >= 6) break;
                    if (((temp.bBishop | temp.bQueen) & (1ul << (y2 * 6 + x2))) > 0) wcheck = true;
                    if (((temp.wall | temp.ball | temp.wKing | temp.bKing) & (1ul << (y2 * 6 + x2))) > 0) break;
                }
            }
            for (int i = -1; i <= 1; i++)
            {
                for (int j = -1; j <= 1; j++)
                {
                    int x2 = x1 + i;
                    int y2 = y1 + j;
                    if (x2 < 0 | x2 >= 6 | y2 < 0 | y2 >= 6) continue;
                    if ((temp.bKing & (1ul << (y2 * 6 + x2))) > 0) wcheck = true;
                }
            }
            if (y1 <5 && x1 > 1 && (temp.bPawn & (1ul << (wkingpos +5))) > 0) wcheck = true;
            if (y1 <5 && x1 < 5 && (temp.bPawn & (1ul << (wkingpos +7))) > 0) wcheck = true;
            temp.bcheck = bcheck;
            temp.wcheck = wcheck;
            //*
            if ((char.IsLower(piece)&bcheck)|(char.IsUpper(piece)&wcheck))
            {
                return null;
            }
            //*/
            Console.WriteLine(sw.Elapsed.ToString());
            return temp;
        }
        [MethodImpl(MethodImplOptions.AggressiveOptimization | MethodImplOptions.AggressiveInlining)]
        static private void Change(ref BitBoard board,char piece,ulong cappos)
        {
            switch (piece)
            {
                case 'P': board.wPawn ^= cappos; break;
                case 'p': board.bPawn ^= cappos; break;
                case 'B': board.wBishop ^= cappos; break;
                case 'b': board.bBishop ^= cappos; break;
                case 'N': board.wKnight ^= cappos; break;
                case 'n': board.bKnight ^= cappos; break;
                case 'R': board.wRook ^= cappos; break;
                case 'r': board.bRook ^= cappos; break;
                case 'Q': board.wQueen ^= cappos; break;
                case 'q': board.bQueen ^= cappos; break;
                default: throw new InvalidOperationException();
            }
            if (char.IsLower(piece))
            {
                board.ball ^= cappos;
            }
            else
            {
                board.wall ^= cappos;
            }
        }
        [MethodImpl(MethodImplOptions.AggressiveOptimization | MethodImplOptions.AggressiveInlining)]
        static char GetPiece(BitBoard board, ulong position)
        {
            char box = ' ';
            if ((board.wPawn & position) > 0) box = 'P';
            if ((board.bPawn & position) > 0) box = 'p';
            if ((board.wBishop & position) > 0) box = 'B';
            if ((board.bBishop & position) > 0) box = 'b';
            if ((board.wKnight & position) > 0) box = 'N';
            if ((board.bKnight & position) > 0) box = 'n';
            if ((board.wRook & position) > 0) box = 'R';
            if ((board.bRook & position) > 0) box = 'r';
            if ((board.wQueen & position) > 0) box = 'Q';
            if ((board.bQueen & position) > 0) box = 'q';
            if ((board.wKing & position) > 0) box = 'K';
            if ((board.bKing & position) > 0) box = 'k';
            return box;
        }
        [MethodImpl(MethodImplOptions.AggressiveOptimization | MethodImplOptions.AggressiveInlining)]
        static BitBoard DeepCopy(BitBoard board) => new BitBoard(!board.wturn,board.wBishop,board.bBishop,board.wKnight,board.bKnight,board.wRook,board.bRook,board.wQueen,board.bQueen,board.wKing,board.bKing,board.wPawn,board.bPawn);
    }
}
