using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayoneerWindowsService.DAO
{
   public class PayeeFormateResponse
    {
        public Result result { get; set; }
    }

    public class Fields
    {
        public List<Item> items { get; set; }
    }

    public class Item
    {
        public string field_name { get; set; }
        public int field_id { get; set; }
        public string field_type { get; set; }
        public string label { get; set; }
        public string description { get; set; }
        public string watermark { get; set; }
        public bool required { get; set; }
        public int min_length { get; set; }
        public int max_length { get; set; }
        public string regex { get; set; }
        public bool options_contains_other { get; set; }
        public bool dependent_options_contains_others { get; set; }
    }

    public class PayoutMethod
    {
        public string type { get; set; }
        public string country { get; set; }
        public string currency { get; set; }
        public string bank_account_type { get; set; }
        public Fields fields { get; set; }
    }

    public class Result
    {
        public PayoutMethod payout_method { get; set; }
    }
}
