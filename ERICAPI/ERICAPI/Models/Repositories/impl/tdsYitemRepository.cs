using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ERICAPI.Models.Repositories.impl
{
    /// <summary>
    /// ItdsYitemRepository实现类
    /// </summary>
    public class tdsYitemRepository : ItdsYitemRepository
    {
        private readonly AppDbContext _context;

        public tdsYitemRepository(AppDbContext appDbContext)
        {
            _context = appDbContext;
        }

        /// <summary>
        /// 查询非放弃退税底账序号
        /// </summary>
        /// <param name="bukrs"></param>
        /// <param name="declitem"></param>
        /// <param name="accno"></param>
        /// <returns></returns>
        public IEnumerable<v_sIFRDeclitem> GetDeclitems(string bukrs, string declitem, string accno = null)
        {
            var result = _context.v_sIFRDeclitem.Where(x => x.BUKRS.Equals(bukrs) && x.DFLAG != "X");
            if (IsNum(declitem))
                result = result.Where(x => x.DECLITEM.Equals(declitem)).Union(result.Where(x => x.TAX_CODE.Contains(declitem)));
            else
                result = result.Where(x => x.SMAKTX.Contains(declitem));
            if (!string.IsNullOrEmpty(accno))
                result = result.Where(x => x.ACCNO.Equals(accno));
            return result.ToList();
        }

        /// <summary>
        /// 放弃退税底账序号
        /// </summary>
        /// <param name="bukrs"></param>
        /// <param name="declitem"></param>
        /// <param name="accno"></param>
        /// <returns></returns>
        public IEnumerable<v_sIFRDeclitem> GetDeclitemsDraw(string bukrs, string declitem, string accno = null)
        {
            string sql = string.Empty;
            if(IsNum(declitem))
                sql = "select DECLITEM,MATNR,TAX_CODE,SMAKTX,ZGEWEI,CGEWEI,DFLAG,BUKRS,CLASS,RETRC,BRGEW,SEQNO,'' AS ACCNO,'' AS CELFLG,'' AS APBNO from tdsYitem_DrawBack where bukrs = @bukrs and DECLITEM = @declitem and (DFLAG <> 'X' or DFLAG IS NULL)";
            else
                sql = "select DECLITEM,MATNR,TAX_CODE,SMAKTX,ZGEWEI,CGEWEI,DFLAG,BUKRS,CLASS,RETRC,BRGEW,SEQNO,'' AS ACCNO,'' AS CELFLG,'' AS APBNO from tdsYitem_DrawBack where bukrs = @bukrs and SMAKTX like N'%" + declitem + "%' and (DFLAG <> 'X' or DFLAG IS NULL)";
            return _context.v_sIFRDeclitem.FromSqlRaw(sql, 
                new[] 
                { 
                    new SqlParameter("@bukrs", bukrs),
                    new SqlParameter("@declitem", declitem)
                });
        }

        /// <summary>
        /// 3C判断
        /// </summary>
        /// <param name="bukrs"></param>
        /// <param name="declitem"></param>
        /// <returns></returns>
        public bool Is3C(string bukrs, string declitem)
        {
            return _context.v_sIFRDeclitem.Where(x => x.BUKRS == bukrs && x.DECLITEM == declitem)
                .FirstOrDefault().SEQNO.Equals("3C");
        }

        /// <summary>
        /// 能源标识
        /// </summary>
        /// <param name="bukrs"></param>
        /// <param name="declitem"></param>
        /// <returns></returns>
        public bool IsEnergy(string bukrs, string declitem)
        {
            return _context.v_sIFRDeclitem.Where(x => x.BUKRS == bukrs && x.DECLITEM == declitem)
                .FirstOrDefault().CELFLG.Equals("CEL");
        }

        public string IsMonitorRule(string bukrs, string declitem)
        {
            var apbno = _context.v_sIFRDeclitem.Where(x => x.BUKRS == bukrs && x.DECLITEM == declitem)
                .FirstOrDefault().APBNO;
            string strflag = string.Empty;
            if (!string.IsNullOrEmpty(apbno))
            {
                if (apbno.ToString().IndexOf("3") != -1)
                {
                    strflag = "3";
                }
                if (apbno.ToString().IndexOf("4") != -1)
                {
                    strflag = "4";
                }
                if (apbno.ToString().IndexOf("8") != -1)
                {
                    strflag = "8";
                }
                if (apbno.ToString().IndexOf("G") != -1)
                {
                    strflag = "G";
                }
                if (apbno.ToString().IndexOf("a") != -1)
                {
                    strflag = "a";
                }
            }
            return strflag;
        }

        public bool IsNum(String str)
        {
            for (int i = 0; i < str.Length; i++)
            {
                if ((str[i] < '0' || str[i] > '9') && str[i] != '.')
                    return false;
            }
            if (str.Length == 0)
                return false;
            return true;
        }
    }
}
