using AForge.Video.DirectShow;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows.Forms;

namespace baiduAI
{
    public partial class Form1 : Form
    {
        private long lastTime = DateTime.Now.ToFileTime();
        private long sleepTime = 100 * 10000;
        private bool run = true;
        private byte[] bytes;
        private VideoCaptureDevice videoDevice;
        private FaceAi baiduAi;
        private VoiceAi voiceAi;
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            FilterInfoCollection fic = new FilterInfoCollection(FilterCategory.VideoInputDevice);
            videoDevice = new VideoCaptureDevice(fic[0].MonikerString);
            videoDevice.DisplayPropertyPage(IntPtr.Zero);
            videoDevice.NewFrame += VideoDevice_NewFrame;
            videoDevice.Start();

            baiduAi = FaceAi.GetInstance();
            voiceAi = VoiceAi.GetInstance();

            Thread mythread = new Thread(ThreadMain);
            mythread.Start();

            string voiceFilePath = voiceAi.Tts("百度AI");
            voiceAi.Play(voiceFilePath);

        }

        private void VideoDevice_NewFrame(object sender, AForge.Video.NewFrameEventArgs eventArgs)
        {
            long currentTime = DateTime.Now.ToFileTime();
            if (currentTime - lastTime < sleepTime)
            {
                return;
            }

            Bitmap bitmap = (Bitmap)eventArgs.Frame.Clone();
            Bitmap zoom = Images.ZoomImage(bitmap, pictureBox1.Width, pictureBox1.Height);
            if (bytes == null) {
                bytes = Images.bitmap2byte(Images.ZoomP(zoom, 0.25f,0.25f));
            }

            lastTime = currentTime;
            pictureBox1.Image = zoom;
            bitmap.Dispose();
        }


        private void ThreadMain()
        {
            while (run)
            {
                if (bytes != null)
                {
                    try
                    {
                        JToken jtk = baiduAi.Match(bytes);
                        IEnumerable<JToken> jtks = jtk.SelectTokens("result");
                        if (jtks != null)
                        {
                            for (int i = 0; i < jtks.Count(); i++)
                            {
                                if (Double.Parse(jtks.ElementAt(i)[0].SelectToken("scores")[0].ToString()) > 65D) {
                                    string voiceFilePath = voiceAi.Tts(jtks.ElementAt(i)[0].SelectToken("user_info").ToString());
                                    voiceAi.Play(voiceFilePath);
                                }
                            }
                        }

                        Thread.Sleep(200);
                    }
                    catch (Exception)
                    {
                        Console.WriteLine("百度AI异常");
                    }
                    finally
                    {
                        bytes = null;
                    }

                }
            }
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            run = false;
            videoDevice.Stop();
        }


    }
}
