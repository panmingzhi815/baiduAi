using Baidu.Aip.Speech;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace faceAI
{
    class Voices
    {
        private static Voices voices = new Voices();
        private string voiceCacheFloder = "repositoryVoices";
        static string API_KEY = "kdh2shLbaPyehC9PD1KyZGn9";
        static string SECRET_KEY = "28f29c73f89f1c776f7404212acf0eaa ";

        private Dictionary<string, long> playHistory = new Dictionary<string, long>();

        private Voices() { }

        public static Voices getInstance()
        {
            return voices;
        }

        public string Tts(string key, string text)
        {
            string path = voiceCacheFloder + "/" + key + ".mp3";

            if (!Directory.Exists(voiceCacheFloder))
            {
                Directory.CreateDirectory(voiceCacheFloder);
            }

            if (File.Exists(path))
            {
                return path;
            }

            Tts _ttsClient = new Baidu.Aip.Speech.Tts(API_KEY, SECRET_KEY);
            // 可选参数
            var option = new Dictionary<string, object>()
            {
                {"spd", 5}, // 语速
                {"vol", 7}, // 音量
                {"per", 1}  // 发音人，4：情感度丫丫童声
            };
            var result = _ttsClient.Synthesis(text, option);

            if (result.ErrorCode == 0)  // 或 result.Success
            {
                File.WriteAllBytes(path, result.Data);
                return path;
            }

            throw new IOException("语音合成失败");
        }

        public void play(string filepath)
        {
            long time;
            playHistory.TryGetValue(filepath, out time);
            long filetime = DateTime.Now.ToFileTime();
            if (time != 0 && filetime - time < 15000000)
            {
                playHistory[filepath] = filetime;
                return;
            }

            playHistory[filepath] = filetime;

            Voices.mciSendString("close all", "", 0, 0);
            Voices.mciSendString("open " + filepath + " alias media", "", 0, 0);
            Voices.mciSendString("play media", "", 0, 0);

            
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
