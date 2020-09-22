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
        /// 查询
        /// </summary>
        /// <param name="bukrs"></param>
        /// <param name="declitem"></param>
        /// <param name="accno"></param>
        /// <returns></returns>
        public IEnumerable<v_sIFRDeclitem> GetDeclitems(string bukrs, string declitem, string accno = null)
        {
            var result = _context.v_sIFRDeclitem.Where(x => x.BUKRS.Equals(bukrs) && x.DECLITEM.Equals(declitem));
            if (!string.IsNullOrEmpty(accno))
                result = result.Where(x => x.ACCNO.Equals(accno));
            return result.ToList();
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
    }
}
