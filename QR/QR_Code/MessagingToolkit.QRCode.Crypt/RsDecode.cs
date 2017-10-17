using System;

namespace MessagingToolkit.QRCode.Crypt
{
    public class RsDecode
    {
        public static readonly int RS_PERM_ERROR = -1;
        public static readonly int RS_CORRECT_ERROR = -2;
        private static readonly Galois galois = Galois.GetInstance();
        private int npar;

        public RsDecode(int npar)
        {
            this.npar = npar;
        }

        public int CalcSigmaMBM(int[] sigma, int[] omega, int[] syn)
        {
            int[] numArray1 = new int[this.npar];
            int[] a1 = new int[this.npar];
            numArray1[1] = 1;
            a1[0] = 1;
            int val2 = 1;
            int num1 = 0;
            int num2 = -1;
            for (int index1 = 0; index1 < this.npar; ++index1)
            {
                int a2 = syn[index1];
                for (int index2 = 1; index2 <= num1; ++index2)
                    a2 ^= RsDecode.galois.Mul(a1[index2], syn[index1 - index2]);
                if (a2 != 0)
                {
                    int log = RsDecode.galois.ToLog(a2);
                    int[] numArray2 = new int[this.npar];
                    for (int index2 = 0; index2 <= index1; ++index2)
                        numArray2[index2] = a1[index2] ^ RsDecode.galois.MulExp(numArray1[index2], log);
                    int num3 = index1 - num2;
                    if (num3 > num1)
                    {
                        num2 = index1 - num1;
                        num1 = num3;
                        if (num1 > this.npar / 2)
                            return -1;
                        for (int index2 = 0; index2 <= val2; ++index2)
                            numArray1[index2] = RsDecode.galois.DivExp(a1[index2], log);
                        val2 = num1;
                    }
                    a1 = numArray2;
                }
                Array.Copy((Array)numArray1, 0, (Array)numArray1, 1, Math.Min(numArray1.Length - 1, val2));
                numArray1[0] = 0;
                ++val2;
            }
            RsDecode.galois.MulPoly(omega, a1, syn);
            Array.Copy((Array)a1, 0, (Array)sigma, 0, Math.Min(a1.Length, sigma.Length));
            return num1;
        }

        private int ChienSearch(int[] pos, int n, int jisu, int[] sigma)
        {
            int a1 = sigma[1];
            if (jisu == 1)
            {
                if (RsDecode.galois.ToLog(a1) >= n)
                    return RsDecode.RS_CORRECT_ERROR;
                pos[0] = a1;
                return 0;
            }
            int num1 = jisu - 1;
            for (int a2 = 0; a2 < n; ++a2)
            {
                int num2 = (int)byte.MaxValue - a2;
                int num3 = 1;
                for (int index = 1; index <= jisu; ++index)
                    num3 ^= RsDecode.galois.MulExp(sigma[index], num2 * index % (int)byte.MaxValue);
                if (num3 == 0)
                {
                    int exp = RsDecode.galois.ToExp(a2);
                    a1 ^= exp;
                    pos[num1--] = exp;
                    if (num1 == 0)
                    {
                        if (RsDecode.galois.ToLog(a1) >= n)
                            return RsDecode.RS_CORRECT_ERROR;
                        pos[0] = a1;
                        return 0;
                    }
                }
            }
            return RsDecode.RS_CORRECT_ERROR;
        }

        private void DoForney(int[] data, int length, int jisu, int[] pos, int[] sigma, int[] omega)
        {
            for (int index1 = 0; index1 < jisu; ++index1)
            {
                int po = pos[index1];
                int num1 = (int)byte.MaxValue - RsDecode.galois.ToLog(po);
                int a = omega[0];
                for (int index2 = 1; index2 < jisu; ++index2)
                    a ^= RsDecode.galois.MulExp(omega[index2], num1 * index2 % (int)byte.MaxValue);
                int b = sigma[1];
                int num2 = 2;
                while (num2 < jisu)
                {
                    b ^= RsDecode.galois.MulExp(sigma[num2 + 1], num1 * num2 % (int)byte.MaxValue);
                    num2 += 2;
                }
                data[RsDecode.galois.ToPos(length, po)] ^= RsDecode.galois.Mul(po, RsDecode.galois.Div(a, b));
            }
        }

        public int Decode(int[] data, int length, bool noCorrect)
        {
            if (length < this.npar || length > (int)byte.MaxValue)
                return RsDecode.RS_PERM_ERROR;
            int[] syn = new int[this.npar];
            if (RsDecode.galois.CalcSyndrome(data, length, syn))
                return 0;
            int[] sigma = new int[this.npar / 2 + 2];
            int[] omega = new int[this.npar / 2 + 1];
            int jisu = this.CalcSigmaMBM(sigma, omega, syn);
            if (jisu <= 0)
                return RsDecode.RS_CORRECT_ERROR;
            int[] pos = new int[jisu];
            int num = this.ChienSearch(pos, length, jisu, sigma);
            if (num < 0)
                return num;
            if (!noCorrect)
                this.DoForney(data, length, jisu, pos, sigma, omega);
            return jisu;
        }

        public int Decode(int[] data, int length)
        {
            return this.Decode(data, length, false);
        }

        public int Decode(int[] data)
        {
            return this.Decode(data, data.Length, false);
        }
    }
}
