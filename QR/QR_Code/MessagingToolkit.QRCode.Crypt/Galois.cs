using System;

namespace MessagingToolkit.QRCode.Crypt
{
    public sealed class Galois
    {
        public static readonly int POLYNOMIAL = 29;
        private static readonly Galois instance = new Galois();
        private int[] expTbl = new int[510];
        private int[] logTbl = new int[256];

        private Galois()
        {
            this.InitGaloisTable();
        }

        public static Galois GetInstance()
        {
            return Galois.instance;
        }

        private void InitGaloisTable()
        {
            int index1 = 1;
            for (int index2 = 0; index2 < (int)byte.MaxValue; ++index2)
            {
                this.expTbl[index2] = this.expTbl[(int)byte.MaxValue + index2] = index1;
                this.logTbl[index1] = index2;
                index1 <<= 1;
                if ((index1 & 256) != 0)
                    index1 = (index1 ^ Galois.POLYNOMIAL) & (int)byte.MaxValue;
            }
        }

        public int ToExp(int a)
        {
            return this.expTbl[a];
        }

        public int ToLog(int a)
        {
            return this.logTbl[a];
        }

        public int ToPos(int length, int a)
        {
            return length - 1 - this.logTbl[a];
        }

        public int Mul(int a, int b)
        {
            return a == 0 || b == 0 ? 0 : this.expTbl[this.logTbl[a] + this.logTbl[b]];
        }

        public int MulExp(int a, int b)
        {
            return a == 0 ? 0 : this.expTbl[this.logTbl[a] + b];
        }

        public int Div(int a, int b)
        {
            return a == 0 ? 0 : this.expTbl[this.logTbl[a] - this.logTbl[b] + (int)byte.MaxValue];
        }

        public int DivExp(int a, int b)
        {
            return a == 0 ? 0 : this.expTbl[this.logTbl[a] - b + (int)byte.MaxValue];
        }

        public int Inv(int a)
        {
            return this.expTbl[(int)byte.MaxValue - this.logTbl[a]];
        }

        public void MulPoly(int[] seki, int[] a, int[] b)
        {
            for (int index = 0; index < seki.Length; ++index)
                seki[index] = 0;
            for (int index1 = 0; index1 < a.Length; ++index1)
            {
                if (a[index1] != 0)
                {
                    int num1 = this.logTbl[a[index1]];
                    int num2 = Math.Min(b.Length, seki.Length - index1);
                    for (int index2 = 0; index2 < num2; ++index2)
                    {
                        if (b[index2] != 0)
                            seki[index1 + index2] ^= this.expTbl[num1 + this.logTbl[b[index2]]];
                    }
                }
            }
        }

        public bool CalcSyndrome(int[] data, int length, int[] syn)
        {
            int num = 0;
            for (int index1 = 0; index1 < syn.Length; ++index1)
            {
                int index2 = 0;
                for (int index3 = 0; index3 < length; ++index3)
                    index2 = data[index3] ^ (index2 == 0 ? 0 : this.expTbl[this.logTbl[index2] + index1]);
                syn[index1] = index2;
                num |= index2;
            }
            return num == 0;
        }
    }
}
