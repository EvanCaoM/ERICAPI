using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ERICAPI.Models.Repositories
{
    public interface IZL11Repository
    {
        IEnumerable<VIEW_spare_All> Query(JObject form);

        bool CheckDiff(string bukrs, string matnr, string smaktx, string tax_code, string cgewei, string declitem);

        DropdownList GetCtrl(string sysid, string clrid);

        bool InsertMro(VIEW_spare_All zl11);

        bool InsertCmro(VIEW_spare_All zl11);

        bool InsertMroNew(VIEW_spare_All zl11);

        bool LockClick(string bukrs, string matnr, string vendorcode, string type, string lockId);

        int Delete(string type, string mandt, string bukrs, string matnr, string vendorcode, string declitem, string delName, string delReason);
    }
}
