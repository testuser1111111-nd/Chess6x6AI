using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TorchSharp;

namespace Chess6x6AI
{
    internal class FileControl
    {
        public static void Output(((bool[], bool[]), int)[] values,string filename)
        {
            StringBuilder sb = new StringBuilder();
            foreach(var a in values)
            {
                byte[] data = new byte[36 * 36 * 2 + 1];
                bool[] i1 = a.Item1.Item1;
                bool[] i2 = a.Item1.Item2;
                for(int i = 0; i < 1296; i++)
                {
                    data[i] =(byte)(
                        (i1[i * 8 + 7] ? 1 << 7 : 0) |
                        (i1[i * 8 + 6] ? 1 << 6 : 0) |
                        (i1[i * 8 + 5] ? 1 << 5 : 0) |
                        (i1[i * 8 + 4] ? 1 << 4 : 0) |
                        (i1[i * 8 + 3] ? 1 << 3 : 0) |
                        (i1[i * 8 + 2] ? 1 << 2 : 0) |
                        (i1[i * 8 + 1] ? 1 << 1 : 0) |
                        (i1[i * 8 + 0] ? 1 : 0)); 
                    data[i+1296] = (byte)(
                        (i2[i * 8 + 7] ? 1 << 7 : 0) |
                        (i2[i * 8 + 6] ? 1 << 6 : 0) |
                        (i2[i * 8 + 5] ? 1 << 5 : 0) |
                        (i2[i * 8 + 4] ? 1 << 4 : 0) |
                        (i2[i * 8 + 3] ? 1 << 3 : 0) |
                        (i2[i * 8 + 2] ? 1 << 2 : 0) |
                        (i2[i * 8 + 1] ? 1 << 1 : 0) |
                        (i2[i * 8 + 0] ? 1 : 0));
                }
                data[2592] = (byte)(sbyte)a.Item2;//-127~127しかこないのでこれで
                sb.AppendLine(Convert.ToBase64String(data));
            }
            File.WriteAllText(filename,sb.ToString());
        }
        public static void Output2(HashSet<BitBoard> pos, string filename)
        {
            var posarr = pos.ToArray();
            string[] strarr = new string[posarr.Length];
            int evaled = 0;
            Parallel.For(0,Program.limit, j =>{
                evaled++;
                if (evaled % 100 == 0) Console.WriteLine(evaled);
                byte[] data = new byte[36 * 36 * 2 + 1];
                bool[] i1 = posarr[j].halfKP().Item1;
                bool[] i2 = posarr[j].halfKP().Item2;
                for (int i = 0; i < 1296; i++)
                {
                    data[i] = (byte)(
                        (i1[i * 8 + 7] ? 1 << 7 : 0) |
                        (i1[i * 8 + 6] ? 1 << 6 : 0) |
                        (i1[i * 8 + 5] ? 1 << 5 : 0) |
                        (i1[i * 8 + 4] ? 1 << 4 : 0) |
                        (i1[i * 8 + 3] ? 1 << 3 : 0) |
                        (i1[i * 8 + 2] ? 1 << 2 : 0) |
                        (i1[i * 8 + 1] ? 1 << 1 : 0) |
                        (i1[i * 8 + 0] ? 1 : 0));
                    data[i + 1296] = (byte)(
                        (i2[i * 8 + 7] ? 1 << 7 : 0) |
                        (i2[i * 8 + 6] ? 1 << 6 : 0) |
                        (i2[i * 8 + 5] ? 1 << 5 : 0) |
                        (i2[i * 8 + 4] ? 1 << 4 : 0) |
                        (i2[i * 8 + 3] ? 1 << 3 : 0) |
                        (i2[i * 8 + 2] ? 1 << 2 : 0) |
                        (i2[i * 8 + 1] ? 1 << 1 : 0) |
                        (i2[i * 8 + 0] ? 1 : 0));
                }
                data[2592] = (byte)(sbyte)BoardOps.EvalBoard(posarr[j], 4);//-104~104しかこないのでこれで
                
                strarr[j] = (Convert.ToBase64String(data));
            });
            for(int i = 0;i< (strarr.Length+99999)/100000; i++)//testdata生成時は少し書き替える必要がある
            {
                StringBuilder sb = new StringBuilder();
                for (int j = 0; (j < 100000) & i*100000 + j < strarr.Length; j++)
                {
                    sb.AppendLine(strarr[j]);
                }
                File.WriteAllText(filename+i.ToString(), sb.ToString());
            }
        }
    }
}
