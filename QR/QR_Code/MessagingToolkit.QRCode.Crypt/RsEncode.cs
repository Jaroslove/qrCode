using System;

namespace MessagingToolkit.QRCode.Crypt
{
    public class RsEncode
    {
        public static readonly int RS_PERM_ERROR = -1;
        private static readonly Galois galois = Galois.GetInstance();
        private int npar;
        private int[] encodeGx;

        public RsEncode(int npar)
        {
            this.npar = npar;
            this.MakeEncodeGx();
        }

        private void MakeEncodeGx()
        {
            this.encodeGx = new int[this.npar];
            this.encodeGx[this.npar - 1] = 1;
            for (int a = 0; a < this.npar; ++a)
            {
                int exp = RsEncode.galois.ToExp(a);
                for (int index = 0; index < this.npar - 1; ++index)
                    this.encodeGx[index] = RsEncode.galois.Mul(this.encodeGx[index], exp) ^ this.encodeGx[index + 1];
                this.encodeGx[this.npar - 1] = RsEncode.galois.Mul(this.encodeGx[this.npar - 1], exp);
            }
        }

        public int Encode(int[] data, int length, int[] parity, int parityStartPos)
        {
            if (length < 0 || length + this.npar > (int)byte.MaxValue)
                return RsEncode.RS_PERM_ERROR;
            int[] numArray = new int[this.npar];
            for (int index1 = 0; index1 < length; ++index1)
            {
                int num = data[index1];
                int a = numArray[0] ^ num;
                for (int index2 = 0; index2 < this.npar - 1; ++index2)
                    numArray[index2] = numArray[index2 + 1] ^ RsEncode.galois.Mul(a, this.encodeGx[index2]);
                numArray[this.npar - 1] = RsEncode.galois.Mul(a, this.encodeGx[this.npar - 1]);
            }
            if (parity != null)
                Array.Copy((Array)numArray, 0, (Array)parity, parityStartPos, this.npar);
            return 0;
        }

        public int Encode(int[] data, int length, int[] parity)
        {
            return this.Encode(data, length, parity, 0);
        }

        public int Encode(int[] data, int[] parity)
        {
            return this.Encode(data, data.Length, parity, 0);
        }
    }
}
