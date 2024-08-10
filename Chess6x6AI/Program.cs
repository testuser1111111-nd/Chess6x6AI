﻿using SkiaSharp;
using System;
using System.Diagnostics;
using System.Net.NetworkInformation;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Text;
using static System.Net.Mime.MediaTypeNames;

namespace Chess6x6AI
{
    internal class Program
    {
        [MethodImpl(MethodImplOptions.AggressiveOptimization)]
        static void Main(string[] args)
        {
            BitBoard test = new BitBoard(true,0, 0, 18, 18*(1uL<<30), 33, 33*(1uL<<30), 4, 4*(1uL<<30), 8, 8*(1uL<<30));
            Console.WriteLine(test);
            Stopwatch sw = new Stopwatch();
            while (true)
            {
                try
                {
                    string move = Console.ReadLine();
                    sw.Start();
                    if (move.Length != 4 & move.Length != 5) throw new InvalidDataException("input len must be 4 or 5. ex:c2c4");
                    if (move[0] < 'a' | 'f' < move[0] | move[1] < '1' | '6' < move[1] | move[2] < 'a' | 'f' < move[2] | move[3] < '1' | '6' < move[3]) throw new InvalidOperationException("position out of range");
                    int cur = (move[0] - 'a') + (move[1] - '1') * 6;
                    int fin = (move[2] - 'a') + (move[3] - '1') * 6;
                    char pp = move.Length > 4? move[4]:'p';
                    BitBoard temp = BoardOps.NextBoard(test, cur, fin, pp);//動きが可能か判断し、可能なら盤面を返す;
                    if (temp is null)
                    {
                        throw new InvalidOperationException();
                    }
                    else
                    {
                        test = temp;
                    }
                }
                catch(Exception e)
                {
                    Console.WriteLine(e.Message);
                    continue;
                }
                Console.WriteLine(test);
                var next = BoardOps.GenBoard(test);
                if (next.Count == 0)
                {
                    if (test.wturn)
                    {
                        Console.WriteLine(test.wcheck ? "white king got checkmated." : "stalemate");
                    }
                    else
                    {
                        Console.WriteLine(test.bcheck ? "black king got checkmated." : "stalemate");
                    }
                    return;
                }
                else
                {
                    Console.WriteLine(test.wcheck ? "white king got checked." : "");
                    Console.WriteLine(test.bcheck ? "black king got checked." : "");
                }
                BitBoard minboard = null;
                int score = int.MaxValue;
                int ncur= 0, nfin = 0;
                char np = ' ';
                Parallel.ForEach(next, a => {

                    var eval = minmax(a.Item1, 4);//なんかおかしい
                    if (eval < score)
                    {
                        score = eval;
                        minboard = a.Item1;
                        ncur = a.Item2;
                        nfin = a.Item3;
                        np = a.Item4;
                    }
                });
                test = minboard;
                Console.WriteLine(minboard);
                Console.WriteLine(ncur+" "+nfin+" "+np+" "+score);
                var next2 = BoardOps.GenBoard(test);
                if (next2.Count == 0)
                {
                    if (test.wturn)
                    {
                        Console.WriteLine(test.wcheck ? "white king got checkmated." : "stalemate");
                    }
                    else
                    {
                        Console.WriteLine(test.bcheck ? "black king got checkmated." : "stalemate");
                    }
                    return;
                }
                else
                {
                    Console.WriteLine(test.wcheck ? "white king got checked." : "");
                    Console.WriteLine(test.bcheck ? "black king got checked." : "");
                }
                Console.WriteLine(sw.Elapsed);
                sw.Stop();
                sw.Reset();
            }
        }
        
        static int minmax(BitBoard board,int depth)
        {
            var next = BoardOps.GenBoard(board);
            if(next.Count == 0)
            {
                if(board.wcheck&board.wturn)return -100 ;
                if (board.bcheck& !board.wturn) return 100;
                return 0;
            }
            if (depth == 0)
            {
                return
                    (BitOperations.PopCount(board.wPawn) +
                    BitOperations.PopCount(board.wBishop) * 3 +
                    BitOperations.PopCount(board.wKnight) * 3 +
                    BitOperations.PopCount(board.wRook) * 5 +
                    BitOperations.PopCount(board.wQueen) * 9 +
                    BitOperations.PopCount(board.bPawn) * -1 +
                    BitOperations.PopCount(board.bBishop) * -3 +
                    BitOperations.PopCount(board.bKnight) * -3 +
                    BitOperations.PopCount(board.bRook) * -5 +
                    BitOperations.PopCount(board.bQueen) * -9)
                    * (board.wturn ? 1 : -1);
            }
            if (board.wturn)
            {
                int max = int.MinValue;
                foreach(var a in next)
                {
                    int score = minmax(a.Item1, depth - 1);
                    if (score > max) max = score;
                }
                return max;
            }
            else
            {
                int min = int.MaxValue;
                foreach(var a in next)
                {
                    int score = minmax(a.Item1, depth - 1);
                    if (score < min) min = score;
                }
                return min;
            }

        }
    }
}
