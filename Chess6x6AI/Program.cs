using Chess6x6AI;
using SkiaSharp;
using System;
using System.Diagnostics;
using System.Net.NetworkInformation;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Text;
using static System.Net.Mime.MediaTypeNames;
using static Tensorboard.CostGraphDef.Types;
using static TorchSharp.torch;

namespace Chess6x6AI
{
    internal class Program
    {
        static int minmaxcalled = 0;
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
                if (test.turn >= 75)
                {
                    Console.WriteLine("time over"); return;
                }
                /*
                BitBoard minboard = null;
                int score = int.MaxValue;
                int ncur= 0, nfin = 0;
                char np = ' ';
                foreach(var a in next)
                {
                    var eval = minmax(a.Item1, 3);//実質5手見てる
                    if (eval < score)
                    {
                        score = eval;
                        minboard = a.Item1;
                        ncur = a.Item2;
                        nfin = a.Item3;
                        np = a.Item4;
                    }
                }
                Console.WriteLine(minmaxcalled);
                minmaxcalled = 0;
                test = minboard;
                
                Console.WriteLine(minboard);
                Console.WriteLine(ncur+" "+nfin+" "+np+" "+score);
                */
                test = MCTS(test, 100000);
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
                if (test.turn >= 75)
                {
                    Console.WriteLine("time over");
                }
                Console.WriteLine(sw.Elapsed);
                sw.Stop();
                sw.Reset();
            }
        }
        
        static int minmax(BitBoard board,int depth)
        {
            if (board.turn >= 75)
            {
                return 0;
            }
            var next = BoardOps.GenBoard(board);
            minmaxcalled += next.Count;
            if(next.Count == 0)
            {
                if(board.wcheck&board.wturn)return -100-depth;
                if (board.bcheck& !board.wturn) return 100+depth;
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
                    BitOperations.PopCount(board.bQueen) * -9);
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


        static Random random = new Random();

        static BitBoard MCTS(BitBoard root, int simulations)
        {
            Node rootNode = new Node(root, null);
            // シミュレーションを繰り返す
            for (int i = 0; i<simulations; i++)
            {
                // 1. Selection: UCB1で子ノードを選択
                Node node = rootNode;
                while (node.Children.Count > 0)
                {
                        node = node.Children.OrderByDescending(n => n.UCB1()).First();
                }
                // 2. Expansion: 新しい子ノードを作成
                var possibleMoves = BoardOps.GenBoard(node.Board);
                if (possibleMoves.Count > 0)
                {
                    var randomMove = possibleMoves[random.Next(possibleMoves.Count)];
                    BitBoard newBoard = randomMove.Item1;
                    Node childNode = new Node(newBoard, node, randomMove.Item2, randomMove.Item3, randomMove.Item4);
                    node.Children.Add(childNode);
                    node = childNode;
                }
                // 3. Simulation: ランダムプレイアウト
                int result = Simulate(node.Board);
                // 4. Backpropagation: 結果を親ノードに反映
                while (node != null)
                {
                    node.Visits++;
                    node.Wins += result;
                    node = node.Parent;
                }
            }
            // 最も訪問された子ノードを次の手として選択
            return rootNode.Children.OrderByDescending(n => n.Visits).First().Board;
        }
        // ランダムプレイアウトを行う関数
        static int Simulate(BitBoard board)
        {
            BitBoard tempBoard = board;
            while (true)
            {
                var possibleMoves = BoardOps.GenBoard(tempBoard);
                if (possibleMoves.Count == 0)
                {
                    if (tempBoard.wcheck & tempBoard.wturn) return -1; // ブラックの勝ち
                    if (tempBoard.bcheck & !tempBoard.wturn) return 1;  // ホワイトの勝ち
                    return 0; // 引き分け
                }
                var randomMove = possibleMoves[random.Next(possibleMoves.Count)];
                tempBoard = randomMove.Item1;
                if (tempBoard.turn >= 75) return 0; // 手数制限による引き分け
            }
        }
    }
    // ノードの情報を格納するためのクラス
    class Node
    {
        public BitBoard Board;
        public Node Parent;
        public List<Node> Children = new List<Node>();
        public int Wins = 0;
        public int Visits = 0;
        public int MoveFrom, MoveTo;
        public char Promotion;

        public Node(BitBoard board, Node parent, int moveFrom = 0, int moveTo = 0, char promotion = ' ')
        {
            Board = board;
            Parent = parent;
            MoveFrom = moveFrom;
            MoveTo = moveTo;
            Promotion = promotion;
        }

        // UCB1を計算する関数
        public double UCB1(double c = 1.41)
        {
            if (Visits == 0) return double.MaxValue;
            return (double)Wins / Visits + c * Math.Sqrt(Math.Log(Parent.Visits) / Visits);
        }
    }
}
