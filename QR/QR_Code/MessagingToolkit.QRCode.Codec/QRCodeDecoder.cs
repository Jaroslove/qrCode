using MessagingToolkit.QRCode.Codec.Data;
using MessagingToolkit.QRCode.Codec.Reader;
using MessagingToolkit.QRCode.Crypt;
using MessagingToolkit.QRCode.ExceptionHandler;
using MessagingToolkit.QRCode.Geom;
using MessagingToolkit.QRCode.Helper;
using System;
using System.Collections;
using System.Text;

namespace MessagingToolkit.QRCode.Codec
{
    public class QRCodeDecoder
    {
        internal ArrayList lastResults = ArrayList.Synchronized(new ArrayList(10));
        internal QRCodeSymbol qrCodeSymbol;
        internal int numTryDecode;
        internal ArrayList results;
        internal static DebugCanvas canvas;
        internal QRCodeImageReader imageReader;
        internal int numLastCorrectionFailures;

        public static DebugCanvas Canvas
        {
            get
            {
                return QRCodeDecoder.canvas;
            }
            set
            {
                QRCodeDecoder.canvas = value;
            }
        }

        internal virtual Point[] AdjustPoints
        {
            get
            {
                ArrayList arrayList = ArrayList.Synchronized(new ArrayList(10));
                for (int index = 0; index < 4; ++index)
                    arrayList.Add((object)new Point(1, 1));
                int num1 = 0;
                int num2 = 0;
                for (int index1 = 0; index1 > -4; --index1)
                {
                    for (int index2 = 0; index2 > -4; --index2)
                    {
                        if (index2 != index1 && (index2 + index1) % 2 == 0)
                        {
                            arrayList.Add((object)new Point(index2 - num1, index1 - num2));
                            num1 = index2;
                            num2 = index1;
                        }
                    }
                }
                Point[] pointArray = new Point[arrayList.Count];
                for (int index = 0; index < pointArray.Length; ++index)
                    pointArray[index] = (Point)arrayList[index];
                return pointArray;
            }
        }

        public QRCodeDecoder()
        {
            this.numTryDecode = 0;
            this.results = ArrayList.Synchronized(new ArrayList(10));
            QRCodeDecoder.canvas = (DebugCanvas)new DebugCanvasAdapter();
        }

        public virtual sbyte[] DecodeBytes(QRCodeImage qrCodeImage)
        {
            Point[] adjustPoints = this.AdjustPoints;
            ArrayList arrayList = ArrayList.Synchronized(new ArrayList(10));
            this.numTryDecode = 0;
            while (this.numTryDecode < adjustPoints.Length)
            {
                try
                {
                    QRCodeDecoder.DecodeResult decodeResult = this.Decode(qrCodeImage, adjustPoints[this.numTryDecode]);
                    if (decodeResult.IsCorrectionSucceeded)
                        return decodeResult.DecodedBytes;
                    arrayList.Add((object)decodeResult);
                    QRCodeDecoder.canvas.Print("Decoding succeeded but could not correct");
                    QRCodeDecoder.canvas.Print("all errors. Retrying..");
                }
                catch (DecodingFailedException ex)
                {
                    if (ex.Message.IndexOf("Finder Pattern") >= 0)
                        throw ex;
                }
                finally
                {
                    ++this.numTryDecode;
                }
            }
            if (arrayList.Count == 0)
                throw new DecodingFailedException("Give up decoding");
            int index1 = -1;
            int num = int.MaxValue;
            for (int index2 = 0; index2 < arrayList.Count; ++index2)
            {
                QRCodeDecoder.DecodeResult decodeResult = (QRCodeDecoder.DecodeResult)arrayList[index2];
                if (decodeResult.NumCorrectionFailures < num)
                {
                    num = decodeResult.NumCorrectionFailures;
                    index1 = index2;
                }
            }
            QRCodeDecoder.canvas.Print("All trials need for correct error");
            QRCodeDecoder.canvas.Print("Reporting #" + (object)index1 + " that,");
            QRCodeDecoder.canvas.Print("corrected minimum errors (" + (object)num + ")");
            QRCodeDecoder.canvas.Print("Decoding finished.");
            return ((QRCodeDecoder.DecodeResult)arrayList[index1]).DecodedBytes;
        }

        public virtual string Decode(QRCodeImage qrCodeImage, Encoding encoding)
        {
            sbyte[] numArray = this.DecodeBytes(qrCodeImage);
            byte[] bytes = new byte[numArray.Length];
            Buffer.BlockCopy((Array)numArray, 0, (Array)bytes, 0, bytes.Length);
            return encoding.GetString(bytes);
        }

