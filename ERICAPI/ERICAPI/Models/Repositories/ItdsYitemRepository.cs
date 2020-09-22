using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ERICAPI.Models.Repositories
{
    /// <summary>
    /// 项号Repository接口
    /// </summary>
    public interface ItdsYitemRepository
    {
        /// <summary>
        /// 查询项号
        /// </summary>
        /// <returns></returns>
        IEnumerable<v_sIFRDeclitem> GetDeclitems(string bukrs, string declitem, string accno = null);

        bool Is3C(string bukrs, string declitem);

        bool IsEnergy(string bukrs, string declitem);

        string IsMonitorRule(string bukrs, string declitem);
    }

}
