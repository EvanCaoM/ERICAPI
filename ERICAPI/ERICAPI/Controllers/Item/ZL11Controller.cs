using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.Security;
using System.Threading.Tasks;
using ERICAPI.Models;
using ERICAPI.Models.Repositories;
using ERICAPI.Utils;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.SqlServer.Migrations.Internal;
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
        private ItdsYitemRepository _tdsYitemRepository;


        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="zl11Repository"></param>
        /// <param name="tdsYitemRepository"></param>
        public ZL11Controller(IZL11Repository zl11Repository, ItdsYitemRepository tdsYitemRepository)
        {
            _zl11Repository = zl11Repository;
            _tdsYitemRepository = tdsYitemRepository;

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
            var result = _zl11Repository.Query(form);
            if(result == null || result.Count() == 0)
                return ServerResponse<JArray>.CreateByErrorMessage("无数据！");
            else
                return ServerResponse<JArray>.CreateBySuccess(JArray.FromObject(_zl11Repository.Query(form)));
        }

        /// <summary>
        /// 保存
        /// </summary>
        /// <param name="form"></param>
        /// <returns></returns>
        [HttpPost]
        public ServerResponse<string> Save(JArray form)
        {
            var userName = GetUser();
            string strBeiZhu = "";
            bool flag = false;
            IList<VIEW_spare_All> zl11s = form.ToObject<IList<VIEW_spare_All>>();
            foreach (var zl11 in zl11s)
            {
                zl11.CHNAME = userName;
                zl11.CHDATE = DateTime.Now.Date.ToString("yyyyMMdd");

                if (!zl11.Type.Equals("放弃退税")
                    && _zl11Repository.CheckDiff(zl11.BUKRS, zl11.MATNR, zl11.SMAKTX, zl11.TAX_CODE, zl11.CGEWEI, zl11.DECLITEM)
                    && !_zl11Repository.GetCtrl("QSBN_CheckDiff", "01").drpValue.Contains(userName))
                    return ServerResponse<string>.CreateByErrorMessage("料号" + zl11.MATNR + "备案不一致，请确认！");

                #region 国内Vendor Beizhu必填  境外Vendor 能效标识必填
                if (zl11.Vendorcode.Equals("QCI_MRO") || zl11.Vendorcode.Substring(zl11.Vendorcode.Length - 1, 1) == "F")
                {
                    if ((_tdsYitemRepository.Is3C(zl11.BUKRS, zl11.DECLITEM) & zl11.C3FLAG == "不涉及")
                        & (_tdsYitemRepository.IsEnergy(zl11.BUKRS, zl11.DECLITEM) & zl11.CELFLAG == "不涉及"))
                    {
                        strBeiZhu += "【" + zl11.BUKRS + ',' + zl11.DECLITEM + ',' + zl11.MATNR + "涉及3C验证；"
                            + zl11.Vendorcode + ',' + zl11.MATNR + "涉及能效标识" + "】";
                    }
                    else if ((_tdsYitemRepository.Is3C(zl11.BUKRS, zl11.DECLITEM) & zl11.C3FLAG == "不涉及"))
                    {
                        strBeiZhu += "【" + zl11.BUKRS + ',' + zl11.DECLITEM + ',' + zl11.MATNR + "涉及3C验证】";
                    }
                    else if (_tdsYitemRepository.IsEnergy(zl11.BUKRS, zl11.DECLITEM) & zl11.CELFLAG == "不涉及")
                    {
                        strBeiZhu += "【" + zl11.BUKRS + ',' + zl11.Vendorcode + ',' + zl11.MATNR + "涉及能效标识" + "】";
                    }
                    else
                    {
                        if (zl11.BRGEW != "")//BRGEW zhengsui
                        {
                            strBeiZhu += "【" + zl11.BUKRS + ',' + zl11.Vendorcode + ',' + zl11.MATNR + "涉及" + zl11.BRGEW + "】";
                        }
                        flag = InsertMro(zl11);

                    }
                }
                else if (zl11.Vendorcode.Substring(zl11.Vendorcode.Length - 1, 1) == "L"
                    || zl11.Vendorcode != "QCI_MRO")
                {
                    if (zl11.C3REMARK == "无备注")
                    {
                        strBeiZhu += "【" + zl11.BUKRS + ',' + zl11.Vendorcode + ',' + zl11.MATNR + "备注栏位必填" + "】";
                    }
                    else
                    {
                        flag = InsertMro(zl11);
                    }
                }

                #endregion
            }

            //todo...有问题，循环插入没办法判断是否保存成功
            if (flag)
                return ServerResponse<string>.CreateBySuccessMessage("Save Successfully" + strBeiZhu);
            else
                return ServerResponse<string>.CreateByErrorMessage("Save Error" + strBeiZhu);


        }

        /// <summary>
        /// 加解锁权限
        /// </summary>
        /// <param name="form"></param>
        /// <returns></returns>
        [HttpPost]
        public ServerResponse<string> LockAuthority(JObject form)
        {
            if (_zl11Repository.GetCtrl("QSBN_LOCK", "01").drpValue.Contains(GetUser()))
                return ServerResponse<string>.CreateBySuccessMessage("OK!");
            else
                return ServerResponse<string>.CreateByErrorMessage("操作错误，仅关务有加锁和解锁的权限 !");

        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="form"></param>
        /// <returns></returns>
        [HttpPost]
        public ServerResponse<string> Delete(JObject form)
        {
            JArray forms = JArray.FromObject(form["data"]);
            var delReason = form["delReason"].ToString();
            try
            {
                foreach (var data in forms)
                {
                    _zl11Repository.Delete(form["Type"].ToString(), form["MANDT"].ToString(),
                        form["BUKRS"].ToString(), form["MATNR"].ToString(), form["Vendorcode"].ToString(), form["DECLITEM"].ToString(),
                        GetUser(), delReason);
                }

                return ServerResponse<string>.CreateBySuccessMessage("删除成功！");
            }
            catch
            {
                return ServerResponse<string>.CreateByErrorMessage("删除失败！");
            }
        }

        /// <summary>
        /// 加解锁
        /// </summary>
        /// <param name="form"></param>
        /// <returns></returns>
        [HttpPost]
        public ServerResponse<string> LockClick(JObject form)
        {
            try
            {
                _zl11Repository.LockClick(form["BUKRS"].ToString(), form["MATNR"].ToString(), form["Vendorcode"].ToString(), form["Type"].ToString(), form["LOCK"].ToString());
                return ServerResponse<string>.CreateBySuccessMessage("Success");
            }
            catch(Exception e)
            {
                return ServerResponse<string>.CreateByErrorMessage("Error" + e.Message);
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

        /// <summary>
        /// 插入归并数据
        /// </summary>
        /// <param name="zl11"></param>
        /// <returns></returns>
        private bool InsertMro(VIEW_spare_All zl11)
        {
            bool flag1, flag2 = true;
            if (!string.IsNullOrEmpty(zl11.Type))
                flag1 = _zl11Repository.InsertMroNew(zl11);
            else
            {
                flag1 = _zl11Repository.InsertCmro(zl11);
                flag2 = _zl11Repository.InsertMro(zl11);
            }
            return flag1 && flag2;
        }



    }
}
