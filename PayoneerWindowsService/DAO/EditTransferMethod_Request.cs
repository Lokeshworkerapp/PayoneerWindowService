using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayoneerWindowsService.DAO
{
    public class EditTransferMethod_Request
    {
        public string type { get; set; }
        public string bank_account_type { get; set; }
        public string country { get; set; }
        public string currency { get; set; }
        public List<EditTransfer_BankFieldDetail> bank_field_details { get; set; }
    }
    public class EditTransfer_BankFieldDetail
    {
        public string name { get; set; }
        public string value { get; set; }
    }

}
