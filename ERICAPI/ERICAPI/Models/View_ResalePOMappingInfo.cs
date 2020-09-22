using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ERICAPI.Models
{
    public class View_ResalePOMappingInfo
    {
        [Key, Column(Order = 1)]
        public string CompanyCode { get; set; }
        public string PONo { get; set; }
        public string POItem { get; set; }
        public string OldCompany { get; set; }
        public string OldAssetNo { get; set; }
        public string OldPONo { get; set; }
        public string OldPOItem { get; set; }
        public string OldECDocumentNo { get; set; }
    }
}
