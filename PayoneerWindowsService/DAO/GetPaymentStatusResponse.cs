using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayoneerWindowsService.DAO
{
    public class GetPaymentStatusResponse
    {
        public Status_Result result { get; set; }
    }

    public class Status_Result
    {
        public DateTime payout_date { get; set; }
        public double amount { get; set; }
        public string currency { get; set; }
        public string status { get; set; }
        public double target_amount { get; set; }
        public string target_currency { get; set; }
        public string payee_id { get; set; }
        public string payout_id { get; set; }
        public DateTime scheduled_payout_date { get; set; }
        public DateTime load_date { get; set; }
    }

}
