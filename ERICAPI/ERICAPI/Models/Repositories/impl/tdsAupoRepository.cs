using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace ERICAPI.Models.Repositories.impl
{
    /// <summary>
    /// ItdsAupoRepository实现类
    /// </summary>
    public class tdsAupoRepository : ItdsAupoRepository
    {
        private readonly AppDbContext _context;
        private readonly PAKSDbContext _paksContext;
        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="appDbContext"></param>
        /// <param name="paksDbContext"></param>
        public tdsAupoRepository(AppDbContext appDbContext, PAKSDbContext paksDbContext)
        {
            _context = appDbContext;
            _paksContext = paksDbContext;
        }

        /// <summary>
        /// 单条添加
        /// </summary>
        /// <param name="tdsAupo"></param>
        public void AddTdsAupo(tdsAupo tdsAupo)
        {
            RemoveTdsAupo(tdsAupo.BUKRS, tdsAupo.EBELN, new string[] { tdsAupo.EBELP });
            _context.tdsAupos.Add(tdsAupo);
            _context.SaveChanges();
        }

        /// <summary>
        /// 批量添加
        /// </summary>
        /// <param name="tdsAupos"></param>
        public bool AddTdsAupos (IEnumerable<tdsAupo> tdsAupos)
        {
            foreach (var tdsAupo in tdsAupos)
            {
                RemoveTdsAupo(tdsAupo.BUKRS, tdsAupo.EBELN, new string[] { tdsAupo.EBELP });
            }
            try
            {
                _context.tdsAupos.AddRange(tdsAupos);
                _context.SaveChanges();
                return true;
            }
            catch(SqlException e)
            {
                throw e;
            }
        }


        /// <summary>
        /// 异步批量添加
        /// </summary>
        /// <param name="tdsAupos"></param>
        /// <returns></returns>
        public async Task AddTdsAuposTask(IList<tdsAupo> tdsAupos)
        {

            await Task.Run(() => {
                foreach (var tdsAupo in tdsAupos)
                {
                    RemoveTdsAupo(tdsAupo.BUKRS, tdsAupo.EBELN, new string[] { tdsAupo.EBELP });
                }
                _context.tdsAupos.AddRange(tdsAupos);
                _context.SaveChanges();
            });
        }

        /// <summary>
        /// 根据ebelp批量删除
        /// </summary>
        /// <param name="bukrs"></param>
        /// <param name="ebeln"></param>
        /// <param name="ebelp"></param>
        public void RemoveTdsAupo(string bukrs, string ebeln, string[] ebelp)
        {
            IEnumerable<tdsAupo> tdsAupos = new List<tdsAupo>();
            try
            {
                tdsAupos = GetTdsAupos(bukrs, ebeln, ebelp);
                _context.tdsAupos.RemoveRange(tdsAupos);
                _context.SaveChanges();
            }
            catch (SqlException e)
            {
                throw e;
            }
        }

        /// <summary>
        /// 单条删除
        /// </summary>
        /// <param name="bukrs"></param>
        /// <param name="ebeln"></param>
        /// <param name="ebelp"></param>
        public bool RemoveTdsAupo(string bukrs, string ebeln, string ebelp)
        {
            tdsAupo tdsAupo = new tdsAupo();
            try
            {
                tdsAupo = GetTdsAupo(bukrs, ebeln, ebelp);
                _context.tdsAupos.Remove(tdsAupo);
                _context.SaveChanges();
                return true;
            }
            catch (SqlException e)
            {
                return false;
            }
        }

        /// <summary>
        /// 批量删除
        /// </summary>
        /// <param name="tdsAupos"></param>
        /// <returns></returns>
        public bool RemoveTdsAupo(IEnumerable<tdsAupo> tdsAupos)
        {
            try
            {
                _context.tdsAupos.RemoveRange(tdsAupos);
                _context.SaveChanges();
                return true;
            }
            catch (SqlException e)
            {
                return false;
            }
        }

        /// <summary>
        /// 查询
        /// </summary>
        /// <param name="form"></param>
        /// <param name="purchdateFrom"></param>
        /// <param name="purchdateTo"></param>
        /// <returns></returns>
        public IEnumerable<v_sIFREupoMain> GetMainpo(v_sIFREupoMain form, string purchdateFrom = null, string purchdateTo = null)
        {

            var data = _context.v_sIFREupoMains;
            var result = form.BUKRS.Equals("Company") ? data : data.Where(x => x.BUKRS == form.BUKRS);
            result = string.IsNullOrEmpty(form.BOITYP) ? result : result.Where(x => x.BOITYP == form.BOITYP);
            result = string.IsNullOrEmpty(form.EBELN) ? result : result.Where(x => x.EBELN == form.EBELN);
            result = string.IsNullOrEmpty(form.matnrE) ? result : result.Where(x => x.matnrE == form.matnrE);
            result = string.IsNullOrEmpty(form.TXZ01) ? result : result.Where(x => x.TXZ01.Contains(form.TXZ01));
            result = string.IsNullOrEmpty(form.DECLITEM) ? result : result.Where(x => x.DECLITEM == form.DECLITEM);
            result = string.IsNullOrEmpty(form.STATUS) ? result : result.Where(x => x.STATUS == form.STATUS);
            result = string.IsNullOrEmpty(form.LIFNR) ? result : result.Where(x => x.LIFNR == form.LIFNR);
            result = string.IsNullOrEmpty(form.vendortype) ? result : result.Where(x => x.vendortype == form.vendortype);
            result = string.IsNullOrEmpty(form.CHNAME) ? result : result.Where(x => x.CHNAME == form.CHNAME);
            result = string.IsNullOrEmpty(form.BRGEW) ? result : result.Where(x => x.BRGEW == form.BRGEW);
            result = string.IsNullOrEmpty(form.CHType) ? result : result.Where(x => x.CHType == form.CHType);
            result = string.IsNullOrEmpty(form.MARK) ? result : result.Where(x => x.MARK == form.MARK);
            result = string.IsNullOrEmpty(purchdateFrom) ? result : result.Where(x => String.Compare(x.purchdate, purchdateFrom) >= 0);
            result = string.IsNullOrEmpty(purchdateTo) ? result : result.Where(x => String.Compare(x.purchdate, purchdateTo) <= 0);
            return result.OrderBy(x => x.EBELN).ThenBy(x => x.EBELP);
        }

        /// <summary>
        /// 利用动态表达树动态查询，有点慢
        /// </summary>
        /// <returns></returns>
        public IEnumerable<v_sIFREupoMain> GetMainpo(Dictionary<string, string> form)
        {
            var data = from a in _context.v_sIFREupoMains
                       select a;

            //使用表达式树动态生成查询条件
            ParameterExpression c = Expression.Parameter(typeof(v_sIFREupoMain), "c");
            Expression condition = Expression.Constant(true);
            foreach (var formPair in form)
            {
                if (!string.IsNullOrEmpty(formPair.Value))
                {

                    if (!formPair.Key.Contains("Date"))
                    {
                        Expression con = Expression.Call(
                                Expression.Property(c, typeof(v_sIFREupoMain).GetProperty(formPair.Key)),
                                typeof(string).GetMethod("Equals", new Type[] { typeof(string) }),
                                Expression.Constant(formPair.Value));
                        condition = Expression.And(con, condition);
                        Expression<Func<v_sIFREupoMain, bool>> end =
                            Expression.Lambda<Func<v_sIFREupoMain, bool>>(condition, new ParameterExpression[] { c });
                        data = data.Where(end);
                    }
                    else
                    {
                        //日期单独处理
                        switch (formPair.Key.Replace("From", "").Replace("To", ""))
                        {
                            case "purchdate":
                                data = formPair.Key.EndsWith("From") ?
                                    data.Where(x => String.Compare(x.purchdate, formPair.Value) >= 0) :
                                    data.Where(x => String.Compare(x.purchdate, formPair.Value) <= 0);
                                break;
                        }
                    }
                }
            }
            return data;
        }

        /// <summary>
        /// 单条查询tdsAupo
        /// </summary>
        /// <param name="bukrs"></param>
        /// <param name="ebeln"></param>
        /// <param name="ebelp"></param>
        /// <returns></returns>
        public tdsAupo GetTdsAupo(string bukrs, string ebeln, string ebelp)
        {
            return _context.tdsAupos.Where(x => x.BUKRS == bukrs && x.EBELN == ebeln && x.EBELP == ebelp).FirstOrDefault();
        }

        /// <summary>
        /// 根据ebelp批量查询
        /// </summary>
        /// <param name="bukrs"></param>
        /// <param name="ebeln"></param>
        /// <param name="ebelp"></param>
        /// <returns></returns>
        public IEnumerable<tdsAupo> GetTdsAupos(string bukrs, string ebeln, string[] ebelp)
        {
            return _context.tdsAupos.Where(x => x.BUKRS == bukrs && x.EBELN == ebeln && ebelp.Contains(x.EBELP)).ToList();
        }

        /// <summary>
        /// 批量查询
        /// </summary>
        /// <returns></returns>
        public IEnumerable<tdsAupo> GetTdsAupos()
        {
            return _context.tdsAupos;
        }

        /// <summary>
        /// 获取公司别
        /// </summary>
        /// <returns></returns>
        public IEnumerable<DropdownList> GetComcode(string user)
        {
            //前端工号无法获取，默认全部
            user = "A7100040";
            return _context.dropdownLists.FromSqlRaw($"sp_tbsGetComcdByUserShebei {user}");
        }

        /// <summary>
        /// 查询vendorname，偷懒使用DropdownList实例
        /// </summary>
        /// <param name="vendorcode"></param>
        /// <returns></returns>
        public string GetVendorname(string vendorcode)
        {
            return _context.dropdownLists.FromSqlRaw($"SELECT TOP 1 name1 as drpValue,'' as drpText FROM tdsLFA1  WHERE MANDT  in( '218','228','299') AND  LIFNR = @lifnr"
                ,new[] {new SqlParameter("lifnr", vendorcode)}).FirstOrDefault().drpValue;
        }

        public string GetTcurr(string bukrs, string fcurr)
        {
            return _context.dropdownLists.FromSqlRaw($"select top 1 CAST(ukurs AS VARCHAR(10)) as drpValue,'' as drpText from tbstcurr where bukrs = @bukrs and fcurr = @fcurr order by gdatu desc",
                new[]
                {
                    new SqlParameter("bukrs", bukrs),
                    new SqlParameter("fcurr", fcurr)
                }).FirstOrDefault().drpValue;
        }

        /// <summary>
        /// 单价判定，不能高于最高价，不能低于最低价
        /// 感觉和这个页面没关系，先不加
        /// </summary>
        /// <param name="bukrs"></param>
        /// <param name="declitem"></param>
        /// <returns></returns>
        public DropdownList GetPrice(string bukrs, string declitem)
        {
            return _context.dropdownLists.FromSqlRaw($"select MAX(UPRICE) AS drpValue, MIN(UPRICE) AS drpText FROM tdsPRICE WHERE BUKRS = @bukrs AND DECLITEM = @declitem and mandt in ('218', '228', '299') GROUP BY BUKRS,DECLITEM",
                new[]
                {
                    new SqlParameter("bukrs", bukrs),
                    new SqlParameter("declitem", declitem)
                }).FirstOrDefault();
            
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="bukrs"></param>
        /// <param name="matnr"></param>
        /// <param name="vendorcode"></param>
        /// <returns></returns>
        public bool IsExistZl11(string bukrs, string matnr, string vendorcode)
        {
            return _context.dropdownLists.FromSqlRaw($"SELECT  '' as drpValue,'' as drpText  FROM view_spare_all  where matnr=@matnr and bukrs=@bukrs AND DFLAG ='' and declitem !='' and vendorcode=@vendorcode ",
                new[]
                {
                    new SqlParameter("bukrs", bukrs),
                    new SqlParameter("vendorcode", vendorcode),
                    new SqlParameter("matnr", matnr)
                }).Any();
        }

        /// <summary>
        /// 当有料号的po: 自主报关、代理报关、 在归类有料号的PO时，
        /// 系统校验公司别+料号是否存在不一致的底账序号，如果有，系统提示；如果没有，系统保存成功
        /// </summary>
        /// <param name="bukrs"></param>
        /// <param name="vendorcode"></param>
        /// <param name="matnr"></param>
        /// <param name="declitem"></param>
        /// <returns></returns>
        public bool SameDecMatnr(string bukrs, string vendorcode, string matnr, string declitem)
        {
            return _context.dropdownLists.FromSqlRaw($"SELECT  '' as drpValue,'' as drpText  FROM dbo.VIEW_spare_All WHERE BUKRS=@bukrs AND Vendorcode=@vendorcode AND MATNR=@matnr AND (DECLITEM=@declitem OR DECLITEM='') AND DFLAG<>'X' ",
                new[]
                {
                    new SqlParameter("bukrs", bukrs),
                    new SqlParameter("vendorcode", vendorcode),
                    new SqlParameter("declitem", declitem),
                    new SqlParameter("matnr", matnr)
                }).Any();
        }

        public bool SameDecDiffEbeln(string bukrs, string matnr, string declitem)
        {
            return _context.dropdownLists.FromSqlRaw($"SELECT  '' as drpValue,'' as drpText  FROM v_sIFREupoMain_Decl with(nolock) where Bukrs=@bukrs AND MATNRE = @matnr and declitem != @declitem",
                new[]
                {
                    new SqlParameter("bukrs", bukrs),
                    new SqlParameter("declitem", declitem),
                    new SqlParameter("matnr", matnr)
                }).Any();
        }

        public bool PoCheck(string bukrs, string lifnr, string txz01, string ebeln, string vendortype, string declitem)
        {
            txz01 = txz01.Replace("'", "''");
            return _context.dropdownLists.FromSqlRaw($"SELECT  '' as drpValue,'' as drpText  FROM v_SIFREupoMain  where Bukrs=@bukrs AND lifnr=@lifnr AND txz01=@txz01 and ebeln !=@ebeln  AND PURCHDATE>'20160101'  and CHType=N'已归并' and declitem!='9999' and isnull(declitem,'')!='' and declitem!=@declitem and vendortype<>N'放弃退税' ",
                new[]
                {
                    new SqlParameter("bukrs", bukrs),
                    new SqlParameter("lifnr", lifnr),
                    new SqlParameter("vendortype", vendortype),
                    new SqlParameter("declitem", declitem),
                    new SqlParameter("ebeln", ebeln),
                    new SqlParameter("txz01", txz01)
                }).Any();
        }

        public bool ECCheck(string bukrs, string ebeln, string ebelp)
        {
            return _context.dropdownLists.FromSqlRaw($"SELECT  '' as drpValue,'' as drpText  FROM tdeLIPS  where EBELN= @ebeln and EBELP=@ebelp and comcd=@bukrs AND [STATS]<>'X'",
                new[]
                {
                    new SqlParameter("bukrs", bukrs),
                    new SqlParameter("ebeln", ebeln),
                    new SqlParameter("ebelp", ebelp)
                }).Any();
        }

        /// <summary>
        /// 更新其他数据
        /// </summary>
        /// <param name="tdsAupo"></param>
        /// <param name="retrc"></param>
        /// <param name="itemno"></param>
        /// <returns></returns>
        public int UpdateOtherDatas(tdsAupo tdsAupo, string retrc, string itemno)
        {
            string sql1 = $"delete tdspoitem  where BUKRS = @bukrs and ebeln = @ebeln and ebelp = @ebelp and  mandt = @mandt;";
            string sql2 = $"insert into tdspoitem ([Mandt],bukrs,[Ebeln],[Ebelp],[Retrc],[Remark] ) values ( @mandt ,@bukrs,@ebeln',@ebelp,@retrc,'');";
            string sql3 = $"update tdsPOBasicInfoAupo set ITMENO =@itemno where mandt = @mandt and bukrs = @bukrs and ebeln = @ebeln and ebelp = @ebelp ;";
            string sql4 = $"update tdsuploadMsg set LOCK = 'Y' where bukrs = @bukrs and ebeln = @ebeln and ebelp = @ebelp ;";
            return _context.Database.ExecuteSqlRaw(sql1 + sql2 + sql3 + sql4,
                new[]
                {
                    new SqlParameter("bukrs", tdsAupo.BUKRS),
                    new SqlParameter("ebeln", tdsAupo.EBELN),
                    new SqlParameter("ebelp", tdsAupo.EBELP),
                    new SqlParameter("mandt", tdsAupo.MANDT),
                    new SqlParameter("retrc", retrc),
                    new SqlParameter("itemno", itemno),
                });
        }

        /// <summary>
        /// 原进口PO
        /// </summary>
        /// <param name="bukrs"></param>
        /// <param name="ebeln"></param>
        /// <param name="ebelp"></param>
        /// <returns></returns>
        public IEnumerable<View_ResalePOMappingInfo> GetPrePO(string bukrs, string ebeln, string ebelp)
        {
            return _paksContext.View_ResalePOMappingInfo.Where(x => x.CompanyCode == bukrs && x.PONo == ebeln && x.POItem == ebelp).ToList();
        }

        /// <summary>
        /// 查询pr单号
        /// </summary>
        /// <param name="bukrs"></param>
        /// <param name="ebeln"></param>
        /// <returns></returns>
        public string QueryPR(string bukrs, string ebeln)
        {
            
            return _context.dropdownLists.FromSqlRaw($"SELECT distinct wpoid as drpValue,'' as drpText  FROM tdsPoBasicInfo  where Bukrs=@bukrs and EBELN= @ebeln ",
                new[]
                {
                    new SqlParameter("bukrs", bukrs),
                    new SqlParameter("ebeln", ebeln)
                }).FirstOrDefault().drpValue;
        }

        /// <summary>
        /// 从tbsctrl中获取配置文件
        /// </summary>
        /// <param name="sysid"></param>
        /// <param name="clrid"></param>
        /// <returns></returns>
        public DropdownList GetCtrl(string sysid, string clrid)
        {
            return _context.dropdownLists.FromSqlRaw($"SELECT VCHR1 as drpValue,VCHR2 as drpText  FROM tbsCtrl where SYSID=@sysid AND CTLID=@clrid ",
                new[]
                {
                    new SqlParameter("sysid", sysid),
                    new SqlParameter("clrid", clrid)
                }).FirstOrDefault();
        }

    }
}
