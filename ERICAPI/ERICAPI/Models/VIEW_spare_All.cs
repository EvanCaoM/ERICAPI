using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ERICAPI.Models
{
    [Table("VIEW_spare_All")]
    public class VIEW_spare_All
    {
        public string MATNR { get; set; }
        public string BUKRS { get; set; }
        public string WAERS { get; set; }
        public string Vendor { get; set; }
        public string Vendorcode { get; set; }
        public string IsToTww { get; set; }
        public string price { get; set; }
        public string CRNAME { get; set; }
        public string CRDATE { get; set; }
        public string SMAKTX_U { get; set; }
        public string DECLITEM { get; set; }
        public string MAKTX { get; set; }
        public string CHNAME { get; set; }
        public string CHDATE { get; set; }
        public string MA_MATNR { get; set; }
        public string ZGEWEI { get; set; }
        public string CGEWEI { get; set; }
        public string TAX_CODE { get; set; }
        public string SMAKTX { get; set; }
        public string BRGEW { get; set; }
        public string DFLAG { get; set; }
        public string DELNAME { get; set; }
        public string DELREASON { get; set; }
        public Nullable<System.DateTime> DELDATE { get; set; }
        public string C3FLAG { get; set; }
        public string C3REMARK { get; set; }
        public string CELFLAG { get; set; }
        public string MatType { get; set; }
        public string ID { get; set; }
        public string LOCK { get; set; }
        public string Type { get; set; }

    }
}
