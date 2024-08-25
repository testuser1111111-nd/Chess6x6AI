using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Chess6x6AI
{
    class NNUEagent
    {
        float[][] weight1a;
        float[] bias1a;
        float[][] weight1b;
        float[] bias1b;
        float[][] weight2;
        float[] bias2;
        float[][] weight3;
        float[] bias3;
        float[][] weight4;
        float[] bias4;
        public NNUEagent() 
        {
            var reader = new StreamReader("model_weights2.csv");
            string line = reader.ReadLine();
            var values = line.Split(',');
            int dimension1 = 256;
            int dimension2 = 10368;
            weight1a = new float[dimension1][];
            for (int i = 0; i < dimension1; i++)
            {
                weight1a[i] = new float[dimension2];
                for (int j = 0; j < dimension2; j++)
                {
                    weight1a[i][j] = float.Parse(values[i*dimension2+j]);
                }
            }
            line = reader.ReadLine(); 
            values = line.Split(',');
            bias1a = new float[dimension1];
            for(int i = 0;i < dimension1; i++)
            {
                bias1a[i] = float.Parse(values[i]);
            }
            line = reader.ReadLine(); 
            values = line.Split(',');
            weight1b = new float[dimension1][];
            for (int i = 0; i < dimension1; i++)
            {
                weight1b[i] = new float[dimension2];
                for (int j = 0; j < dimension2; j++)
                {
                    weight1b[i][j] = float.Parse(values[i * dimension2 + j]);
                }
            }
            line = reader.ReadLine();
            values = line.Split(',');
            bias1b = new float[dimension1];
            for (int i = 0; i < dimension1; i++)
            {
                bias1b[i] = float.Parse(values[i]);
            }
            line = reader.ReadLine();
            values = line.Split(',');
            dimension1 = 32;
            dimension2 = 512;
            weight2 = new float[dimension1][];
            for (int i = 0; i < dimension1; i++)
            {
                weight2[i] = new float[dimension2];
                for (int j = 0; j < dimension2; j++)
                {
                    weight2[i][j] = float.Parse(values[i * dimension2 + j]);
                }
            }
            line = reader.ReadLine();
            values = line.Split(',');
            bias2 = new float[dimension1];
            for (int i = 0; i < dimension1; i++)
            {
                bias2[i] = float.Parse(values[i]);
            }
            line = reader.ReadLine();
            values = line.Split(',');
            dimension1 = 32;
            dimension2 = 32;
            weight3 = new float[dimension1][];
            for (int i = 0; i < dimension1; i++)
            {
                weight3[i] = new float[dimension2];
                for (int j = 0; j < dimension2; j++)
                {
                    weight3[i][j] = float.Parse(values[i * dimension2 + j]);
                }
            }
            line = reader.ReadLine();
            values = line.Split(',');
            bias3 = new float[dimension1];
            for (int i = 0; i < dimension1; i++)
            {
                bias3[i] = float.Parse(values[i]);
            }
            line = reader.ReadLine();
            values = line.Split(',');
            dimension1 = 1;
            dimension2 = 32;
            weight4 = new float[dimension1][];
            for (int i = 0; i < dimension1; i++)
            {
                weight4[i] = new float[dimension2];
                for (int j = 0; j < dimension2; j++)
                {
                    weight4[i][j] = float.Parse(values[i * dimension2 + j]);
                }
            }
            line = reader.ReadLine();
            values = line.Split(',');
            bias4 = new float[dimension1];
            for (int i = 0; i < dimension1; i++)
            {
                bias4[i] = float.Parse(values[i]);
            }
        }
        public float straighteval(BitBoard board) {
            var hkp = board.halfKP();
            var kp1 = hkp.Item1;
            var kp2 = hkp.Item2;
            float[] l1 = new float[512];
            for(int i = 0; i < 256; i++)
            {
                l1[i] = bias1a[i];
                l1[i+256] = bias1b[i];
            }
            for(int j = 0; j < 10368; j++)
            {
                if (kp1[j])
                {
                    for(int i = 0; i < 256; i++)
                    {
                        l1[i] += weight1a[i][j];
                    }
                }
                if (kp2[j])
                {
                    for(int i = 0; i < 256; i++)
                    {
                        l1[i+256] += weight1b[i][j];
                    }
                }
            }
            for(int i = 0; i < 256; i++)
            {
                if (l1[i] < 0) l1[i] = 0;
                if (l1[i + 256] < 0) l1[i + 256] = 0;
            }
            float[] l2 = new float[32];
            for(int i = 0;i < 32; i++)
            {
                l2[i] = bias2[i];
                for(int j = 0; j < 512; j++)
                {
                    l2[i] += weight2[i][j] * l1[j];
                }
                if(l2[i] < 0) { l2[i] = 0; }
            }
            float[] l3 = new float[32];
            for (int i = 0; i < 32; i++)
            {
                l3[i] = bias3[i];
                for (int j = 0; j < 32; j++)
                {
                    l3[i] += weight3[i][j] * l2[j];
                }
                if (l3[i] < 0) { l3[i] = 0; }
            }
            float l4 = bias4[0];
            for(int i = 0; i < 32; i++)
            {
                l4 += weight4[0][i] * l3[i];
            }
            return l4;
        }
        public float EvalBoard(BitBoard board,int depth)
        {
            return NNegamax(board, -10000, 10000, depth);
            /*
            float[] l1a = new float[256];
            float[] l1b = new float[256];//relu前の一層目の値
            var halfkp = board.halfKP();
            for(int i = 0; i < 256; i++)
            {
                l1a[i] = 0;
                l1b[i] = 0;
                for (int j = 0; j < 10368;j++)
                {
                    if (halfkp.Item1[j]) l1a[i] += weight1a[i][j];
                    if (halfkp.Item2[j]) l1b[i] += weight1b[i][j];
                }
            }
            */
            //return NNUEgamax(board,float.MinValue,float.MaxValue,depth,l1a,l1b);
        }
        public float NNUEgamax(BitBoard board,float alpha,float beta,int depth, float[] l1a, float[] l1b)
        {
            throw new NotImplementedException();
        }

        public float NNegamax(BitBoard board,float alpha,float beta,int depth)
        {
            int color = board.wturn ? 1 : -1;
            var next = BoardOps.GenBoard(board);
            if (next.Count == 0)
            {
                if (board.wcheck & board.wturn) return -100 - depth;
                if (board.bcheck & !board.wturn) return -100 - depth;
                return 0;
            }
            if (depth == 0)
            {
                return straighteval(board);
            }

            float maxEval = -10000;
            foreach (var a in next)
            {
                float eval = -NNegamax(a.Item1, -beta, -alpha, depth - 1);
                maxEval = Math.Max(maxEval, eval);
                alpha = Math.Max(alpha, eval);
                if (alpha >= beta) break;
            }
            return maxEval;
        }
    }
}

