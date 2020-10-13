using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;

namespace ERICAPI.Models.Repositories.impl
{
    public class ZL11Repository : IZL11Repository
    {
        private readonly AppDbContext _context;

        public ZL11Repository(AppDbContext appDbContext)
        {
            _context = appDbContext;
        }

        /// <summary>
        /// 利用动态表达树动态查询，有点慢
        /// </summary>
        /// <returns></returns>
        public IEnumerable<VIEW_spare_All> Query(JObject form)
        {
            var result = _context.VIEW_spare_All.Select(x => x);
            result = (form["BUKRS"].ToString().Equals("Company") || string.IsNullOrEmpty(form["BUKRS"].ToString())) ? result : result.Where(x => x.BUKRS.Equals(form["BUKRS"].ToString()));
            result = string.IsNullOrEmpty(form["MATNR"].ToString()) ? result : result.Where(x => x.MATNR.Equals(form["MATNR"].ToString()));
            result = string.IsNullOrEmpty(form["DECLITEM"].ToString()) ? result : result.Where(x => x.DECLITEM.Equals(form["DECLITEM"].ToString()));
            result = string.IsNullOrEmpty(form["Vendorcode"].ToString()) ? result : result.Where(x => x.Vendorcode.Equals(form["Vendorcode"].ToString()));
            result = string.IsNullOrEmpty(form["TAX_CODE"].ToString()) ? result : result.Where(x => x.TAX_CODE.Equals(form["TAX_CODE"].ToString()));
            result = string.IsNullOrEmpty(form["IsToTww"].ToString()) ? result : result.Where(x => x.IsToTww.Equals(form["IsToTww"].ToString()));
            result = string.IsNullOrEmpty(form["DFLAG"].ToString()) ? result : (form["DFLAG"].ToString().Equals("Y") ? result.Where(x => x.DFLAG == "") : result.Where(x => x.DFLAG.Equals("X")));
            result = string.IsNullOrEmpty(form["IsRegiestered"].ToString()) ? result : (form["IsRegistered"].ToString().Equals("Y") ? result.Where(x => x.DECLITEM != "") : result.Where(x => x.DECLITEM == "" || x.DECLITEM == null));
            result = string.IsNullOrEmpty(form["SMAKTX"].ToString()) ? result : result.Where(x => x.SMAKTX.Contains(form["SMAKTX"].ToString()));
            result = string.IsNullOrEmpty(form["MatType"].ToString()) ? result : result.Where(x => x.MatType.Equals(form["MatType"].ToString()));
            result = string.IsNullOrEmpty(form["Type"].ToString()) ? result : result.Where(x => x.Type.Equals(form["Type"].ToString()));
            result = string.IsNullOrEmpty(form["DateFrom"].ToString()) ? result : result.Where(x => String.Compare(x.CRDATE, form["DateFrom"].ToString()) >= 0);
            result = string.IsNullOrEmpty(form["DateTo"].ToString()) ? result : result.Where(x => String.Compare(x.CRDATE, form["DateTo"].ToString()) <= 0);
            return result.OrderByDescending(x => x.CRDATE);

        }

