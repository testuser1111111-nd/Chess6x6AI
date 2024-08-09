using System;
using System.Net.NetworkInformation;
using System.Runtime.CompilerServices;
using System.Text;

namespace Chess6x6AI
{
    internal class Program
    {
        [MethodImpl(MethodImplOptions.AggressiveOptimization)]
        static void Main(string[] args)
        {
            BitBoard test = new BitBoard(true,0, 0, 18, 18*(1uL<<30), 33, 33*(1uL<<30), 4, 4*(1uL<<30), 8, 8*(1uL<<30));
            Console.WriteLine(test);
            while (true)
            {
                try
                {
                    string move = Console.ReadLine();
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
                }
                Console.WriteLine(test);
                Console.WriteLine(test.wcheck ? "white king got checked." : "");
                Console.WriteLine(test.bcheck ? "black king got checked." : "");
            }
        }
    }
}
