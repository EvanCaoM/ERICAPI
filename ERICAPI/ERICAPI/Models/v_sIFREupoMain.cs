using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ERICAPI.Models
{
    [Table("v_sIFREupoMain")]
    public class v_sIFREupoMain
    {
        public string MANDT { get; set; }
        [Key, Column(Order = 1, TypeName = "VARCHAR(4)")]
        public string BUKRS { get; set; }
        [Key, Column(Order = 2, TypeName ="VARCHAR(15)")]
        public string EBELN { get; set; }
        [Key, Column(Order = 3, TypeName = "VARCHAR(5)")]
        public string EBELP { get; set; }
        public string TXZ01 { get; set; }
        public decimal MENGE { get; set; }
        public decimal PEINH { get; set; }
        public string MEINS { get; set; }
        public decimal NETPR { get; set; }
        public decimal NETWR { get; set; }
        public string ELIKZ { get; set; }
        public string ANLN1 { get; set; }
        public string DECLITEM { get; set; }
        public string TAX_CODE { get; set; }
        public string matnrE { get; set; }
        public string MATNR { get; set; }
        public string SMAKTX { get; set; }
        public string ZGEWEI { get; set; }
        public string RETRC { get; set; }
        public string WAERS { get; set; }
        public string APFLAG { get; set; }
        public string CGEWEI { get; set; }
        public string BRGEW { get; set; }
        public decimal APPQTY { get; set; }
        public decimal DECLQTY { get; set; }
        public string LIFNR { get; set; }
        public string vendorname { get; set; }
        public string vendortype { get; set; }
        public string Agent { get; set; }
        public string Agent1 { get; set; }
        public string CHType { get; set; }
        public string CHNAME { get; set; }
        public System.DateTime CHDATE { get; set; }
        public string STATUS { get; set; }
        public string MARK { get; set; }
        public string REMARK { get; set; }
        public string CUSRMK { get; set; }
        public string CELFLAG { get; set; }
        public string ERNAM { get; set; }
        public string purchdate { get; set; }
        public string Prdetyp { get; set; }
        public string IsNOEric { get; set; }
        public string MailContent { get; set; }
        public string INCO1 { get; set; }
        public string BOITYP { get; set; }
    }

}
