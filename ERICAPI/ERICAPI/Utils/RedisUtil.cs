using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace ERICAPI.Utils
{
    /// <summary>
    /// 调用Redis API工具类
    /// </summary>
    public class RedisUtil
    {
        /// <summary>
        /// 调用API
        /// </summary>
        /// <param name="send"></param>
        /// <param name="url"></param>
        /// <returns></returns>
        public static string Call(string send, string url)
        {
            byte[] buffer = Encoding.UTF8.GetBytes(send);
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.ContentType = "application/json";
            request.Method = "POST";
            request.GetRequestStream().Write(buffer, 0, buffer.Length);
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            StreamReader reader = new StreamReader(response.GetResponseStream(), Encoding.UTF8);
            return reader.ReadToEnd();
        }

        /// <summary>
        /// 取值
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string GetRedisValue(string key)
        {
            var data = new
            {
                key = key
            };
            try
            {
                string result = Call(JsonConvert.SerializeObject(data), "http://172.19.165.22:8085/redisapi/api/redis/getvalue");
                if ((int)JObject.Parse(result)["Status"] == 0)
                    return JObject.Parse(result)["Data"].ToString();
                else
                    return null;
            }
            catch (Exception e)
            {
                return null;
                throw e;
            }
        }

        /// <summary>
        /// 存值
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="expireSeconds"></param>
        /// <returns></returns>
        public static bool SetRedisValue(string key, object value, int expireSeconds = 10 * 60)
        {
            var data = new
            {
                key = key,
                value = value,
                expireSeconds = expireSeconds
            };
            try
            {
                string result = Call(JsonConvert.SerializeObject(data), "http://172.19.165.22:8085/redisapi/api/redis/setvalue");
                return ((int)JObject.Parse(result)["Status"] == 0);
            }
            catch (Exception e)
            {
                return false;
                throw e;
            }

        }

        /// <summary>
        /// 是否存在
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static bool ExistsKey(string key)
        {
            var data = new
            {
                key = key
            };
            try
            {
                string result = Call(JsonConvert.SerializeObject(data), "http://172.19.165.22:8085/redisapi/api/redis/Exists");
                if ((int)JObject.Parse(result)["Status"] == 0)
                    return (bool)(JObject.Parse(result)["Data"]);
                else
                    return false;

            }
            catch (Exception e)
            {
                return false;
                throw e;
            }

        }
    }

}