        /// <summary>
        /// 插入旧料号数据
        /// </summary>
        /// <param name="zl11"></param>
        /// <returns></returns>
        public bool InsertMro(VIEW_spare_All zl11)
        {

            //厂区
            SqlParameter BUKRS = new SqlParameter("@BUKRS", SqlDbType.VarChar);
            BUKRS.Value = zl11.BUKRS;
            //料号
            SqlParameter MATNR = new SqlParameter("@MATNR", SqlDbType.VarChar);
            MATNR.Value = zl11.MATNR;
            //申请人名字
            SqlParameter CRNAME = new SqlParameter("@CRNAME", SqlDbType.VarChar);
            CRNAME.Value = zl11.CRNAME;
            //申请时间
            SqlParameter CRDATE = new SqlParameter("@CRDATE", SqlDbType.VarChar);
            CRDATE.Value = zl11.CRDATE;
            //中文描述
            SqlParameter MAKTX = new SqlParameter("@MAKTX", SqlDbType.NVarChar);
            MAKTX.Value = zl11.MAKTX;
            //Declitem
            SqlParameter DECLITEM = new SqlParameter("@DECLITEM", SqlDbType.VarChar);
            DECLITEM.Value = zl11.DECLITEM;
            //修改人工号
            SqlParameter CHNAME = new SqlParameter("@CHNAME", SqlDbType.VarChar);
            CHNAME.Value = zl11.CHNAME;
            //修改时间
            SqlParameter CHDATE = new SqlParameter("@CHDATE", SqlDbType.VarChar);
            CHDATE.Value = zl11.CHDATE;

            SqlParameter C3FLAG = new SqlParameter("@C3FLAG", SqlDbType.NVarChar);
            C3FLAG.Value = zl11.C3FLAG;

            SqlParameter C3REMARK = new SqlParameter("@C3REMARK", SqlDbType.NVarChar);
            C3REMARK.Value = zl11.C3REMARK;

            SqlParameter CELFLAG = new SqlParameter("@CELFLAG", SqlDbType.NVarChar);
            CELFLAG.Value = zl11.CELFLAG;

            SqlParameter[] parameters = new SqlParameter[] { BUKRS, MATNR, CRNAME, CRDATE, MAKTX, DECLITEM, CHNAME, CHDATE, C3FLAG, C3REMARK, CELFLAG };

            try
            {
                _context.VIEW_spare_All.FromSqlRaw($"exec sp_sp_insertmro", parameters);
                return true;
            }
            catch(Exception e)
            {
                return false;
            }

        }

