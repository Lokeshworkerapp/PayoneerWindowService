using Newtonsoft.Json;
using PayoneerWindowsService.BAL;
using PayoneerWindowsService.DAL;
using PayoneerWindowsService.DAO;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace PayoneerWindowsService
{
    public partial class Service1 : ServiceBase
    {
        public Service1()
        {
            InitializeComponent();
            //Payoneer_Login();
            Payonner_PayeeRegister();
        }

        public string Payoneer_Login()
        {
            string respToken = null;
            string Token = null;
            Authorization_Request _request = new Authorization_Request();
            Authorization_Response _responseObj = new Authorization_Response();
            try
            {

                string client_id = ConfigurationManager.AppSettings["client_id"];

                string client_secret = ConfigurationManager.AppSettings["client_secret"];


                string inputString = client_id +":"+ client_secret;

                byte[] _Token = Encoding.UTF8.GetBytes(inputString);

                respToken = Convert.ToBase64String(_Token);

                _request.client_id = client_id;
                _request.client_secret = client_secret;
                _request.grant_type = "client_credentials";
                _request.scope = "read write";

                 string result = _getLoginResponse.RestResponse("https://login.sandbox.payoneer.com/api/v2/oauth2/token", RestSharp.Method.POST, _request, respToken, "Payoneer_Login", "");

                 _responseObj = JsonConvert.DeserializeObject<Authorization_Response>(result);

                Token = _responseObj.access_token;
            }
            catch (Exception ex)
            {
                //=====LogWrite here====//
                LogWriter.Add("PayoneerWindows_ServiceLog", (ex.Message + " | " + DateTime.Now.ToString("ddMMyyyy")), "", "", 2);
            }

            return Token;
        }

        public string GetRegisterPayeeFormat(string Token, string Country, string Currency, string Payee_type, string Program_id)
        {
            string respToken = null;
            string ApiResult = null;
            string result = null;
            PayeeFormateRequest _request = new PayeeFormateRequest();
            PayeeFormateResponse _responseObj = new PayeeFormateResponse();
            try
            {    
                _request.country= Country;
                _request.currency = Currency;
                _request.payee_type = Payee_type;
                _request.program_id = Program_id;
                string country = _request.country;
                string currency = _request.currency;
                string payee_type = _request.payee_type;
                string program_id = _request.program_id;
                result = _getResponse.RestResponse("programs/" + program_id + "/payout-methods/bank/account-types/" + payee_type + "/countries/" + country + "/currencies/" + currency + "", RestSharp.Method.GET, "", Token, "Payoneer_RegisterPayeeFormat", "");

                //_responseObj = JsonConvert.DeserializeObject<PayeeFormateResponse>(result);
            }
            catch (Exception ex)
            {
                //=====LogWrite here====//
                LogWriter.Add("PayoneerWindows_ServiceLog", (ex.Message + " | " + DateTime.Now.ToString("ddMMyyyy")), "", "", 2);
            }

            return result;
        }



        public void Payonner_PayeeRegister()
        {
            string respToken = null;
            string Token = null;
            string apiResponsePayeeFormate = null;
            PayeeRegisterRequest _request = new PayeeRegisterRequest();
            PayeeFormateResponse _responseObj = new PayeeFormateResponse();
            try
            {
                if (Token == null)
                {
                    Token = Payoneer_Login();
                   
                }
                apiResponsePayeeFormate = GetRegisterPayeeFormat(Token, "IN", "USD", "COMPANY", "100224230");
                if (apiResponsePayeeFormate.Contains("payee_id"))
                {
                    _request.payee_id = "testWLIndividual_01";
                }
              
                _request.payee = new Payee
                {
                    type = "individual",
                    contact = new Contact
                    {
                        last_name = "John",
                        first_name = "Smith",
                        date_of_birth = "1999-06-06"
                    },
                    address = new Address
                    {
                        address_line_1 = "123 Main St.",
                        address_line_2 = "",
                        city = "New York",
                        state = "NY",
                        country = "US",
                      zip_code = "10001"
                    }
                };
                _request.payout_method = new Payout_Method
                {
                    type = "banktransfer",
                    bank_account_type = "personal",
                    country = "US",
                    currency = "USD",
                    bank_field_details = new List<BankFieldDetail>
                    {
                     new BankFieldDetail{name ="AccountNumber", value ="345234552345" },
                      new BankFieldDetail { name = "AccountName", value= "John Smith" },

                   new BankFieldDetail{ name = "BankName", value = "Bank of Hope"},

                    new BankFieldDetail{ name = "RoutingNumber", value = "122105155" },

                     new BankFieldDetail{ name = "AccountType", value = "S"}
                    }
                };
                //_request.payee_type = "COMPANY";
                //_request.program_id = "100224230";
                //string country = _request.country;
                //string currency = _request.currency;
                //string payee_type = _request.payee_type;
                //string program_id = _request.program_id;
                string program_id = "100224230";
                string result = _getResponse.RestResponse("programs/"+ program_id + "/payees/register-payee", RestSharp.Method.POST, _request, Token, "Payonner_PayeeRegister", "");

                _responseObj = JsonConvert.DeserializeObject<PayeeFormateResponse>(result);


            }
            catch (Exception ex)
            {
                //=====LogWrite here====//
                LogWriter.Add("PayoneerWindows_ServiceLog", (ex.Message + " | " + DateTime.Now.ToString("ddMMyyyy")), "", "", 2);
            }

            // return Token;
        }









        protected override void OnStart(string[] args)
        {
        }

        protected override void OnStop()
        {
        }
    }
}
