using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayoneerWindowsService.DAO
{
    public class payeeStatusResponse
    {
        public StatusResult result { get; set; }
    }
    public class PayoutStatusMethod
    {
        public string type { get; set; }
        public string currency { get; set; }
    }

    public class StatusResult
    {
        public string account_id { get; set; }
        public Status status { get; set; }
        public string registration_date { get; set; }
        public PayoutStatusMethod payout_method { get; set; }
    }
    

    public class Status
    {
        public int type { get; set; }
        public string description { get; set; }
    }
}