        public virtual string Decode(QRCodeImage qrCodeImage)
        {
            sbyte[] numArray = this.DecodeBytes(qrCodeImage);
            byte[] bytes = new byte[numArray.Length];
            Buffer.BlockCopy((Array)numArray, 0, (Array)bytes, 0, bytes.Length);
            return Encoding.GetEncoding(StringHelper.GuessEncoding(bytes)).GetString(bytes);
        }

        internal virtual QRCodeDecoder.DecodeResult Decode(QRCodeImage qrCodeImage, Point adjust)
        {
            try
            {
                if (this.numTryDecode == 0)
                {
                    QRCodeDecoder.canvas.Print("Decoding started");
                    int[][] intArray = this.imageToIntArray(qrCodeImage);
                    this.imageReader = new QRCodeImageReader();
                    this.qrCodeSymbol = this.imageReader.GetQRCodeSymbol(intArray);
                }
                else
                {
                    QRCodeDecoder.canvas.Print("--");
                    QRCodeDecoder.canvas.Print("Decoding restarted #" + (object)this.numTryDecode);
                    this.qrCodeSymbol = this.imageReader.GetQRCodeSymbolWithAdjustedGrid(adjust);
                }
            }
            catch (SymbolNotFoundException ex)
            {
                throw new DecodingFailedException(ex.Message);
            }
            QRCodeDecoder.canvas.Print("Created QRCode symbol.");
            QRCodeDecoder.canvas.Print("Reading symbol.");
            QRCodeDecoder.canvas.Print("Version: " + this.qrCodeSymbol.VersionReference);
            QRCodeDecoder.canvas.Print("Mask pattern: " + this.qrCodeSymbol.MaskPatternRefererAsString);
            int[] blocks1 = this.qrCodeSymbol.Blocks;
            QRCodeDecoder.canvas.Print("Correcting data errors.");
            int[] blocks2 = this.CorrectDataBlocks(blocks1);
            try
            {
                return new QRCodeDecoder.DecodeResult(this, this.GetDecodedByteArray(blocks2, this.qrCodeSymbol.Version, this.qrCodeSymbol.NumErrorCollectionCode), this.numLastCorrectionFailures);
            }
            catch (InvalidDataBlockException ex)
            {
                QRCodeDecoder.canvas.Print(ex.Message);
                throw new DecodingFailedException(ex.Message);
            }
        }

        internal virtual int[][] imageToIntArray(QRCodeImage image)
        {
            int width = image.Width;
            int height = image.Height;
            int[][] numArray = new int[width][];
            for (int index = 0; index < width; ++index)
                numArray[index] = new int[height];
            for (int y = 0; y < height; ++y)
            {
                for (int x = 0; x < width; ++x)
                    numArray[x][y] = image.GetPixel(x, y);
            }
            return numArray;
        }