        /// <summary>
        /// 插入旧料号项号
        /// </summary>
        /// <param name="zl11"></param>
        /// <returns></returns>
        public bool InsertCmro(VIEW_spare_All zl11)
        {
            //抵账序号
            SqlParameter DECLITEM = new SqlParameter("@DECLITEM", SqlDbType.VarChar);
            DECLITEM.Value = zl11.DECLITEM;
            //料号对应抵账序号
            SqlParameter MA_MATNR = new SqlParameter("@MA_MATNR", SqlDbType.VarChar);
            MA_MATNR.Value = zl11.MA_MATNR;
            //数字化单位
            SqlParameter ZGEWEI = new SqlParameter("@ZGEWEI", SqlDbType.VarChar);
            ZGEWEI.Value = zl11.ZGEWEI;
            //汉化单位
            SqlParameter CGEWEI = new SqlParameter("@CGEWEI", SqlDbType.NVarChar);
            CGEWEI.Value = zl11.CGEWEI;
            //H.S.CODE
            SqlParameter TAX_CODE = new SqlParameter("@TAX_CODE", SqlDbType.VarChar);
            TAX_CODE.Value = zl11.TAX_CODE;
            //中文品名，关务需要
            SqlParameter SMAKTX = new SqlParameter("@SMAKTX", SqlDbType.NVarChar);
            SMAKTX.Value = zl11.SMAKTX;
            //征税
            SqlParameter BRGEW = new SqlParameter("@BRGEW", SqlDbType.NVarChar);
            BRGEW.Value = zl11.BRGEW;

            SqlParameter[] parameters = new SqlParameter[] { DECLITEM, MA_MATNR, ZGEWEI, CGEWEI, TAX_CODE, SMAKTX, BRGEW };

            try
            {
                _context.VIEW_spare_All.FromSqlRaw($"exec sp_sp_insertcmro", parameters);
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// 插入新料号项号
        /// </summary>
        /// <param name="zl11"></param>
        /// <returns></returns>
        public bool InsertMroNew(VIEW_spare_All zl11)
        {
            //厂区
            SqlParameter BUKRS = new SqlParameter("@BUKRS", SqlDbType.VarChar);
            BUKRS.Value = zl11.BUKRS;
            //料号
            SqlParameter MATNR = new SqlParameter("@MATNR", SqlDbType.VarChar);
            MATNR.Value = zl11.MATNR;
            //申请人名字
            SqlParameter CRNAME = new SqlParameter("@CRNAME", SqlDbType.VarChar);
            CRNAME.Value = zl11.CRNAME;
            //申请时间
            SqlParameter CRDATE = new SqlParameter("@CRDATE", SqlDbType.VarChar);
            CRDATE.Value = zl11.CRDATE;
            //中文描述
            SqlParameter MAKTX = new SqlParameter("@MAKTX", SqlDbType.NVarChar);
            MAKTX.Value = zl11.MAKTX;
            //Declitem
            SqlParameter DECLITEM = new SqlParameter("@DECLITEM", SqlDbType.VarChar);
            DECLITEM.Value = zl11.DECLITEM;
            //Vendorcode
            SqlParameter VENDORCODE = new SqlParameter("@VENDORCODE", SqlDbType.VarChar);
            VENDORCODE.Value = zl11.Vendorcode;
            //修改人工号
            SqlParameter CHNAME = new SqlParameter("@CHNAME", SqlDbType.VarChar);
            CHNAME.Value = zl11.CHNAME;
            //修改时间
            SqlParameter CHDATE = new SqlParameter("@CHDATE", SqlDbType.VarChar);
            CHDATE.Value = zl11.CHDATE;

            SqlParameter C3FLAG = new SqlParameter("@C3FLAG", SqlDbType.NVarChar);
            C3FLAG.Value = zl11.C3FLAG;

            SqlParameter C3REMARK = new SqlParameter("@C3REMARK", SqlDbType.NVarChar);
            C3REMARK.Value = zl11.C3REMARK;

            SqlParameter CELFLAG = new SqlParameter("@CELFLAG", SqlDbType.NVarChar);
            CELFLAG.Value = zl11.CELFLAG;


            SqlParameter[] parameters = new SqlParameter[] { BUKRS, MATNR, CRNAME, CRDATE, MAKTX, DECLITEM, VENDORCODE, CHNAME, CHDATE, C3FLAG, C3REMARK, CELFLAG };
            
            try
            {
                _context.VIEW_spare_All.FromSqlRaw($"exec sp_sp_insertmro_New", parameters);
                return true;
            }
            catch (Exception e)
            {
                return false;
            }


        }

        /// <summary>
        /// 料号检查
        /// </summary>
        /// <param name="bukrs"></param>
        /// <param name="matnr"></param>
        /// <param name="smaktx"></param>
        /// <param name="tax_code"></param>
        /// <param name="cgewei"></param>
        /// <param name="declitem"></param>
        /// <returns></returns>
        public bool CheckDiff(string bukrs, string matnr, string smaktx, string tax_code, string cgewei, string declitem)
        {
            return _context.VIEW_spare_All.FromSqlRaw($"exec usp_eric_checkdiff2",
                new[]
                {
                    new SqlParameter("bukrs", bukrs),
                    new SqlParameter("matnr", matnr),
                    new SqlParameter("smaktx", smaktx),
                    new SqlParameter("tax_code", tax_code),
                    new SqlParameter("cgewei", cgewei),
                    new SqlParameter("declitem", declitem)
                }).Any();

        }


        /// <summary>
        /// 从tbsctrl中获取配置文件
        /// </summary>
        /// <param name="sysid"></param>
        /// <param name="clrid"></param>
        /// <returns></returns>
        public DropdownList GetCtrl(string sysid, string clrid)
        {
            var sql = "SELECT VCHR1 as drpValue,VCHR2 as drpText  FROM tbsCtrl where SYSID=@sysid AND CTLID=@clrid ";
            return _context.dropdownLists.FromSqlRaw(sql,
                new[]
                {
                    new SqlParameter("sysid", sysid),
                    new SqlParameter("clrid", clrid)
                }).FirstOrDefault();
        }

        /// <summary>
        /// 加解锁
        /// </summary>
        /// <param name="bukrs"></param>
        /// <param name="matnr"></param>
        /// <param name="vendorcode"></param>
        /// <param name="type"></param>
        /// <param name="lockId"></param>
        /// <returns></returns>
        public bool LockClick(string bukrs, string matnr, string vendorcode, string type, string lockId)
        {
            var sqlNew = "UPDATE tdsMatnr_New SET LOCK=@lockId where  bukrs=@bukrs and mro_matnr=@matnr and vendorcode=@vendorcode and lock<>@lockId";
            var sql = "UPDATE tdsMatnr_New SET LOCK=@lockId where  bukrs=@bukrs and mro_matnr=@matnr and vendorcode=@vendorcode and lock<>@lockId";
            var parameters = new SqlParameter[]
            {
                new SqlParameter("@bukrs", bukrs),
                new SqlParameter("@matnr", matnr),
                new SqlParameter("@vendorcode", vendorcode),
                new SqlParameter("@lockId", lockId)
            };
            try
            {
                if(!string.IsNullOrEmpty(type))
                    _context.VIEW_spare_All.FromSqlRaw(sqlNew, parameters);
                else
                    _context.VIEW_spare_All.FromSqlRaw(sql, parameters);
                return true;
            }
            catch (Exception e)
            {
                return false;
            }

        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="type"></param>
        /// <param name="mandt"></param>
        /// <param name="bukrs"></param>
        /// <param name="matnr"></param>
        /// <param name="vendorcode"></param>
        /// <param name="declitem"></param>
        /// <param name="delName"></param>
        /// <param name="delReason"></param>
        /// <returns></returns>
        public int Delete(string type, string mandt, string bukrs, string matnr, string vendorcode, string declitem, string delName, string delReason)
        {
            var sql1 = "Delete tdsymro where MANDT =@mandt AND BUKRS=@bukrs and  MRO_MATNR=@matnr and dflag='X';";
            var sql2 = " Update tdsYmro SET DFLAG='X',DELREASON=@delReason,DELNAME=@delName,DELDATE=GETDATE()  where MANDT =@mandt AND BUKRS=@bukrs and  MRO_MATNR=@matnr AND DECLITEM=@declitem ;";
            var sql3 = "UPDATE TDSYBMRO SET LOCK='N' WHERE BUKRS=@bukrs AND MATNR=@matnr  and vendorcode=@vendorcode ;";
            var sql4 = "Insert into tdsYmro (MANDT,BUKRS,MRO_MATNR,DECLITEM,CRNAME,CRDATE) VALUES (@mandt,@bukrs,@matnr,'', (SELECT top 1 CRNAME FROM tdsYmro WHERE MANDT=@mandt AND bukrs=@bukrs and mro_matnr=@matnr and dflag='X'), (SELECT top 1 CRDATE FROM tdsYmro WHERE MANDT=@mandt AND bukrs=@bukrs and mro_matnr=@matnr and dflag='X'))";

            var sqlNew1 = "Update tdsYmro_New SET DFLAG='X',DELREASON=@delReason,DELNAME=@delName,DELDATE=GETDATE() where MANDT =@mandt AND BUKRS=@bukrs and  MRO_MATNR=@matnr AND DECLITEM=@declitem and vendorcode=@vendorcode ;";
            var sqlNew2 = "UPDATE tdsMatnr_New SET LOCK='N' WHERE BUKRS=@bukrs AND MRO_MATNR=@matnr  and vendorcode=@vendorcode ;";
            var sqlNew3 = "Insert into tdsYmro_New (MANDT,BUKRS,MRO_MATNR,DECLITEM,vendorcode,CRNAME,CRDATE,MAKTX ) VALUES (@mandt,@bukrs,@matnr,'',@vendorcode,(SELECT top 1 CRNAME FROM tdsYmro_New WHERE MANDT=@mandt AND bukrs=@bukrs and mro_matnr=@matnr and dflag='X' and vendorcode=@vendorcode),(SELECT top 1 CRDATE FROM tdsYmro_New WHERE MANDT=@mandt AND bukrs=@bukrs and mro_matnr=@matnr and dflag='X' and vendorcode=@vendorcode),(SELECT top 1 MAKTX FROM tdsYmro_New WHERE MANDT=@mandt AND bukrs=@bukrs and mro_matnr=@matnr and dflag='X' and vendorcode=@vendorcode));";

            string sql;

            if (string.IsNullOrEmpty(type))
                sql = sql1 + sql2 + sql3 + sql4;
            else
                sql = sqlNew1 + sqlNew2 + sqlNew3;

            return _context.Database.ExecuteSqlRaw(sql,
                new[]
                {
                    new SqlParameter("@mandt", mandt),
                    new SqlParameter("@bukrs", bukrs),
                    new SqlParameter("@matnr", matnr),
                    new SqlParameter("@vendorcode", vendorcode),
                    new SqlParameter("@declitem", declitem),
                    new SqlParameter("@delName", delName),
                    new SqlParameter("@delReason", delReason)
                });

        }
    
   
    }
}
