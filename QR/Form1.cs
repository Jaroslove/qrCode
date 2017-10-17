using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace QR
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            using (SaveFileDialog save = new SaveFileDialog() { Filter = "JPEG|*.jpg", ValidateNames = true })
            {
                if(save.ShowDialog() == DialogResult.OK)
                {
                    MessagingToolkit.QRCode.Codec.QRCodeEncoder encoder = new MessagingToolkit.QRCode.Codec.QRCodeEncoder();
                    encoder.QRCodeScale = 8;
                    Bitmap bitmap = encoder.Encode(textBox1.Text);
                    pictureBox1.Image = bitmap;
                    bitmap.Save(save.FileName, ImageFormat.Jpeg);
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog open = new OpenFileDialog() { Filter = "JPEG|*.jpg", ValidateNames = true, Multiselect = false})
            {
                if(open.ShowDialog() == DialogResult.OK)
                {
                    pictureBox1.Image = Image.FromFile(open.FileName);
                    MessagingToolkit.QRCode.Codec.QRCodeDecoder decoder = new MessagingToolkit.QRCode.Codec.QRCodeDecoder();
                    textBox2.Text = decoder.Decode(new MessagingToolkit.QRCode.Codec.Data.QRCodeBitmapImage(pictureBox1.Image as Bitmap));
                }
            }
        }
    }
}
