using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml.Linq;
using AForge.Video.DirectShow;
using baiduAI;

namespace baiduAiApp
{
    public partial class Form1 : Form
    {
        private VideoCaptureDevice videoDevice;
        private object bytes = new byte[0];

        public Form1()
        {
            InitializeComponent();
        }

        private void pictureBox1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            openFileDialog1.ShowDialog();
        }

        private void openFileDialog1_FileOk(object sender, CancelEventArgs e)
        {
            string filename = openFileDialog1.FileName;
            pictureBox1.Image = Bitmap.FromFile(filename);
        }

        private void 提交_Click(object sender, EventArgs e)
        {
            MemoryStream ms = new MemoryStream();
            pictureBox1.Image.Save(ms, ImageFormat.Jpeg);

            FaceAi face = FaceAi.GetInstance();
            bool b = face.UserAddDemo(uid.Text, user_info.Text, ms.ToArray());
            if (b)
            {
                MessageBox.Show("操作成功");
            }
            else
            {
                MessageBox.Show("操作失败");
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            bytes = null;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            FilterInfoCollection fic = new FilterInfoCollection(FilterCategory.VideoInputDevice);
            videoDevice = new VideoCaptureDevice(fic[0].MonikerString);
            videoDevice.NewFrame += VideoDevice_NewFrame;
            videoDevice.Start();
        }

        private void VideoDevice_NewFrame(object sender, AForge.Video.NewFrameEventArgs eventArgs)
        {
            Bitmap bitmap = (Bitmap)eventArgs.Frame.Clone();
            if (bytes == null)
            {
                bytes = new byte[0];
                pictureBox1.Image = Images.ZoomP(bitmap, pictureBox1.Width * 1f / bitmap.Width, pictureBox1.Height * 1f / bitmap.Height);
            }
            else {
                pictureBox2.Image = Images.ZoomP(bitmap, pictureBox2.Width * 1f / bitmap.Width, pictureBox2.Height * 1f / bitmap.Height);
            }

            
            bitmap.Dispose();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            FaceAi face = FaceAi.GetInstance();
            bool b = face.UserDelDemo(uid.Text);
            if (b)
            {
                MessageBox.Show("操作成功");
            }
            else
            {
                MessageBox.Show("操作失败");
            }
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            videoDevice.Stop();
        }
    }
}
