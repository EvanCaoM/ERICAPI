using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
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
        public IEnumerable<VIEW_spare_All> Query(Dictionary<string, string> form)
        {
            var data = from a in _context.VIEW_spare_All
                       select a;

            //使用表达式树动态生成查询条件
            ParameterExpression c = Expression.Parameter(typeof(VIEW_spare_All), "c");
            Expression condition = Expression.Constant(true);
            foreach (var formPair in form)
            {
                if (!string.IsNullOrEmpty(formPair.Value))
                {

                    if (!formPair.Key.Contains("Date"))
                    {
                        Expression con = Expression.Call(
                                Expression.Property(c, typeof(VIEW_spare_All).GetProperty(formPair.Key)),
                                typeof(string).GetMethod("Equals", new Type[] { typeof(string) }),
                                Expression.Constant(formPair.Value));
                        condition = Expression.And(con, condition);
                        Expression<Func<VIEW_spare_All, bool>> end =
                            Expression.Lambda<Func<VIEW_spare_All, bool>>(condition, new ParameterExpression[] { c });
                        data = data.Where(end);
                    }
                    else
                    {
                        //日期单独处理
                        switch (formPair.Key.Replace("From", "").Replace("To", ""))
                        {
                            case "purchdate":
                                data = formPair.Key.EndsWith("From") ?
                                    data.Where(x => String.Compare(x.CHDATE, formPair.Value) >= 0) :
                                    data.Where(x => String.Compare(x.CHDATE, formPair.Value) <= 0);
                                break;
                        }
                    }
                }
            }
            return data;
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

    }
}
