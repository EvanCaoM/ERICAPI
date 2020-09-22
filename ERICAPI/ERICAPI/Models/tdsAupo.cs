using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ERICAPI.Models
{
    [Table("tdsAupo")]
    public class tdsAupo
    {
        [Key, Column(Order = 1)]
        public string MANDT { get; set; }
        [Key, Column(Order = 2)]
        public string BUKRS { get; set; }
        [Key, Column(Order = 3)]
        public string EBELN { get; set; }
        [Key, Column(Order = 4)]
        public string EBELP { get; set; }
        public string DECLITEM { get; set; }
        public string MATNR { get; set; }
        public string E_I { get; set; }
        public string TAX_CODE { get; set; }
        public string ZGEWEI { get; set; }
        public string SMAKTX { get; set; }
        public decimal APPQTY { get; set; }
        public decimal DECLQTY { get; set; }
        public string APFLAG { get; set; }
        public string STATUS { get; set; }
        public string MARK { get; set; }
        public string REMARK { get; set; }
        public string CHNAME { get; set; }
        public DateTime CHDATE { get; set; }
        public string ZHENGSHUI { get; set; }
        public string HOMEORABROAD { get; set; }
        public string CELFLAG { get; set; }
        public string ACCNO { get; set; }
    }
}