        internal virtual int[] CorrectDataBlocks(int[] blocks)
        {
            int num1 = 0;
            int num2 = 0;
            int dataCapacity = this.qrCodeSymbol.DataCapacity;
            int[] numArray1 = new int[dataCapacity];
            int errorCollectionCode = this.qrCodeSymbol.NumErrorCollectionCode;
            int numRsBlocks = this.qrCodeSymbol.NumRSBlocks;
            int num3 = errorCollectionCode / numRsBlocks;
            if (numRsBlocks == 1)
            {
                int num4 = new RsDecode(num3 / 2).Decode(blocks);
                if (num4 > 0)
                {
                    int num5 = num1 + num4;
                }
                else if (num4 < 0)
                {
                    int num6 = num2 + 1;
                }
                return blocks;
            }
            int length1 = dataCapacity % numRsBlocks;
            if (length1 == 0)
            {
                int length2 = dataCapacity / numRsBlocks;
                int[][] numArray2 = new int[numRsBlocks][];
                for (int index = 0; index < numRsBlocks; ++index)
                    numArray2[index] = new int[length2];
                int[][] numArray3 = numArray2;
                for (int index1 = 0; index1 < numRsBlocks; ++index1)
                {
                    for (int index2 = 0; index2 < length2; ++index2)
                        numArray3[index1][index2] = blocks[index2 * numRsBlocks + index1];
                    int num4 = new RsDecode(num3 / 2).Decode(numArray3[index1]);
                    if (num4 > 0)
                        num1 += num4;
                    else if (num4 < 0)
                        ++num2;
                }
                int num5 = 0;
                for (int index1 = 0; index1 < numRsBlocks; ++index1)
                {
                    for (int index2 = 0; index2 < length2 - num3; ++index2)
                        numArray1[num5++] = numArray3[index1][index2];
                }
            }
            else
            {
                int length2 = dataCapacity / numRsBlocks;
                int length3 = dataCapacity / numRsBlocks + 1;
                int length4 = numRsBlocks - length1;
                int[][] numArray2 = new int[length4][];
                for (int index = 0; index < length4; ++index)
                    numArray2[index] = new int[length2];
                int[][] numArray3 = numArray2;
                int[][] numArray4 = new int[length1][];
                for (int index = 0; index < length1; ++index)
                    numArray4[index] = new int[length3];
                int[][] numArray5 = numArray4;
                for (int index1 = 0; index1 < numRsBlocks; ++index1)
                {
                    if (index1 < length4)
                    {
                        int num4 = 0;
                        for (int index2 = 0; index2 < length2; ++index2)
                        {
                            if (index2 == length2 - num3)
                                num4 = length1;
                            numArray3[index1][index2] = blocks[index2 * numRsBlocks + index1 + num4];
                        }
                        int num5 = new RsDecode(num3 / 2).Decode(numArray3[index1]);
                        if (num5 > 0)
                            num1 += num5;
                        else if (num5 < 0)
                            ++num2;
                    }
                    else
                    {
                        int num4 = 0;
                        for (int index2 = 0; index2 < length3; ++index2)
                        {
                            if (index2 == length2 - num3)
                                num4 = length4;
                            numArray5[index1 - length4][index2] = blocks[index2 * numRsBlocks + index1 - num4];
                        }
                        int num5 = new RsDecode(num3 / 2).Decode(numArray5[index1 - length4]);
                        if (num5 > 0)
                            num1 += num5;
                        else if (num5 < 0)
                            ++num2;
                    }
                }
                int num6 = 0;
                for (int index1 = 0; index1 < numRsBlocks; ++index1)
                {
                    if (index1 < length4)
                    {
                        for (int index2 = 0; index2 < length2 - num3; ++index2)
                            numArray1[num6++] = numArray3[index1][index2];
                    }
                    else
                    {
                        for (int index2 = 0; index2 < length3 - num3; ++index2)
                            numArray1[num6++] = numArray5[index1 - length4][index2];
                    }
                }
            }
            if (num1 > 0)
                QRCodeDecoder.canvas.Print(Convert.ToString(num1) + " data errors corrected successfully.");
            else
                QRCodeDecoder.canvas.Print("No errors found.");
            this.numLastCorrectionFailures = num2;
            return numArray1;
        }

        internal virtual sbyte[] GetDecodedByteArray(int[] blocks, int version, int numErrorCorrectionCode)
        {
            QRCodeDataBlockReader codeDataBlockReader = new QRCodeDataBlockReader(blocks, version, numErrorCorrectionCode);
            sbyte[] dataByte;
            try
            {
                dataByte = codeDataBlockReader.DataByte;
            }
            catch (InvalidDataBlockException ex)
            {
                throw ex;
            }
            return dataByte;
        }

        internal virtual string GetDecodedString(int[] blocks, int version, int numErrorCorrectionCode)
        {
            QRCodeDataBlockReader codeDataBlockReader = new QRCodeDataBlockReader(blocks, version, numErrorCorrectionCode);
            string dataString;
            try
            {
                dataString = codeDataBlockReader.DataString;
            }
            catch (IndexOutOfRangeException ex)
            {
                throw new InvalidDataBlockException(ex.Message);
            }
            return dataString;
        }

        internal class DecodeResult
        {
            private int numCorrectionFailures;
            internal sbyte[] decodedBytes;
            private QRCodeDecoder enclosingInstance;

            public DecodeResult(QRCodeDecoder enclosingInstance, sbyte[] decodedBytes, int numCorrectionFailures)
            {
                this.InitBlock(enclosingInstance);
                this.decodedBytes = decodedBytes;
                this.numCorrectionFailures = numCorrectionFailures;
            }

            private void InitBlock(QRCodeDecoder enclosingInstance)
            {
                this.enclosingInstance = enclosingInstance;
            }

            public virtual sbyte[] DecodedBytes
            {
                get
                {
                    return this.decodedBytes;
                }
            }

            public virtual int NumCorrectionFailures
            {
                get
                {
                    return this.numCorrectionFailures;
                }
            }

            public virtual bool IsCorrectionSucceeded
            {
                get
                {
                    return this.enclosingInstance.numLastCorrectionFailures == 0;
                }
            }

            public QRCodeDecoder EnclosingInstance
            {
                get
                {
                    return this.enclosingInstance;
                }
            }
        }
    }
}
