using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ERICAPI.Models
{
    [Table("v_sIFRDeclitem")]
    public class v_sIFRDeclitem
    {
        [Key, Column(Order = 3)]
        public string DECLITEM { get; set; }
        public string MATNR { get; set; }
        public string TAX_CODE { get; set; }
        public string SMAKTX { get; set; }
        public string ZGEWEI { get; set; }
        public string CGEWEI { get; set; }
        public string DFLAG { get; set; }
        [Key, Column(Order = 1)]
        public string BUKRS { get; set; }
        public string CLASS { get; set; }
        public string RETRC { get; set; }
        public string BRGEW { get; set; }
        public string SEQNO { get; set; }
        [Key, Column(Order = 2)]
        public string ACCNO { get; set; }

        public string CELFLG { get; set; }

        public string APBNO { get; set; }
    }
}
