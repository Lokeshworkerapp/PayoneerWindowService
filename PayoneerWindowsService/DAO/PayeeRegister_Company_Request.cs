using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayoneerWindowsService.DAO
{
  public  class PayeeRegister_Company_Request
    {
        public string payee_id { get; set; }
        public Company_Payee payee { get; set; }
        public Company_Payout_Method payout_method { get; set; }
    }
    public class Company_Address
    {
        public string address_line_1 { get; set; }
        public string address_line_2 { get; set; }
        public string city { get; set; }
        public string state { get; set; }
        public string country { get; set; }
        public string zip_code { get; set; }
    }

    public class Company_BankFieldDetail
    {
        public string name { get; set; }
        public string value { get; set; }
    }

    public class Company_Contact
    {
        public string last_name { get; set; }
        public string first_name { get; set; }
        public string date_of_birth { get; set; }
    }

    public class Company_Payee
    {
        public string type { get; set; }
        public Company_Contact contact { get; set; }
        public Company_Address address { get; set; }
        public Company company { get; set; }
    }

    public class Company_Payout_Method
    {
        public string type { get; set; }
        public string bank_account_type { get; set; }
        public string country { get; set; }
        public string currency { get; set; }
        public List<Company_BankFieldDetail> bank_field_details { get; set; }
    }
    public class Company
    {
        public string incorporated_address_1 { get; set; }
        public string incorporated_city { get; set; }
        public string incorporated_country { get; set; }
        public string legal_type { get; set; }
        public string name { get; set; }       
    }
}
