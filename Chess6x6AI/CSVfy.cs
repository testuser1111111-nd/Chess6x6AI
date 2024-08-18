using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chess6x6AI
{
    internal class FileControl
    {
        public static void Output(((bool[], bool[]), int)[] values)
        {
            StringBuilder sb = new StringBuilder();
            foreach(var a in values)
            {
                byte[] data = new byte[36 * 36 * 2 + 1];
                bool[] i1 = a.Item1.Item1;
                bool[] i2 = a.Item1.Item2;
                for(int i = 0; i < 2196; i++)
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
                    data[i+2196] = (byte)(
                        (i2[i * 8 + 7] ? 1 << 7 : 0) |
                        (i2[i * 8 + 6] ? 1 << 6 : 0) |
                        (i2[i * 8 + 5] ? 1 << 5 : 0) |
                        (i2[i * 8 + 4] ? 1 << 4 : 0) |
                        (i2[i * 8 + 3] ? 1 << 3 : 0) |
                        (i2[i * 8 + 2] ? 1 << 2 : 0) |
                        (i2[i * 8 + 1] ? 1 << 1 : 0) |
                        (i2[i * 8 + 0] ? 1 : 0));
                }
                data[4392] = (byte)(sbyte)a.Item2;//-127~127しかこないのでこれで
                sb.AppendLine(Convert.ToBase64String(data));
            }
            File.WriteAllText("traindata",sb.ToString());
        }
    }
}
