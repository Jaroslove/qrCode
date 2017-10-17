namespace MessagingToolkit.QRCode.Crypt
{
    public sealed class BCH_15_5
    {
        private static readonly int GX = 311;
        private static readonly BCH_15_5 instance = new BCH_15_5();
        private int[] trueCodes = new int[32];

        private BCH_15_5()
        {
            this.MakeTrueCodes();
        }

        public static BCH_15_5 GetInstance()
        {
            return BCH_15_5.instance;
        }

        private void MakeTrueCodes()
        {
            for (int data = 0; data < this.trueCodes.Length; ++data)
                this.trueCodes[data] = this.SlowEncode(data);
        }

        private int SlowEncode(int data)
        {
            int num = 0;
            data <<= 5;
            for (int index = 0; index < 5; ++index)
            {
                num <<= 1;
                data <<= 1;
                if (((num ^ data) & 1024) != 0)
                    num ^= BCH_15_5.GX;
            }
            return data & 31744 | num & 1023;
        }

        public int Encode(int data)
        {
            return this.trueCodes[data & 31];
        }

        private static int CalcDistance(int c1, int c2)
        {
            int num1 = 0;
            int num2 = c1 ^ c2;
            while (num2 != 0)
            {
                if ((num2 & 1) != 0)
                    ++num1;
                num2 >>= 1;
            }
            return num1;
        }

        public int Decode(int data)
        {
            data &= (int)short.MaxValue;
            for (int index = 0; index < this.trueCodes.Length; ++index)
            {
                int trueCode = this.trueCodes[index];
                if (BCH_15_5.CalcDistance(data, trueCode) <= 3)
                    return trueCode;
            }
            return -1;
        }
    }
}
