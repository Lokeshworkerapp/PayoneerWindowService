using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayoneerWindowsService.DAO
{
  public  class PayeeRegisterRequest
    {
        public string payee_id { get; set; }
        public Payee payee { get; set; }
        public Payout_Method payout_method { get; set; }
    }
    public class Address
    {
        public string address_line_1 { get; set; }
        public string address_line_2 { get; set; }
        public string city { get; set; }
        public string state { get; set; }
        public string country { get; set; }
        public string zip_code { get; set; }
    }

    public class BankFieldDetail
    {
        public string name { get; set; }
        public string value { get; set; }
    }

    public class Contact
    {
        public string last_name { get; set; }
        public string first_name { get; set; }
        public string date_of_birth { get; set; }
    }

    public class Payee
    {
        public string type { get; set; }
        public Contact contact { get; set; }
        public Address address { get; set; }
    }

    public class Payout_Method
    {
        public string type { get; set; }
        public string bank_account_type { get; set; }
        public string country { get; set; }
        public string currency { get; set; }
        public List<BankFieldDetail> bank_field_details { get; set; }
    }
}
