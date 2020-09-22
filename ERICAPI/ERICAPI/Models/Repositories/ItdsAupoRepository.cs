using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ERICAPI.Models.Repositories
{
    public interface ItdsAupoRepository
    {
        IEnumerable<tdsAupo> GetTdsAupos();

        IEnumerable<v_sIFREupoMain> GetMainpo(Dictionary<string, string> form);

        IEnumerable<v_sIFREupoMain> GetMainpo(v_sIFREupoMain form, string purchdateFrom = null, string purchdateTo = null);

        void AddTdsAupo(tdsAupo tdsAupo);

        bool AddTdsAupos(IEnumerable<tdsAupo> tdsAupos);

        int UpdateOtherDatas(tdsAupo tdsAupo, string retrc, string itemno);

        Task AddTdsAuposTask(IList<tdsAupo> tdsAupos);

        tdsAupo GetTdsAupo(string bukrs, string ebeln, string ebelp);

        bool RemoveTdsAupo(IEnumerable<tdsAupo> tdsAupos);

        bool RemoveTdsAupo(string bukrs, string ebeln, string ebelp);

        IEnumerable<DropdownList> GetComcode(string user);

        string GetVendorname(string vendorcode);

        string GetTcurr(string bukrs, string fcurr);

        bool IsExistZl11(string bukrs, string matnr, string vendorcode);

        bool SameDecMatnr(string bukrs, string vendorcode, string matnr, string declitem);

        bool SameDecDiffEbeln(string bukrs, string matnr, string declitem);

        bool PoCheck(string bukrs, string lifnr, string txz01, string ebeln, string vendortype, string declitem);

        bool ECCheck(string bukrs, string ebeln, string ebelp);

        IEnumerable<View_ResalePOMappingInfo> GetPrePO(string bukrs, string ebeln, string ebelp);

        string QueryPR(string bukrs, string ebeln);

        DropdownList GetCtrl(string sysid, string clrid);
    }
}
