using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace rtdc_rest.api.Models
{
    public class CreateRetailerReqJson
    {
        public string dataSourceCode { get; set; }
        public string retailerCode { get; set; }
        public long retailerRefId { get; set; }
        public string channelCode { get; set; }
        public string title { get; set; }
        public string email { get; set; }
        public string Phone { get; set; }
        public string taxOffice { get; set; }
        public long taxNumber { get; set; }
        public string contactName { get; set; }
        public string country { get; set; }
        public string city { get; set; }
        public string district { get; set; }
        public string address { get; set; }
        public int zipCode { get; set; }
    }
}
