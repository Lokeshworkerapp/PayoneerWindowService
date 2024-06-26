﻿using Newtonsoft.Json;
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

        public Service1()
        {
            InitializeComponent();

            token = Payoneer_Login();

            //GetKycMethod(token);

            //QueryProgramBalanceMethod(token);

            //FundTransferMethod(token);

            Payoneer_PayeeRegister(token);

            Check_PayeeStatus(token);

            Run_PaymentsCreate(token);

            Get_PaymentsStatus_RecordByID(token);

            //CancelPayoutMethod(token);

            //EditTransferMethod(token);

        }


        //======================= GET TOKEN ========================//
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

                //BaseURL For UAT = "https://login.sandbox.payoneer.com/api/v2/oauth2/token";

                //BaseURL For LIVE = "https://login.payoneer.com/api/v2/oauth2/token";


                string result = _getLoginResponse.RestResponse("https://login.payoneer.com/api/v2/oauth2/token", RestSharp.Method.POST, _request, respToken, "Payoneer_Login", "");

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



        //======================= GET REQUIRED FIELDS ========================//
        public ApiResponseData GetRegisterPayeeFormat(string Token, string country, string currency, string payee_type, string program_id, string payee_id_)
        {

            ApiResponseData apiRes_ = new ApiResponseData();

            try
            {

                return _getResponse.RestResponse("programs/" + program_id + "/payout-methods/bank/account-types/" + payee_type + "/countries/" + country + "/currencies/" + currency + "", RestSharp.Method.GET, "", Token, "Payoneer_RegisterPayeeFormat", payee_id_);

            }
            catch (Exception ex)
            {
                //=====LogWrite here====//
                LogWriter.Add("PayoneerWindows_ServiceLog", (ex.Message + " | " + DateTime.Now.ToString("ddMMyyyy")), "", "", 2);
            }

            return apiRes_;
        }



        //======================= EDIT TRANSFER METHOD ========================//
        public ApiResponseData EditTransferMethod(string Token)
        {

            ApiResponseData apiRes_ = new ApiResponseData();

            EditTransferMethod_Request editTransferMethod_Request = new EditTransferMethod_Request();

            List<EditTransfer_BankFieldDetail> editTransfer_BankFields = new List<EditTransfer_BankFieldDetail>();


            try
            {
                DataTable txn_dt = _Bal.getTblData("usp_Payoneer_Bene_Registration").Tables[0];

                //token = "";

                if (txn_dt.Rows.Count > 0)
                {

                    if (token == "")
                    {
                        token = Payoneer_Login();
                    }

                    //List<EditTransferMethod_Request> paymentRequests = new List<EditTransferMethod_Request>();                    

                    foreach (DataRow txn_dr in txn_dt.Rows)
                    {
                        editTransferMethod_Request.type = "BANK";
                        editTransferMethod_Request.bank_account_type = txn_dr["benetype"].ToString(); // INDIVIDUAL / COMPANY
                        editTransferMethod_Request.country = txn_dr["benecountry"].ToString();
                        editTransferMethod_Request.currency = txn_dr["beneCurrency"].ToString();

                        string program_id = txn_dr["programId"].ToString();
                        string payee_id = "POY1712664606";
                        string AccountNumber = txn_dr["beneAccountno"].ToString();
                        string AccountName = txn_dr["accountName"].ToString();
                        string BankName = txn_dr["bankName"].ToString();
                        string RoutingNumber = txn_dr["routingCode"].ToString();
                        string AccountType = txn_dr["accountType"].ToString();
                        string Swift = txn_dr["bicswift"].ToString();

                        editTransfer_BankFields.Add(new EditTransfer_BankFieldDetail { name = "AccountNumber", value = AccountNumber });
                        editTransfer_BankFields.Add(new EditTransfer_BankFieldDetail { name = "AccountName", value = AccountName });
                        editTransfer_BankFields.Add(new EditTransfer_BankFieldDetail { name = "BankName", value = BankName });
                        editTransfer_BankFields.Add(new EditTransfer_BankFieldDetail { name = "RoutingNumber", value = RoutingNumber });
                        editTransfer_BankFields.Add(new EditTransfer_BankFieldDetail { name = "AccountType", value = AccountType });
                        editTransfer_BankFields.Add(new EditTransfer_BankFieldDetail { name = "Swift", value = Swift });

                        editTransferMethod_Request.bank_field_details = editTransfer_BankFields;

                        return _getResponse.RestResponse("programs/" + program_id + "/payees/" + payee_id + "/payout-methods", RestSharp.Method.POST, editTransferMethod_Request, Token, "Payoneer_EditTransferMethod", payee_id);


                    }
                }

                
            }
            catch (Exception ex)
            {
                //=====LogWrite here====//
                LogWriter.Add("PayoneerWindows_ServiceLog", (ex.Message + " | " + DateTime.Now.ToString("ddMMyyyy")), "", "", 2);
            }

            return apiRes_;
        }



        //======================= PAYEE REGISTRATION ========================//

        public ApiResponseData Payoneer_PayeeRegister(string token)
        {

            PayeeRegister_Company_Request _companyRequest = new PayeeRegister_Company_Request();
            PayeeRegister_Individual_Request _individualRequest = new PayeeRegister_Individual_Request();
            PayeeFormateResponse _responseObj = new PayeeFormateResponse();
            ApiResponseData rest = null;
            List<BankFieldDetail> bank_field_details = new List<BankFieldDetail>();
            List<Company_BankFieldDetail> company_bank_field_details = new List<Company_BankFieldDetail>();

            try
            {

                DataTable txn_dt = _Bal.getTblData("usp_Payoneer_Bene_Registration").Tables[0];

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

                        string benetype_ = txn_dr["benetype"].ToString();   // “PERSONAL/INDIVIDUAL”// “COMPANY”
                        string beneLastName_ = txn_dr["beneLastName"].ToString();
                        string beneFirstName_ = txn_dr["beneFirstName"].ToString();
                        string beneDob_ = txn_dr["beneDob"].ToString();      //DateOfBirth
                        string address_ = txn_dr["benAddress"].ToString();
                        string city_ = txn_dr["benCity"].ToString();
                        string state_ = txn_dr["benState"].ToString();
                        string benecountry_ = txn_dr["benecountry"].ToString();
                        string postcode_ = txn_dr["postcode"].ToString();  //ZipCode
                        string type_ = txn_dr["paymentType"].ToString(); //Type - for Payment - BankTransfer/Cash
                        string accounttype_ = txn_dr["benetype"].ToString(); // “PERSONAL/INDIVIDUAL”// “COMPANY”
                        string benecurrency_ = txn_dr["beneCurrency"].ToString(); //BankCurrency
                        string bankCountry_ = txn_dr["bankCountry"].ToString();  //For PayOut

                        //==For Bank Details===//
                        string bicswift_ = txn_dr["bicswift"].ToString();
                        string bankCode_ = txn_dr["bankCode"].ToString(); // Bank Number
                        string bankname_ = txn_dr["bankName"].ToString();   //Bank Name
                        string beneaccountname_ = txn_dr["accountName"].ToString();  //Enter here customer Full Name
                        string beneaccountno_ = txn_dr["beneAccountno"].ToString();
                        string routingCode_ = txn_dr["routingCode"].ToString();
                        //string iban_ = txn_dr["iban"].ToString();
                        string branchCode_ = txn_dr["branchCode"].ToString();
                        int localField_ = Convert.ToInt32(txn_dr["localField"]);
                        string programId_ = txn_dr["programId"].ToString();
                        string legalType_ = txn_dr["legalType"].ToString();
                        string paymentDescription_ = txn_dr["paymentDescription"].ToString();
                        string amount_ = txn_dr["amount"].ToString();
                        string accountType_ = txn_dr["accountType"].ToString();

                        string payee_id_ = txn_dr["payeeID"].ToString(); //Always NA from Database

                        //reqTrxnNo_ = txn_dr["TxnNo"].ToString();

                        if (payee_id_ == "NA")
                        {
                            payee_id_ = "POY" + _Bal.get_bdTimestamp();


                            //------------------CHECK REQUIRED FIELD HERE---------------------------------//

                            rest = GetRegisterPayeeFormat(token, benecountry_, benecurrency_, benetype_, programId_, payee_id_);


                            if (rest.StatusCode == "OK")
                            {
                                PayeeFormateResponse _presponse = JsonConvert.DeserializeObject<PayeeFormateResponse>(rest.Content);

                                if (_presponse.result.payout_method.fields.items.Any(item => item.field_name == "Swift" && item.required == true))
                                {
                                    bank_field_details.Add(new BankFieldDetail { name = "Swift", value = bicswift_ });
                                    company_bank_field_details.Add(new Company_BankFieldDetail { name = "Swift", value = bicswift_ });
                                }

                                if (_presponse.result.payout_method.fields.items.Any(item => item.field_name == "AccountName" && item.required == true))
                                {
                                    bank_field_details.Add(new BankFieldDetail { name = "AccountName", value = beneaccountname_ });
                                    company_bank_field_details.Add(new Company_BankFieldDetail { name = "AccountName", value = beneaccountname_ });
                                }

                                if (_presponse.result.payout_method.fields.items.Any(item => item.field_name == "BankName" && item.required == true))
                                {
                                    bank_field_details.Add(new BankFieldDetail { name = "BankName", value = bankname_ });
                                    company_bank_field_details.Add(new Company_BankFieldDetail { name = "BankName", value = bankname_ });
                                }

                                if (_presponse.result.payout_method.fields.items.Any(item => item.field_name == "BankNumber" && item.required == true))
                                {
                                    bank_field_details.Add(new BankFieldDetail { name = "BankNumber", value = bankCode_ });
                                    company_bank_field_details.Add(new Company_BankFieldDetail { name = "BankNumber", value = bankCode_ });
                                }

                                if (_presponse.result.payout_method.fields.items.Any(item => item.field_name == "AccountNumber" && item.required == true))
                                {
                                    bank_field_details.Add(new BankFieldDetail { name = "AccountNumber", value = beneaccountno_ });
                                    company_bank_field_details.Add(new Company_BankFieldDetail { name = "AccountNumber", value = beneaccountno_ });
                                }

                                if (_presponse.result.payout_method.fields.items.Any(item => item.field_name == "IBAN" && item.required == true))
                                {
                                    bank_field_details.Add(new BankFieldDetail { name = "IBAN", value = beneaccountno_ });
                                    company_bank_field_details.Add(new Company_BankFieldDetail { name = "IBAN", value = beneaccountno_ });
                                }

                                if (_presponse.result.payout_method.fields.items.Any(item => item.field_name == "BIC" && item.required == true))
                                {
                                    bank_field_details.Add(new BankFieldDetail { name = "BIC", value = bicswift_ });
                                    company_bank_field_details.Add(new Company_BankFieldDetail { name = "BIC", value = bicswift_ });
                                }

                                if (_presponse.result.payout_method.fields.items.Any(item => item.field_name == "BSB" && item.required == true))
                                {
                                    bank_field_details.Add(new BankFieldDetail { name = "BSB", value = bicswift_ });
                                    company_bank_field_details.Add(new Company_BankFieldDetail { name = "BSB", value = bicswift_ });
                                }

                                if (_presponse.result.payout_method.fields.items.Any(item => item.field_name == "SortCode" && item.required == true))
                                {
                                    bank_field_details.Add(new BankFieldDetail { name = "SortCode", value = bicswift_ });
                                    company_bank_field_details.Add(new Company_BankFieldDetail { name = "SortCode", value = bicswift_ });
                                }

                                if (_presponse.result.payout_method.fields.items.Any(item => item.field_name == "BankCode" && item.required == true))
                                {
                                    bank_field_details.Add(new BankFieldDetail { name = "BankCode", value = bankCode_ });
                                    company_bank_field_details.Add(new Company_BankFieldDetail { name = "BankCode", value = bankCode_ });
                                }

                                if (_presponse.result.payout_method.fields.items.Any(item => item.field_name == "BranchCode" && item.required == true))
                                {
                                    bank_field_details.Add(new BankFieldDetail { name = "BranchCode", value = branchCode_ });
                                    company_bank_field_details.Add(new Company_BankFieldDetail { name = "BranchCode", value = branchCode_ });
                                }

                                if (_presponse.result.payout_method.fields.items.Any(item => item.field_name == "AccountType" && item.required == true))
                                {
                                    bank_field_details.Add(new BankFieldDetail { name = "AccountType", value = accountType_ });
                                    company_bank_field_details.Add(new Company_BankFieldDetail { name = "AccountType", value = accountType_ });
                                }

                                if (_presponse.result.payout_method.fields.items.Any(item => item.field_name == "RoutingNumber" && item.required == true))
                                {
                                    bank_field_details.Add(new BankFieldDetail { name = "RoutingNumber", value = routingCode_ });
                                    company_bank_field_details.Add(new Company_BankFieldDetail { name = "RoutingNumber", value = routingCode_ });
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
                                            incorporated_address_2 = "",
                                            incorporated_city = city_,
                                            incorporated_state = state_,
                                            incorporated_country = benecountry_,
                                            legal_type = legalType_,               // PUBLIC/PRIVATE/SOLE_PROPRIETORSHIP/LLC/LLP/NON_PROFIT/LTD/INC
                                            name = beneaccountname_,
                                            incorporated_zipcode = postcode_
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
                                rest = _getResponse.RestResponse("programs/" + programId_ + "/payees/register-payee", RestSharp.Method.POST, _companyRequest, token, "Payoneer_PayeeRegister", payee_id_);
                            }
                            else
                            {
                                rest = _getResponse.RestResponse("programs/" + programId_ + "/payees/register-payee", RestSharp.Method.POST, _individualRequest, token, "Payoneer_PayeeRegister", payee_id_);
                            }

                            if (rest.Content.Contains("\"result\":\"Success\""))
                            {
                                var paramObj = new
                                {
                                    payeeId = payee_id_,
                                    accountNo = beneaccountno_,
                                    programId = programId_
                                };

                                _Bal.commonIUD("usp_Payoneer_Add_Bene", paramObj);
                            }
                            else
                            {
                                //Agency block//

                                var txnStatus_paramObj = new { payee_id_ };
                                _Bal.commonIUD("usp_set_transaction_agency_blocked", txnStatus_paramObj);
                            }

                        }

                        //GetKycMethod(token,programId_,payee_id_);

                    }
                }

            }
            catch (Exception ex)
            {
                //=====LogWrite here====//
                LogWriter.Add("PayoneerWindows_ServiceLog", (ex.Message + " | " + DateTime.Now.ToString("ddMMyyyy")), "", "", 2);
            }

            return rest;
        }



        //=======================CHECK PAYEE REGISTRATION STATUS========================//

        public ApiResponseData Check_PayeeStatus(string token)
        {
            ApiResponseData apiRes_ = new ApiResponseData();

            string payee_id = "";

            string program_id = "";

            DataTable txn_dt = _Bal.getTblData("usp_Payoneer_Check_Bene_Status").Tables[0];

            if (txn_dt.Rows.Count > 0)
            {
                if (token == "")
                {
                    token = Payoneer_Login();
                }
            }
            try
            {
                foreach (DataRow tnx_dr in txn_dt.Rows)
                {
                    payee_id = tnx_dr["PayeeID"].ToString(); // for testing does not exist

                    program_id = tnx_dr["ProgramID"].ToString();

                    apiRes_ = _getResponse.RestResponse("programs/" + program_id + "/payees/" + payee_id + "/status", RestSharp.Method.GET, null, token, "Payoneer_Get_PayeeStatus", payee_id);

                    payeeStatusResponse _response = JsonConvert.DeserializeObject<payeeStatusResponse>(apiRes_.Content);


                    if (_response.result.status.type != 0)
                    {
                        var paramObjc = new
                        {
                            payeeId = payee_id,
                            IsApproved = _response.result.status.type,
                            beneAccountID = _response.result.account_id
                        };

                        _Bal.commonIUD("usp_Payoneer_Update_Bene", paramObjc);
                    }
                }
            }
            catch (Exception ex)
            {
                //=====LogWrite here====//

                LogWriter.Add("Payoneer_ServiceLog", (ex.Message + " | " + DateTime.Now.ToString("ddMMyyyy")), "", "", 2);

            }

            return apiRes_;
        }



        //=======================PAYMENT CREATE========================//

        public string Run_PaymentsCreate(string token)
        {

            string apiResponsePaymentID_ = null;

            string reqTrxnNo_ = null;

            CreatePaymentRequest _request = new CreatePaymentRequest();

            ApiResponseData apiRes_ = new ApiResponseData();

            try
            {
                DataTable txn_dt = _Bal.getTblData("usp_Payoneer_Payment_Create").Tables[0];

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
                        string clientRefID = txn_dr["TrxnNo"].ToString();  //This is as a TransactionNumber

                        string paymentDescription_ = txn_dr["description"].ToString();
                        string benecurrency_ = txn_dr["currency"].ToString();
                        string amount_ = txn_dr["amount"].ToString();
                        string payeeID_ = txn_dr["payee_id"].ToString();
                        string programId = txn_dr["programId"].ToString();

                        //reqTrxnNo_ = txn_dr["TxnNo"].ToString();
                        //===Start For Send Transaction Process - Here ===// 

                        PaymentsList.Add(new Payment { client_reference_id = clientRefID, payee_id = payeeID_, description = paymentDescription_, currency = benecurrency_, amount = amount_ });

                        _request.Payments = PaymentsList;

                        apiRes_ = _getResponse.RestResponse("/programs/" + programId + "/masspayouts", RestSharp.Method.POST, _request, token, "Payoneer_Payments_Create", clientRefID);

                        if (apiRes_.Content.Contains("\"result\":\"Payments Created\""))
                        {
                            //====For Inprocess====//

                            var Inprocess_paramObj = new
                            {
                                //TxnsNumber = txn_dr["TxnNo"],                                
                                //ThirdPartyCode = payeeID_,
                                //UserID = txn_dr["userID"]

                                TxnsNumber = clientRefID,
                                ThirdPartyCode = clientRefID,
                                UserID = "1300562"

                            };

                            _Bal.commonIUD("usp_spInprocessTxn_For_Payoneer", Inprocess_paramObj);

                        }
                        else
                        {
                            //Agency block//

                            //var txnStatus_paramObj = new { Txn_No_ = txn_dr["TxnNo"] };
                            var txnStatus_paramObj = new { Txn_No_ = clientRefID };
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

            string txn_number_ = "";

            string programId = "";

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

                        txn_number_ = tnx_dr["TransactionNumber"].ToString();

                        userID = tnx_dr["UserID"].ToString();

                        programId = tnx_dr["program_Id"].ToString();

                        apiRes_ = _getResponse.RestResponse("programs/" + programId + "/payouts/" + txn_number_ + "/status", RestSharp.Method.POST, null, token, "Payoneer_Payments_Status", txn_number_);

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



        //======================= CANCEL PAYOUT METHOD ========================//
        public ApiResponseData CancelPayoutMethod(string Token)
        {

            ApiResponseData apiRes_ = new ApiResponseData();

            CancelPayout_Response payout_Response = new CancelPayout_Response();


            try
            {
                DataTable txn_dt = _Bal.getTblData("usp_Payoneer_Bene_Registration").Tables[0];

                //token = "";

                if (txn_dt.Rows.Count > 0)
                {

                    if (token == "")
                    {
                        token = Payoneer_Login();
                    }

                    //List<EditTransferMethod_Request> paymentRequests = new List<EditTransferMethod_Request>();                    

                    foreach (DataRow txn_dr in txn_dt.Rows)
                    {
                        
                        string program_id = txn_dr["programId"].ToString();                        
                        //string client_reference_id = txn_dr["TxnNo"].ToString();
                        string client_reference_id = "TXN8053358656";


                        return _getResponse.RestResponse("programs/"+ program_id + "/payouts/"+ client_reference_id + "/cancel", RestSharp.Method.PUT, "", Token, "Payoneer_Cancel_PayoutMethod", client_reference_id);


                    }
                }


            }
            catch (Exception ex)
            {
                //=====LogWrite here====//
                LogWriter.Add("PayoneerWindows_ServiceLog", (ex.Message + " | " + DateTime.Now.ToString("ddMMyyyy")), "", "", 2);
            }

            return apiRes_;
        }



        //======================= Get KYC METHOD ========================//
        public ApiResponseData GetKycMethod(string Token)
        {

            ApiResponseData apiRes_ = new ApiResponseData();

            try
            {
                DataTable txn_dt = _Bal.getTblData("usp_Payoneer_Bene_Registration").Tables[0];

                //token = "";

                if (txn_dt.Rows.Count > 0)
                {

                    if (token == "")
                    {
                        token = Payoneer_Login();
                    }

                    foreach (DataRow txn_dr in txn_dt.Rows)
                    {

                        string program_id = txn_dr["programId"].ToString();
                        //string payee_id = txn_dr["payeeID"].ToString();
                        string payee_id = "POY1712901971";


                        return _getResponse.RestResponse("programs/"+ program_id +"/payees/"+ payee_id +"/kyc", RestSharp.Method.GET, "", Token, "Payoneer_GET_KYC", payee_id);


                    }
                }


            }
            catch (Exception ex)
            {
                //=====LogWrite here====//
                LogWriter.Add("PayoneerWindows_ServiceLog", (ex.Message + " | " + DateTime.Now.ToString("ddMMyyyy")), "", "", 2);
            }

            return apiRes_;
        }



        //======================= QUERY PROGRAM BALANCE METHOD ========================//
        public ApiResponseData QueryProgramBalanceMethod(string Token)
        {

            ApiResponseData apiRes_ = new ApiResponseData();

            try
            {
                DataTable txn_dt = _Bal.getTblData("usp_Payoneer_Bene_Registration").Tables[0];

                //token = "";

                if (txn_dt.Rows.Count > 0)
                {

                    if (token == "")
                    {
                        token = Payoneer_Login();
                    }

                    foreach (DataRow txn_dr in txn_dt.Rows)
                    {

                        string program_id = txn_dr["programId"].ToString();
                        //string payee_id = txn_dr["payeeID"].ToString();
                        //string payee_id = "POY1712813651";


                        return _getResponse.RestResponse("programs/" + program_id + "/balance", RestSharp.Method.GET, "", Token, "Payoneer_QueryProgramBalance", program_id);


                    }
                }


            }
            catch (Exception ex)
            {
                //=====LogWrite here====//
                LogWriter.Add("PayoneerWindows_ServiceLog", (ex.Message + " | " + DateTime.Now.ToString("ddMMyyyy")), "", "", 2);
            }

            return apiRes_;
        }



        //======================= FUND TRANSFER METHOD ========================//
        public ApiResponseData FundTransferMethod(string Token)
        {

            ApiResponseData apiRes_ = new ApiResponseData();

            FundTransfer_Request fundTransfer_ = new FundTransfer_Request();

            try
            {
                DataTable txn_dt = _Bal.getTblData("usp_Payoneer_Bene_Registration").Tables[0];

                //token = "";

                if (txn_dt.Rows.Count > 0)
                {

                    if (token == "")
                    {
                        token = Payoneer_Login();
                    }

                    foreach (DataRow txn_dr in txn_dt.Rows)
                    {

                        //fundTransfer_.target_partner_id = txn_dr["target_partner_id"].ToString();
                        //fundTransfer_.amount = txn_dr["amount"].ToString();
                        //fundTransfer_.description = txn_dr["description"].ToString();

                        fundTransfer_.target_partner_id = "123123123";
                        fundTransfer_.amount = "1000";
                        fundTransfer_.description = "test transfer";

                        string program_id = txn_dr["programId"].ToString();
                        //string payee_id = "POY1712813651";


                        return _getResponse.RestResponse("programs/" + program_id + "/transfer", RestSharp.Method.POST, fundTransfer_, Token, "Payoneer_FundTransfer", program_id);


                    }
                }


            }
            catch (Exception ex)
            {
                //=====LogWrite here====//
                LogWriter.Add("PayoneerWindows_ServiceLog", (ex.Message + " | " + DateTime.Now.ToString("ddMMyyyy")), "", "", 2);
            }

            return apiRes_;
        }




        //===========SET TIMER============//
        protected override void OnStart(string[] args)
        {
        }

        protected override void OnStop()
        {
        }

    }
}
