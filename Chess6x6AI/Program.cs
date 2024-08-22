﻿using Chess6x6AI;
using System;
using System.Diagnostics;
using System.Numerics;
using System.Runtime.CompilerServices;
using static Tensorboard.CostGraphDef.Types;
using static TorchSharp.torch;
namespace Chess6x6AI
{
    internal class Program
    {
        [MethodImpl(MethodImplOptions.AggressiveOptimization)]
        static void Main3(string[] args)
        {
            BitBoard test = new BitBoard(true, 18, 18 * (1uL << 30), 33, 33 * (1uL << 30), 4, 4 * (1uL << 30), 8, 8 * (1uL << 30));
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
                    char pp = move.Length > 4 ? move[4] : 'p';
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
                catch (Exception e)
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
                if (test.turn >= 100)
                {
                    Console.WriteLine("time over"); return;
                }
                //*
                BitBoard minboard = null;
                int score = int.MinValue;
                int ncur= 0, nfin = 0;
                char np = ' ';
                int rems = BitOperations.PopCount(test.wall | test.ball);
                foreach(var a in next)
                {
                    var eval = -BoardOps.EvalBoard(a.Item1, 4);//実質5手見てる
                    if (eval > score)
                    {
                        score = eval;
                        minboard = a.Item1;
                        ncur = a.Item2;
                        nfin = a.Item3;
                        np = a.Item4;
                    }
                }
                test = minboard;
                Console.WriteLine(minboard);
                Console.WriteLine(ncur + " " + nfin + " " + np + " " + score);
                Console.WriteLine(test);
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
                if (test.turn >= 100)
                {
                    Console.WriteLine("time over");
                }
                Console.WriteLine(sw.Elapsed);
                sw.Stop();
                sw.Reset();
            }
        }
        public const int limit = 10000;//traindata 1000000, test 10000
        static void Main()
        {
            //Dictionary <BitBoard, ((bool[], bool[]),int)> dict = new Dictionary<BitBoard, ((bool[], bool[]), int)> ();
            HashSet<BitBoard> set = new HashSet<BitBoard> ();
            Random random = new Random (1);//seed train:0 test:1

            while (set.Count<=limit)//dict.Keys.Count <= limit)//300000に後でする　今はプログラムのテスト
            {
                BitBoard test = new BitBoard(true, 18, 18 * (1uL << 30), 33, 33 * (1uL << 30), 4, 4 * (1uL << 30), 8, 8 * (1uL << 30));
                while (true)
                {
                    //wturn
                    Console.WriteLine(test);
                    var nextmove = BoardOps.GenBoard (test);

                    //if(!dict.ContainsKey(test))dict.Add(test, (test.halfKP(), BoardOps.EvalBoard(test, 4)));
                    set.Add(test);
                    if (nextmove.Count == 0) break;
                    if (test.turn >= 100) break;
                    test = nextmove[random.Next(nextmove.Count)].Item1;
                    //if (dict.Keys.Count > limit) break;
                    if(set.Count > limit) break;
                    //bturn
                    Console.WriteLine(test);
                    nextmove = BoardOps.GenBoard(test);
                    //if (!dict.ContainsKey(test)) dict.Add(test, (test.halfKP(), BoardOps.EvalBoard(test, 4)));
                    set.Add (test);
                    if (nextmove.Count == 0) break;
                    if(test.turn >= 100) break;
                    test = nextmove[random.Next(nextmove.Count)].Item1;
                    if(set.Count>limit) break;
                    //if (dict.Keys.Count > limit) break;
                   Console.WriteLine (set.Count);
                    // Console.WriteLine(dict.Keys.Count);
                }
            }
            //FileControl.Output(dict.Values.ToArray(),"traindata1");
            FileControl.Output2(set, "testdata");
        }
    }
}
