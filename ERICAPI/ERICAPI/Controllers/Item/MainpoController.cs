using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using ERICAPI.Models;
using ERICAPI.Models.Repositories;
using System.Threading;
using ERICAPI.Utils;
using Newtonsoft.Json;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.Security.Cryptography.X509Certificates;
using Microsoft.EntityFrameworkCore.Internal;

namespace ERICAPI.Controllers.Item
{
    /// <summary>
    /// Mainpo页面
    /// </summary>
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class MainpoController : ControllerBase
    {
        private ItdsAupoRepository _tdsAupoRepository;
        private ItdsYitemRepository _tdsYitemRepository;

        /// <summary>
        /// 实例化Repository
        /// </summary>
        /// <param name="tdsAupoRepository"></param>
        /// <param name="tdsYitemRepository"></param>
        public MainpoController(ItdsAupoRepository tdsAupoRepository, ItdsYitemRepository tdsYitemRepository)
        {
            _tdsAupoRepository = tdsAupoRepository;
            _tdsYitemRepository = tdsYitemRepository;
        }

        /// <summary>
        /// 查询
        /// </summary>
        /// <param name="form"></param>
        /// <returns></returns>
        [HttpPost]
        public JArray Query(JObject form)
        {
            //TODO。。。判空
            //太慢
            //Dictionary<string, string> formdata = form.ToObject<Dictionary<string, string>>();
            //return JArray.FromObject(_tdsAupoRepository.GetMainpo(formdata));


            v_sIFREupoMain queryForm = new v_sIFREupoMain();
            queryForm.BUKRS = form["BUKRS"].ToString().Trim();
            queryForm.EBELN = form["EBELN"].ToString().Trim();
            queryForm.matnrE = form["matnrE"].ToString().Trim();
            queryForm.TXZ01 = form["TXZ01"].ToString().Trim();
            queryForm.DECLITEM = form["DECLITEM"].ToString().Trim();
            queryForm.STATUS = form["STATUS"].ToString().Trim();
            queryForm.LIFNR = form["LIFNR"].ToString().Trim();
            queryForm.vendortype = form["vendortype"].ToString().Trim();
            queryForm.CHNAME = form["CHNAME"].ToString().Trim();
            queryForm.BRGEW = form["BRGEW"].ToString().Trim();
            queryForm.CHType = form["CHType"].ToString().Trim();
            queryForm.MARK = form["MARK"].ToString().Trim();
            string purchdateFrom = form["purchdateFrom"].ToString().Trim();
            string purchdateTo = form["purchdateTo"].ToString().Trim();
            
            var results = JArray.FromObject(_tdsAupoRepository.GetMainpo(queryForm, purchdateFrom, purchdateTo));
            if (results == null)
                return null;
            else
            {
                var tcurr = string.Empty;
                if (results.First["BUKRS"].ToString().Equals("9000"))
                    results[0]["vendorname"] = _tdsAupoRepository.GetVendorname(results.First["LIFNR"].ToString());
                if (!results.First["WAERS"].ToString().Equals("RMB"))
                    tcurr = _tdsAupoRepository.GetTcurr(results.First["BUKRS"].ToString(), results.First["WAERS"].ToString());

                foreach (var result in results)
                {
                    result["MARK"] = string.IsNullOrEmpty(result["MARK"].ToString()) ? "0" : result["MARK"];
                    result["TCURR"] = tcurr;
                }
                return results;
            }
        }

        /// <summary>
        /// 保存
        /// </summary>
        /// <param name="forms"></param>
        /// <returns></returns>
        [HttpPost]
        public ServerResponse<string> Save(JArray forms)
        {
            IList<tdsAupo> tdsAupos = new List<tdsAupo>();
            string user = GetUser();


            if (string.IsNullOrEmpty(user))
                return ServerResponse<string>.CreateByErrorMessage("请求超时，请刷新主页重进！");
            foreach (var form in forms)
            {

                if (form["REMARK"].ToString().Equals("申请人确认抽单") && string.IsNullOrEmpty(form["DECLITEM"].ToString()))
                {
                }
                else
                {
                    # region 境外po
                    if (form["LIFNR"].ToString().Equals("QCI_MRO")
                        || form["LIFNR"].ToString().Substring(form["LIFNR"].ToString().Length - 1, 1) == "F")//境外PO
                    {
                        if (_tdsYitemRepository.Is3C(form["BUKRS"].ToString(), form["DECLITEM"].ToString())
                        && form["MARK"].ToString().Equals("0"))
                            return ServerResponse<string>.CreateByErrorMessage("此序号涉及3C认证!不能设置为不涉及！");
                        if (_tdsYitemRepository.IsEnergy(form["BUKRS"].ToString(), form["DECLITEM"].ToString())
                            && form["CELFLAG"].ToString().Equals("0"))
                            return ServerResponse<string>.CreateByErrorMessage("此序号涉及能源标识!不能设置为不涉及！");
                    }
                    #endregion
                    #region 境内po
                    else
                    {
                        string strFlag = _tdsYitemRepository.IsMonitorRule(form["BUKRS"].ToString(), form["DECLITEM"].ToString());
                        if (strFlag != "")
                        {
                            switch (strFlag)
                            {
                                case "3":
                                    return ServerResponse<string>.CreateByErrorMessage("此序号涉及证件：3两用物项和技术出口许可证 ");
                                    break;
                                case "4":
                                    return ServerResponse<string>.CreateByErrorMessage("此序号涉及证件：4出口许可证");
                                    break;
                                case "8":
                                    return ServerResponse<string>.CreateByErrorMessage("此序号涉及证件：8禁止出口商品");
                                    break;
                                case "G":
                                    return ServerResponse<string>.CreateByErrorMessage("此序号涉及证件：G两用物项和技术出口许可证(定向)");
                                    break;
                                default:
                                    break;
                            }
                        }
                    }
                    #endregion
                    if (form["vendortype"].ToString() == "自主报关" || form["vendortype"].ToString() == "代理报关")
                    {
                        #region 有料号的po：当有料号的po: 自主报关、代理报关、 在归类有料号的PO时，系统校验公司别+料号是否存在不一致的底账序号，如果有，系统提示；如果没有，系统保存成功
                        if (form["matnrE"].ToString() != "COASSET" && form["matnrE"].ToString() != "FOCCONSU" && form["matnrE"].ToString() != "")
                        {
                            if (!_tdsAupoRepository.IsExistZl11(form["BUKRS"].ToString(), form["matnrE"].ToString(), form["LIFNR"].ToString()))
                            {
                                return ServerResponse<string>.CreateByErrorMessage("料号:" + form["matnrE"].ToString() + "在ZL11未备案！");
                            }
                            if (!_tdsAupoRepository.SameDecMatnr(form["BUKRS"].ToString(), form["LIFNR"].ToString(), form["matnrE"].ToString(), form["DECLITEM"].ToString()))
                            {
                                return ServerResponse<string>.CreateByErrorMessage("归并序号与ZL11序号不一致，请核实！");
                            }
                            if (!_tdsAupoRepository.SameDecDiffEbeln(form["BUKRS"].ToString(), form["matnrE"].ToString(), form["DECLITEM"].ToString()))
                            {
                                if ((user != "03070472" && user != "06030239" && user != "A0070028" && user != "A0070116" && user != "A7100040" && user != "QSBN"))
                                {
                                    return ServerResponse<string>.CreateByErrorMessage("相同料号，不同PO中的底账序号不一致！");
                                }
                            }
                        }
                        #endregion
                        #region 无料号po：当没有料号的po:自主报关、代理报关、 同厂区（QSMC及QCMC公司别）、同VENDOR CODE、同Shorttext、归并底账序号不同时，无法保存
                        else
                        {
                            if (!"TFC_MRO,TFQ_MRO,TCC_MRO,TLC_MRO,TGC_MRO,TWW_MRO,TWQ_MRO,ZYS_MRO,ZYQ_MRO".Contains(form["LIFNR"].ToString()))
                            {
                                if (!_tdsAupoRepository.PoCheck(form["BUKRS"].ToString(), form["LIFNR"].ToString()
                                    , form["TXZ01"].ToString(), form["EBELN"].ToString()
                                    , form["vendortype"].ToString(), form["DECLITEM"].ToString())
                                && !"03070472,A0070028,A2132386,A0070116,06030239,QSBN,A7100040".Contains(user))
                                {
                                    return ServerResponse<string>.CreateByErrorMessage("ITEM:" + form["EBELP"].ToString() + "相同Short text已归并过底账序号:" + form["DECLITEM"].ToString() + ",请确认");
                                }
                            }
                        }
                        #endregion
                    }
                    #region 判断是否做过EC单1
                    if (_tdsAupoRepository.ECCheck(form["BUKRS"].ToString(), form["EBELN"].ToString(), form["EBELP"].ToString()))
                    {
                        return ServerResponse<string>.CreateByErrorMessage("该PO已做过EC单!");
                    }
                    #endregion
                }
                tdsAupo tdsAupo = new tdsAupo();
                tdsAupo.MANDT = form["MANDT"].ToString();
                tdsAupo.BUKRS = form["BUKRS"].ToString();

                tdsAupo.EBELN = form["EBELN"].ToString();
                //tdsAupo.EBELN = "111111111";
                tdsAupo.EBELP = form["EBELP"].ToString();
                tdsAupo.DECLITEM = form["DECLITEM"].ToString();
                tdsAupo.MATNR = form["MATNR"].ToString();
                tdsAupo.E_I = "I";
                tdsAupo.TAX_CODE = form["TAX_CODE"].ToString();
                tdsAupo.ZGEWEI = form["ZGEWEI"].ToString();
                tdsAupo.SMAKTX = form["SMAKTX"].ToString();
                tdsAupo.APPQTY = form["APPQTY"].ToObject<decimal>();
                tdsAupo.DECLQTY = form["DECLQTY"].ToObject<decimal>();
                tdsAupo.APFLAG = form["APFLAG"].ToString();
                tdsAupo.STATUS = form["STATUS"].ToString();
                tdsAupo.MARK = form["MARK"].ToString();
                tdsAupo.REMARK = form["REMARK"].ToString();
                tdsAupo.CHDATE = DateTime.Now;
                tdsAupo.CHNAME = user;
                tdsAupo.ZHENGSHUI = form["BRGEW"].ToString();
                tdsAupo.HOMEORABROAD = string.Empty;
                tdsAupo.CELFLAG = form["CELFLAG"].ToString();
                tdsAupo.ACCNO = form["BOITYP"].ToString();
                tdsAupos.Add(tdsAupo);
                // 管制
                string retrc = form["RETRC"].ToString();
                // 新旧
                string itemno = form["STATUS"].ToString();
                _tdsAupoRepository.UpdateOtherDatas(tdsAupo, retrc, itemno);
            }
            try
            {
                if (_tdsAupoRepository.AddTdsAupos(tdsAupos))
                {
                    SendMailApbno(forms);
                    return ServerResponse<string>.CreateBySuccessMessage("Save Successfully!");
                }
                else
                    return ServerResponse<string>.CreateByErrorMessage("Save Error!");
            }
            catch (Exception e)
            {
                return ServerResponse<string>.CreateByErrorMessage("Save Error!" + e.Message);
            }



            //Task task = _tdsAupoRepository.AddTdsAuposTask(tdsAupos);
            //Console.WriteLine("dada");
            //task.Wait();
            //Console.WriteLine("hahh");
            //Thread.Sleep(1000);
            //task.GetAwaiter().GetResult();

            //Console.WriteLine("hahh");

        }

        /// <summary>
        /// 查询底账序号
        /// </summary>
        /// <param name="form"></param>
        [HttpPost]
        public JArray QueryDeclitem(JObject form)
        {
            return JArray.FromObject(_tdsYitemRepository.GetDeclitems(form["BUKRS"].ToString(), form["DECLITEM"].ToString()));
        }

        /// <summary>
        /// 查询公司别
        /// </summary>
        /// <returns></returns>
        [HttpPost, HttpGet]
        public JArray GetComcode()
        {
            string userName = GetUser() ?? "QSBN";
            return JArray.FromObject(_tdsAupoRepository.GetComcode(userName));
        }

        /// <summary>
        /// 获取vendorname
        /// </summary>
        /// <param name="lifnr"></param>
        /// <returns></returns>
        [HttpPost]
        public string GetVendorcode(string lifnr)
        {
            return _tdsAupoRepository.GetVendorname(lifnr);
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
        /// 原进口PO
        /// </summary>
        /// <param name="form"></param>
        /// <returns></returns>
        [HttpPost]
        public ServerResponse<IEnumerable<View_ResalePOMappingInfo>> GetPrePO(JObject form)
        {
            string bukrs = form["BUKRS"].ToString().Trim();
            string ebeln = form["EBELN"].ToString().Trim();
            string ebelp = form["EBELP"].ToString().Trim();
            IEnumerable< View_ResalePOMappingInfo > result = _tdsAupoRepository.GetPrePO(bukrs, ebeln, ebelp);
            if (result.Count() == 0)
                return ServerResponse<IEnumerable<View_ResalePOMappingInfo>>.CreateByErrorMessage("尚无原始备案信息");
            else
                return ServerResponse<IEnumerable<View_ResalePOMappingInfo>>.CreateBySuccess(result);
        }

        /// <summary>
        /// 查询pr信息
        /// </summary>
        /// <param name="form"></param>
        /// <returns></returns>
        [HttpPost]
        public string QueryPR(JObject form)
        {
            string strWpoid = _tdsAupoRepository.QueryPR(form["BUKRS"].ToString().Trim(), form["EBELN"].ToString().Trim());
            string valFORMID = "GWF063";      //表單編號
            string valAPPLID = strWpoid;   //查詢單號
            string valCOMCOD = "QUANTACN";         //進行查詢的使用者公司別
            string valUSERID = "A7100040";    //進行查詢的使用者工號   
            var objSecurity = new WSSecurity.WSSecuritySoapClient(WSSecurity.WSSecuritySoapClient.EndpointConfiguration.WSSecuritySoap);
            string strDecrypt = valFORMID + ";" + valAPPLID + ";" + valCOMCOD + ";" + valUSERID;  //以分號分隔各參數，順序勿異動
            WSSecurity.WSEncrptionForMROResponse response = objSecurity.WSEncrptionForMROAsync(1, "GAMS", strDecrypt).Result;
            return HttpUtility.UrlEncode(response.Body.WSEncrptionForMROResult);   //URL編碼避免辨識不正確
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ServerResponse<bool> Delete(JObject form)
        {
            string userName = GetUser();
            if(string.IsNullOrEmpty(userName))
                return ServerResponse<bool>.CreateByErrorMessage("请求超时，请刷新主页重进！！");
            string message = string.Empty;
            IList<tdsAupo> tdsAupos = new List<tdsAupo>();
            var ctrl = _tdsAupoRepository.GetCtrl("QSBN_DelAupo", "01");
            if (!ctrl.drpValue.Contains(userName) && !ctrl.drpText.Contains(userName))
            {
                return ServerResponse<bool>.CreateByErrorMessage("没有删除权限！");
            }
            JArray datas = JArray.FromObject(form["data"]);
            foreach (var data in datas)
            {
                if (_tdsAupoRepository.ECCheck(data["BUKRS"].ToString(), data["EBELN"].ToString(), data["EBELP"].ToString()))
                {
                    message = "已做过EC单，不允许删除！";
                }
                if ((("9110,9210,9001").Contains(data["BUKRS"].ToString())) && ctrl.drpText.Contains(userName) ||
                    ("9100,9200,9600,9900,9000".Contains(data["BUKRS"].ToString())) && ctrl.drpValue.Contains(userName))
                {
                    tdsAupos.Add(_tdsAupoRepository.GetTdsAupo(data["BUKRS"].ToString(), data["EBELN"].ToString(), data["EBELP"].ToString()));
                }
                else
                {
                    message = "没有删除权限！";
                }

            }
            if (string.IsNullOrEmpty(message))
            {
                if(_tdsAupoRepository.RemoveTdsAupo(tdsAupos))
                    return ServerResponse<bool>.CreateBySuccessMessage("删除成功！" );
                else
                    return ServerResponse<bool>.CreateByErrorMessage("删除失败！请联系MIS");
            }
            else
            {
                return ServerResponse<bool>.CreateByErrorMessage("删除失败！" + message);
            }
        }

        private void CheckVendorcode(IEnumerable<tdsAupo> tdsAupos, string lifnr)
        {
            if (!tdsAupos.First().REMARK.Equals("申请人确认抽单"))
            {
                if (lifnr.Substring(lifnr.Length - 1, 1) == "L") 
                {
                    //tdsAupos.Select(x => new[] { new bukrs = x.BUKRS, fsf = x.APPQTY }).Distinct();
                }
                if (lifnr.Substring(lifnr.Length - 3, 3) == "MRO" && lifnr != "QCI_MRO")
                {
                }
            }
        }

        [HttpGet]
        public void Test()
        {
            var a = _tdsAupoRepository.GetCtrl("QSBN_DelAupo", "01");
        }

        /// <summary>
        /// 判断此PO的厂商是否在数据库tdsYven中存在，如果不存在说明是新厂商，经确认好像是没用了
        /// </summary>
        /// <param name="bukrs"></param>
        /// <param name="ebeln"></param>
        /// <param name="lifnr"></param>
        /// <returns></returns>
        private string GetNewVenName(string bukrs, string ebeln, string lifnr)
        {
            var strNewVenderName = string.Empty;
            var agent = _tdsAupoRepository.GetPoAgent(bukrs, ebeln);
            if (!string.IsNullOrEmpty(agent))
            {
                if (!_tdsAupoRepository.IsVenExistInDB(bukrs, agent))
                    strNewVenderName = agent;
            }
            if (_tdsAupoRepository.IsVenExistInDB(bukrs, lifnr))
                strNewVenderName = lifnr;
            return strNewVenderName;
        }

        /// <summary>
        /// PO涉3证提醒报表
        /// </summary>
        /// <param name="forms"></param>
        private void SendMailApbno(JArray forms)
        {
            if (forms.Count > 0)
            {
                if ("9110,9210,9001".Contains(forms.First["BUKRS"].ToString()))
                {
                    HashSet<string> hsDeclitem = new HashSet<string>();
                    foreach (var dr in forms)
                    {
                        hsDeclitem.Add(dr["DECLITEM"].ToString());
                    }
                    string declitem = string.Join("','", hsDeclitem);

                    #region 拼接Apbno栏位
                    IEnumerable<EntityClass3> Apbnos = _tdsAupoRepository.GetAbpno(forms.First["BUKRS"].ToString(), declitem);
                    JArray dtApbno = JArray.FromObject(Apbnos);

                    var query =
                                from form in forms
                                join drApbno in dtApbno
                                on form["DECLITEM"].ToString() equals drApbno["value2"].ToString()
                                where form["LIFNR"].ToString().Equals("TNC_MRO") && drApbno["value3"].ToString().Equals("3")
                                select new { 
                                    LIFNR = form["LIFNR"], 
                                    EBELN = form["EBELN"],
                                    EBELP = form["EBELP"],
                                    DECLITEM = form["DECLITEM"],
                                    TAX_CODE = form["TAX_CODE"],
                                    SMAKTX = form["SMAKTX"],
                                    APBNO = drApbno["value3"]
                                };
                    //select form.Concat(drApbno.Skip(2));
                    #endregion

                    if (query.Count() > 0)
                    {
                        JArray body = new JArray();
                        foreach (var result in query)
                        {
                            JObject jObject = new JObject();
                            jObject["Vendor code"] = result.LIFNR;
                            jObject["Po No"] = result.EBELN;
                            jObject["Po Item"] = result.EBELP;
                            jObject["DECLITEM"] = result.DECLITEM;
                            jObject["TAX_CODE"] = result.TAX_CODE;
                            jObject["SMAKTX"] = result.SMAKTX;
                            jObject["RULE"] = result.APBNO;
                            body.Add(jObject);
                        }
                        var mail = _tdsAupoRepository.GetCtrl("QSBN_MainPOApbno", "01");
                        string mailTo = mail.drpValue.ToString();
                        string mailCc = GetUser() + "," + mail.drpText.ToString();
                        string mailBody = JSONUtil.JArray2HtmlTable(JArray.FromObject(query));
                        //Mail.SendMail(mailBody, mailTo, mailCc, "A7100040", "PO涉两用物项和技术出口许可证（3证）提醒");
                        Mail.SendMail(mailBody, "A7100040", "A7100040", "A7100040", "PO涉两用物项和技术出口许可证（3证）提醒");

                    }
                }
            }

        }

    }
}