using Newtonsoft.Json;
using PayoneerWindowsService.BAL;
using PayoneerWindowsService.DAL;
using PayoneerWindowsService.DAO;
using RestSharp;
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

        string token = "";

        string programId = "";

        string payee_id_ = "";

        public Service1()
        {
            InitializeComponent();
            token = Payoneer_Login();

            Run_PaymentsCreate(token);

            Get_PaymentsStatus_RecordByID(token);

            //Payonner_PayeeRegister();
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


                string inputString = client_id + ":" + client_secret;

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

        public ApiResponseData GetRegisterPayeeFormat(string Token, string Country, string Currency, string Payee_type, string Program_id)
        {
            string respToken = null;
            string ApiResult = null;
            string result = null;

            ApiResponseData apiRes_ = new ApiResponseData();

            PayeeFormateRequest _request = new PayeeFormateRequest();
            PayeeFormateResponse _responseObj = new PayeeFormateResponse();
            try
            {
                _request.country = Country;
                _request.currency = Currency;
                _request.payee_type = Payee_type;
                _request.program_id = Program_id;
                string country = _request.country;
                string currency = _request.currency;
                string payee_type = _request.payee_type;
                string program_id = _request.program_id;
                return _getResponse.RestResponse("programs/" + program_id + "/payout-methods/bank/account-types/" + payee_type + "/countries/" + country + "/currencies/" + currency + "", RestSharp.Method.GET, "", Token, "Payoneer_RegisterPayeeFormat", "");

                //_responseObj = JsonConvert.DeserializeObject<PayeeFormateResponse>(result);
            }
            catch (Exception ex)
            {
                //=====LogWrite here====//
                LogWriter.Add("PayoneerWindows_ServiceLog", (ex.Message + " | " + DateTime.Now.ToString("ddMMyyyy")), "", "", 2);
            }

            return apiRes_;
        }



        public ApiResponseData Payonner_PayeeRegister(string token, string payee_id_, string benetype_, string beneLastName_, string beneFirstName_,
                                string beneDob_, string address_, string city_, string state_, string benecountry_, string postcode_, string type_, string accounttype_,
                                string benecurrency_, string bankCountry_, string bicswift_, string bankCode_, string bankname_,
                                string beneaccountname_, string beneaccountno_, string programId_, string legalType_, string accountType_, string branchCode_)
        {

            PayeeRegister_Company_Request _companyRequest = new PayeeRegister_Company_Request();
            PayeeRegister_Individual_Request _individualRequest = new PayeeRegister_Individual_Request();
            PayeeFormateResponse _responseObj = new PayeeFormateResponse();
            ApiResponseData rest = null;
            List<BankFieldDetail> bank_field_details = new List<BankFieldDetail>();
            List<Company_BankFieldDetail> company_bank_field_details = new List<Company_BankFieldDetail>();
            try
            {

                rest = GetRegisterPayeeFormat(token, benecountry_, benecurrency_, benetype_, programId_);



                if (rest.StatusCode == "OK")
                {
                    PayeeFormateResponse _presponse = JsonConvert.DeserializeObject<PayeeFormateResponse>(rest.Content);


                    if (_presponse.result.payout_method.fields.items.Any(item => item.field_name == "Swift")
                        && _presponse.result.payout_method.fields.items.Any(item => item.required == true))
                    {
                        bank_field_details.Add(new BankFieldDetail { name = "Swift", value = bicswift_ });
                        company_bank_field_details.Add(new Company_BankFieldDetail { name = "Swift", value = bicswift_ });
                    }
                    if (_presponse.result.payout_method.fields.items.Any(item => item.field_name == "AccountName")
                        && _presponse.result.payout_method.fields.items.Any(item => item.required == true))
                    {
                        bank_field_details.Add(new BankFieldDetail { name = "AccountName", value = beneaccountname_ });
                        company_bank_field_details.Add(new Company_BankFieldDetail { name = "AccountName", value = beneaccountname_ });
                    }
                    if (_presponse.result.payout_method.fields.items.Any(item => item.field_name == "BankName")
                        && _presponse.result.payout_method.fields.items.Any(item => item.required == true))
                    {
                        bank_field_details.Add(new BankFieldDetail { name = "BankName", value = bankname_ });
                        company_bank_field_details.Add(new Company_BankFieldDetail { name = "BankName", value = bankname_ });
                    }
                    if (_presponse.result.payout_method.fields.items.Any(item => item.field_name == "BankNumber")
                        && _presponse.result.payout_method.fields.items.Any(item => item.required == true))
                    {
                        bank_field_details.Add(new BankFieldDetail { name = "BankNumber", value = bankCode_ });
                        company_bank_field_details.Add(new Company_BankFieldDetail { name = "BankNumber", value = bankCode_ });
                    }
                    if (_presponse.result.payout_method.fields.items.Any(item => item.field_name == "AccountNumber")
                        && _presponse.result.payout_method.fields.items.Any(item => item.required == true))
                    {
                        bank_field_details.Add(new BankFieldDetail { name = "AccountNumber", value = beneaccountno_ });
                        company_bank_field_details.Add(new Company_BankFieldDetail { name = "AccountNumber", value = beneaccountno_ });
                    }

                    if (_presponse.result.payout_method.fields.items.Any(item => item.field_name == "IBAN")
                        && _presponse.result.payout_method.fields.items.Any(item => item.required == true))
                    {
                        bank_field_details.Add(new BankFieldDetail { name = "IBAN", value = beneaccountno_ });
                        company_bank_field_details.Add(new Company_BankFieldDetail { name = "IBAN", value = beneaccountno_ });
                    }

                    if (_presponse.result.payout_method.fields.items.Any(item => item.field_name == "BSB")
                        && _presponse.result.payout_method.fields.items.Any(item => item.required == true))
                    {
                        bank_field_details.Add(new BankFieldDetail { name = "BSB", value = bicswift_ });
                        company_bank_field_details.Add(new Company_BankFieldDetail { name = "BSB", value = bicswift_ });
                    }

                    if (_presponse.result.payout_method.fields.items.Any(item => item.field_name == "SortCode")
                        && _presponse.result.payout_method.fields.items.Any(item => item.required == true))
                    {
                        bank_field_details.Add(new BankFieldDetail { name = "SortCode", value = bicswift_ });
                        company_bank_field_details.Add(new Company_BankFieldDetail { name = "SortCode", value = bicswift_ });
                    }

                    if (_presponse.result.payout_method.fields.items.Any(item => item.field_name == "BankCode")
                        && _presponse.result.payout_method.fields.items.Any(item => item.required == true))
                    {
                        bank_field_details.Add(new BankFieldDetail { name = "BankCode", value = bankCode_ });
                        company_bank_field_details.Add(new Company_BankFieldDetail { name = "BankCode", value = bankCode_ });
                    }
                    if (_presponse.result.payout_method.fields.items.Any(item => item.field_name == "BranchCode")
                        && _presponse.result.payout_method.fields.items.Any(item => item.required == true))
                    {
                        bank_field_details.Add(new BankFieldDetail { name = "BranchCode", value = branchCode_ });
                        company_bank_field_details.Add(new Company_BankFieldDetail { name = "BranchCode", value = branchCode_ });
                    }
                    if (_presponse.result.payout_method.fields.items.Any(item => item.field_name == "AccountType")
                        && _presponse.result.payout_method.fields.items.Any(item => item.required == true))
                    {
                        bank_field_details.Add(new BankFieldDetail { name = "AccountType", value = accountType_ });
                        company_bank_field_details.Add(new Company_BankFieldDetail { name = "AccountType", value = accountType_ });
                    }



                    if (benetype_ == "COMPANY")
                    {
                        _companyRequest.payee_id = payee_id_;
                        _companyRequest.payee = new Company_Payee
                        {
                            type = benetype_,     // INDIVIDUAL / COMPANY
                            contact = new Company_Contact
                            {
                                last_name = beneLastName_,
                                first_name = beneFirstName_,
                                date_of_birth = beneDob_
                            },
                            address = new Company_Address
                            {
                                address_line_1 = address_,
                                address_line_2 = "",
                                city = city_,
                                state = state_,
                                country = benecountry_,
                                zip_code = postcode_
                            },
                            company = new Company
                            {
                                incorporated_address_1 = address_,
                                incorporated_city = city_,
                                incorporated_country = benecountry_,
                                legal_type = legalType_,               // PUBLIC/PRIVATE/SOLE_PROPRIETORSHIP/LLC/LLP/NON_PROFIT/LTD/INC
                                name = beneaccountname_
                            }


                        };
                        _companyRequest.payout_method = new Company_Payout_Method
                        {
                            type = type_,       //Type - for Payment - BankTransfer/Cash
                            bank_account_type = accounttype_,       // PERSONAL / COMPANY
                            country = bankCountry_,
                            currency = benecurrency_,
                            bank_field_details = company_bank_field_details


                        };

                    }
                    else
                    {

                        _individualRequest.payee_id = payee_id_;

                        _individualRequest.payee = new Payee
                        {
                            type = benetype_,     // INDIVIDUAL / COMPANY
                            contact = new Contact
                            {
                                last_name = beneLastName_,
                                first_name = beneFirstName_,
                                date_of_birth = beneDob_
                            },
                            address = new Address
                            {
                                address_line_1 = address_,
                                address_line_2 = "",
                                city = city_,
                                state = state_,
                                country = benecountry_,
                                zip_code = postcode_
                            }

                        };
                        _individualRequest.payout_method = new Payout_Method
                        {
                            type = type_,       //Type - for Payment - BankTransfer/Cash
                            bank_account_type = accounttype_,       // PERSONAL / COMPANY
                            country = bankCountry_,
                            currency = benecurrency_,
                            bank_field_details = bank_field_details

                        };
                    }

                }

                if (benetype_ == "COMPANY")
                {
                    return _getResponse.RestResponse("programs/" + programId + "/payees/register-payee", RestSharp.Method.POST, _companyRequest, token, "Payonner_PayeeRegister", "");
                }
                else
                {
                    return _getResponse.RestResponse("programs/" + programId + "/payees/register-payee", RestSharp.Method.POST, _individualRequest, token, "Payonner_PayeeRegister", "");
                }


            }
            catch (Exception ex)
            {
                //=====LogWrite here====//
                LogWriter.Add("PayoneerWindows_ServiceLog", (ex.Message + " | " + DateTime.Now.ToString("ddMMyyyy")), "", "", 2);
            }

            return rest;
        }



        public ApiResponseData Check_PayeeStatus(string token, string payee_id, string program_id)
        {
            ApiResponseData apiRes_ = new ApiResponseData();

            try
            {
                apiRes_ = _getResponse.RestResponse("programs/" + program_id + "/payees/" + payee_id + "/status", RestSharp.Method.GET, null, token, "Payoneer_Get_PayeeStatus", "");

            }
            catch (Exception ex)
            {
                //=====LogWrite here====//

                LogWriter.Add("Payoneer_ServiceLog", (ex.Message + " | " + DateTime.Now.ToString("ddMMyyyy")), "", "", 2);


                ////===============Agency Block==================//

                //var txnStatus_paramObj = new { Txn_No_ = reqTrxnNo_ };
                //_Bal.commonIUD("usp_set_transaction_agency_blocked", txnStatus_paramObj);
            }

            return apiRes_;
        }


        public string Run_PaymentsCreate(string token)
        {

            #region Time
            //===GMT TIME IN MILISECOND - CURRENT DATETIME===//
            DateTime epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            long ms = (long)(DateTime.UtcNow - epoch).TotalMilliseconds;
            // Add 5 hours in milliseconds
            ms += (long)TimeSpan.FromHours(5).TotalMilliseconds;
            string getTime = ms.ToString();
            #endregion

            string apiResponsePaymentID_ = null;

            string reqTrxnNo_ = null;            
            
            string clientRefID = "Pay" + getTime;

            CreatePaymentRequest _request = new CreatePaymentRequest();

            ApiResponseData apiRes_ = new ApiResponseData();

            try
            {
                //DataTable txn_dt = _Bal.getTblData("usp_Fyorin_API_SendTxn_Demo").Tables[0];
                DataTable txn_dt = _Bal.getTblData("usp_Payoneer_API_SendTxn_Demo").Tables[0];

                //token = "";

                if (txn_dt.Rows.Count > 0)
                {

                    if (token == "")
                    {
                        token = Payoneer_Login();
                    }

                    List<CreatePaymentRequest> paymentRequests = new List<CreatePaymentRequest>();

                    List<Payment> PaymentsList = new List<Payment>();

                    foreach (DataRow txn_dr in txn_dt.Rows)
                    {
                        reqTrxnNo_ = txn_dr["TxnNo"].ToString();
                        string benetype_ = txn_dr["benetype"].ToString(); // INDIVIDUAL / COMPANY
                        string beneLastName_ = txn_dr["beneLastName"].ToString();
                        string beneFirstName_ = txn_dr["beneFirstName"].ToString();
                        string beneDob_ = txn_dr["beneDob"].ToString();      //DateOfBirth
                        string address_ = txn_dr["benAddress"].ToString();
                        string city_ = txn_dr["benCity"].ToString();
                        string state_ = txn_dr["benState"].ToString();
                        string benecountry_ = txn_dr["benecountry"].ToString();
                        string postcode_ = txn_dr["postcode"].ToString();  //ZipCode
                        string type_ = txn_dr["paymentType"].ToString(); //Type - for Payment - BankTransfer/Cash
                        string accounttype_ = txn_dr["benetype"].ToString(); // PERSONAL / COMPANY
                        string benecurrency_ = txn_dr["beneCurrency"].ToString(); //BankCurrency
                        string bankCountry_ = txn_dr["bankCountry"].ToString();  //For PayOut

                        //==For Bank Details===//
                        string bicswift_ = txn_dr["bicswift"].ToString();
                        string bankCode_ = txn_dr["bankCode"].ToString(); // Bank Number
                        string bankname_ = txn_dr["bankName"].ToString();   //Bank Name
                        string beneaccountname_ = txn_dr["accountName"].ToString();  //Enter here customer Full Name
                        string beneaccountno_ = txn_dr["beneAccountno"].ToString();
                        //string iban_ = txn_dr["iban"].ToString();
                        string branchCode_ = txn_dr["branchCode"].ToString();
                        int localField_ = Convert.ToInt32(txn_dr["localField"]);
                        string programId_ = txn_dr["programId"].ToString();
                        string legalType_ = txn_dr["legalType"].ToString();
                        string paymentDescription_ = txn_dr["paymentDescription"].ToString();
                        string amount_ = txn_dr["amount"].ToString();
                        string accountType_ = txn_dr["accountType"].ToString();

                        payee_id_ = txn_dr["payeeID"].ToString(); ;

                        programId = programId_;                        

                        reqTrxnNo_ = txn_dr["TxnNo"].ToString();

                        if (payee_id_ == "NA")
                        {
                            payee_id_ = "POY" + _Bal.get_bdTimestamp();

                            //===Create Beneficiary=====//
                            apiRes_ = Payonner_PayeeRegister(token, payee_id_, benetype_, beneLastName_, beneFirstName_,
                            beneDob_, address_, city_, state_, benecountry_, postcode_, type_, accounttype_, benecurrency_,
                            bankCountry_, bicswift_, bankCode_, bankname_, beneaccountname_, beneaccountno_, programId_, legalType_, accountType_, branchCode_);
                        }

                        if (apiRes_.Content.Contains("\"result\":\"Success\""))
                        {
                            var paramObj = new
                            {
                                payeeId = payee_id_,
                                accountNo = beneaccountno_
                            };

                            _Bal.commonIUD("usp_Payoneer_Add_Bene", paramObj);


                            apiRes_ = Check_PayeeStatus(token, payee_id_, programId_);




                            if (apiRes_.StatusCode == "OK")
                            {
                                payeeStatusResponse _response = JsonConvert.DeserializeObject<payeeStatusResponse>(apiRes_.Content);

                                if (_response.result.status.type == 1 && _response.result.account_id != null)
                                {
                                    var paramObjc = new
                                    {
                                        payeeId = payee_id_,
                                        beneAccountID = _response.result.account_id
                                    };

                                    _Bal.commonIUD("usp_Payoneer_Update_Bene", paramObjc);

                                    PaymentsList.Add(new Payment { client_reference_id = clientRefID, payee_id = payee_id_, description = paymentDescription_, currency = benecurrency_, amount = amount_ });

                                    _request.Payments = PaymentsList;

                                    apiRes_ = _getResponse.RestResponse("/programs/" + programId + "/masspayouts", RestSharp.Method.POST, _request, token, "Payonner_Payments_Create", "");

                                    if (apiRes_.Content.Contains("\"result\":\"Payments Created\""))
                                    {
                                        //====For Inprocess====//

                                        var Inprocess_paramObj = new
                                        {
                                            TxnsNumber = txn_dr["TxnNo"],
                                            ThirdPartyCode = payee_id_,
                                            UserID = txn_dr["userID"]
                                        };

                                        _Bal.commonIUD("usp_spInprocessTxn", Inprocess_paramObj);

                                    }
                                    else
                                    {
                                        //Agency block//

                                        var txnStatus_paramObj = new { Txn_No_ = txn_dr["TxnNo"] };
                                        _Bal.commonIUD("usp_set_transaction_agency_blocked", txnStatus_paramObj);
                                    }
                                }

                            }
                        }
                        else
                        {
                            //Agency block//

                            var txnStatus_paramObj = new { Txn_No_ = txn_dr["TxnNo"] };
                            _Bal.commonIUD("usp_set_transaction_agency_blocked", txnStatus_paramObj);
                        }

                    }
                }

            }

            catch (Exception ex)
            {
                //=====LogWrite here====//

                LogWriter.Add("Fyorin_ServiceLog", (ex.Message + " | " + DateTime.Now.ToString("ddMMyyyy")), "", "", 2);


                //===============Agency Block==================//

                var txnStatus_paramObj = new { Txn_No_ = reqTrxnNo_ };
                _Bal.commonIUD("usp_set_transaction_agency_blocked", txnStatus_paramObj);

            }
            return apiResponsePaymentID_;
        }



        //=======================CHECK PAYMENT STATUS========================//
        public void Get_PaymentsStatus_RecordByID(string token) //All InProcess Txns
        {

            string userID = null;

            string txn_number_ = "", apiResponsePaymentID_ = "";

            ApiResponseData apiRes_ = new ApiResponseData();

            try
            {
                DataTable txn_dt = _Bal.getTblData("usp_Payoneer_API_txnStatus").Tables[0];

                if (txn_dt.Rows.Count > 0)
                {
                    if (token == "")
                    {
                        token = Payoneer_Login();
                    }

                    foreach (DataRow tnx_dr in txn_dt.Rows)
                    {
                        apiResponsePaymentID_ = tnx_dr["ThirdPartyCode"].ToString().Split('|')[0]; // for testing does not exist

                        txn_number_ = tnx_dr["TransactionNumber"].ToString();

                        userID = tnx_dr["UserID"].ToString();

                        apiRes_ = _getResponse.RestResponse("/programs/" + programId + "/payouts/" + apiResponsePaymentID_ + "/status", RestSharp.Method.POST, null, token, "Payoneer_Payments_Status", "");

                        GetPaymentStatusResponse _presponse = JsonConvert.DeserializeObject<GetPaymentStatusResponse>(apiRes_.Content);

                        if (_presponse.result.status == "Transferred")
                        {
                            //=====Paid======//
                            #region Paid

                            var StatusPaid_paramObj = new
                            {
                                TransactionNumber = txn_number_,
                                UserID = userID
                            };

                            _Bal.commonIUD("usp_UpdateStatusPaid", StatusPaid_paramObj);

                            #endregion
                        }
                        else if (_presponse.result.status == "Cancelled")
                        {
                            //===============Agency Block==================//
                            var txnStatus_paramObj = new { Txn_No_ = txn_number_ };
                            _Bal.commonIUD("usp_set_transaction_agency_blocked", txnStatus_paramObj);
                        }
                        else if (_presponse.result.status == "Pending")
                        {
                            //====Status will not change====//
                        }
                        else if (_presponse.result.status == "Pending Payee")
                        {
                            //====Status will not change====//
                        }
                        else
                        {
                            //===============Agency Block==================//

                            var txnStatus_paramObj = new { Txn_No_ = txn_number_ };

                            _Bal.commonIUD("usp_set_transaction_agency_blocked", txnStatus_paramObj);
                        }

                    }
                }
            }
            catch (Exception ex)
            {
                //=====LogWrite here====//

                LogWriter.Add("Payoneer_ServiceLog", (ex.Message + " | " + DateTime.Now.ToString("ddMMyyyy")), "", "", 2);

                //===============Agency Block==================//

                var txnStatus_paramObj = new { Txn_No_ = txn_number_ };

                _Bal.commonIUD("usp_set_transaction_agency_blocked", txnStatus_paramObj);
            }

        }




        protected override void OnStart(string[] args)
        {
        }

        protected override void OnStop()
        {
        }
    }
}
