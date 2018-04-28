using Baidu.Aip.Face;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NLog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace baiduAI
{
    public class FaceAi
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();
        private string API_KEY = "k5GdSjCS2NGGOD2EACfBlON9";
        private string SECRET_KEY = "cuXGl8i3Uo5zHG47qC7Y73qKsFbd3g64";

        private static FaceAi single = new FaceAi();

        private FaceAi()
        {
        }

        public static FaceAi GetInstance()
        {
            return single;
        }

        public JObject Match(byte[] bytes)
        {
            logger.Debug("正在匹配人脸,大小：{} kb",bytes.Length * 1f / 1024);
            Face face = new Face(API_KEY, SECRET_KEY);
            JObject result = face.Identify("test", bytes);
            logger.Debug("匹配人脸结果：{}", result.ToString());
            return result;
        }

        public bool UserAddDemo(string uid, string userInfo, byte[] image)
        {
            logger.Info("添加用户，编号：{} 姓名：{} 图片:{}kb", uid, userInfo, image.Length * 1f / 1024);
            Face face = new Face(API_KEY, SECRET_KEY);

            var groupId = "test";

            // 如果有可选参数
            var options = new Dictionary<string, object> { { "action_type", "replace" } };
            // 带参数调用人脸注册
            JObject result = face.UserAdd(uid, userInfo, groupId, image, options);
            logger.Info("添加用户结果：{}",result.ToString());
            return result.SelectToken("error_code") == null;
        }

        public bool UserDelDemo(string uid)
        {
            logger.Info("删除用户，编号：{}", uid);
            Face face = new Face(API_KEY, SECRET_KEY);
            // 调用人脸删除，可能会抛出网络等异常，请使用try/catch捕获
            var result = face.UserDelete(uid);
            logger.Info("删除用户结果：{}", result.ToString());
            // 如果有可选参数
            return result.SelectToken("error_code") == null;
        }
    }
}
