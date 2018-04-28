using Baidu.Aip.Speech;
using NLog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Media;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;

namespace baiduAI
{
    class VoiceAi
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();
        private static VoiceAi voices = new VoiceAi();
        private string voiceCacheFloder = "voiceTemp";
        private MD5 md5 = MD5.Create();
        static string API_KEY = "kdh2shLbaPyehC9PD1KyZGn9";
        static string SECRET_KEY = "28f29c73f89f1c776f7404212acf0eaa ";

        private Dictionary<string, long> playHistory = new Dictionary<string, long>();

        private VoiceAi() { }

        public static VoiceAi GetInstance()
        {
            return voices;
        }


        public string Tts(string text)
        {

            if (text.Length == 0)
            {
                text = "无名";
            }

            logger.Info("语音合成：{}", text);

            string path = voiceCacheFloder + "/" + text + ".mp3";

            if (!Directory.Exists(voiceCacheFloder))
            {
                Directory.CreateDirectory(voiceCacheFloder);
            }

            if (File.Exists(path))
            {
                logger.Info("己存在[{}]语音合成文件：{}", text, path);
                return path;
            }

            Tts _ttsClient = new Tts(API_KEY, SECRET_KEY);
            var option = new Dictionary<string, object>()
            {
                {"spd", 5}, // 语速
                {"vol", 7}, // 音量
                {"per", 1}  // 发音人，4：情感度丫丫童声
            };
            var result = _ttsClient.Synthesis(text, option);

            if (result.ErrorCode == 0)
            {
                File.WriteAllBytes(path, result.Data);
                return path;
            }

            throw new IOException("语音合成失败");
        }

        public void Play(string filepath)
        {

            playHistory.TryGetValue(filepath, out long time);
            long filetime = DateTime.Now.ToFileTime();
            if (time != 0 && filetime - time < 15000000)
            {
                playHistory[filepath] = filetime;
                return;
            }
            logger.Info("播放音频:{}", filepath);

            mciSendString("close all", "", 0, 0);
            mciSendString("open " + filepath + " alias media", "", 0, 0);
            mciSendString("play media", "", 0, 0);

            playHistory[filepath] = filetime;

        }

        [DllImport("winmm.dll", EntryPoint = "mciSendString", CharSet = CharSet.Auto)]
        public static extern int mciSendString(
           string lpstrCommand,
           string lpstrReturnString,
           int uReturnLength,
           int hwndCallback
          );


    }
}
