using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayoneerWindowsService.DAO
{
    public class FundTransfer_Request
    {
        public string target_partner_id { get; set; }
        public string amount { get; set; }
        public string description { get; set; }

    }
}
