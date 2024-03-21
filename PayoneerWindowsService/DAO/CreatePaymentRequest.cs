using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayoneerWindowsService.DAO
{
    public class CreatePaymentRequest
    {
        public List<Payment> Payments { get; set; }
    }
    public class Payment
    {
        public string client_reference_id { get; set; }
        public string payee_id { get; set; }
        public string description { get; set; }
        public string currency { get; set; }
        public string amount { get; set; }
    }

}
