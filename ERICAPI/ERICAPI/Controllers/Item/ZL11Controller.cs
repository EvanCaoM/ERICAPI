using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ERICAPI.Models;
using ERICAPI.Models.Repositories;
using ERICAPI.Utils;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;

namespace ERICAPI.Controllers.Item
{
    /// <summary>
    /// 料号归并
    /// </summary>
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class ZL11Controller : ControllerBase
    {
        private IZL11Repository _zl11Repository;

        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="zl11Repository"></param>
        public ZL11Controller(IZL11Repository zl11Repository)
        {
            _zl11Repository = zl11Repository;
        }

        /// <summary>
        /// 查询
        /// </summary>
        /// <param name="form"></param>
        /// <returns></returns>
        [HttpPost]
        public ServerResponse<JArray> Query(JObject form)
        {
            //TODO。。。判空
            //太慢
            Dictionary<string, string> formdata = form.ToObject<Dictionary<string, string>>();
            return ServerResponse<JArray>.CreateBySuccess(JArray.FromObject(_zl11Repository.Query(formdata)));
        }

        [HttpPost]
        public ServerResponse<string> Save(JArray form)
        {
            var userName = GetUser();
            IList<VIEW_spare_All> zl11s = form.ToObject<IList<VIEW_spare_All>>();
            foreach(var zl11 in zl11s)
            {
                zl11.CHNAME = userName;
                zl11.CHDATE = DateTime.Now.Date.ToString("yyyyMMdd");
            }

        }

        /// <summary>
        /// 根据ip从redis中获取工号
        /// </summary>
        /// <returns></returns>
        private string GetUser()
        {
            var ip = HttpContext.Connection.RemoteIpAddress;//ip地址
            return RedisUtil.GetRedisValue("ip:" + ip.ToString());
        }

    }
}
