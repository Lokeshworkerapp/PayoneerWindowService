﻿using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PayoneerWindowsService.BAL;
using PayoneerWindowsService.DAO;
using RestSharp;

namespace PayoneerWindowsService.DAL
{

    public enum HttpStatusCode
    {
        NoContent = 204
    }

    public class _getResponse
    {
        public static ApiResponseData RestResponse(string url, RestSharp.Method method, object reqObj,
     string Token, string log_method_name, string txn_no)
        {            

            ApiResponseData apiRes_ = new ApiResponseData();

            string baseUrl = ConfigurationManager.AppSettings["Baseurl"].ToString();

            string fullUrl = baseUrl + url;

            string responseContent = string.Empty;

            string jsonRequest = string.Empty;

            string content_data = null;

            if (log_method_name == "Payoneer_RegisterPayeeFormat" || 
                log_method_name == "Payoneer_Get_PayeeStatus" || 
                log_method_name == "Payoneer_Cancel_PayoutMethod" ||
                log_method_name == "Payoneer_GET_KYC" ||
                log_method_name == "Payoneer_QueryProgramBalance")
            {
                string fullUrl_ = fullUrl;
                jsonRequest = JsonConvert.SerializeObject(fullUrl_);
            }
            else
            {
                jsonRequest = JsonConvert.SerializeObject(reqObj);
            }

           
            var client = new RestClient();

            var request = new RestRequest(fullUrl, method);

            request.AddHeader("Content-Type", "application/json");

            if (!string.IsNullOrEmpty(Token))
            {
                request.AddHeader("Authorization", "Bearer " + Token);
            }

            if (reqObj != null)
            {
                request.AddJsonBody(jsonRequest);
            }

            IRestResponse response = null; //wait

            try
            {
                response = client.Execute(request);

                content_data = string.Concat(response.Content);

                apiRes_.StatusCode = response.StatusCode.ToString();

                apiRes_.Content = content_data;

            }
            catch (Exception exp)
            {
                responseContent = exp.Message;
            }

            if (!log_method_name.Equals("Payoneer_Login"))
            {
                LogWriter.Add(log_method_name, jsonRequest, content_data, txn_no, 2);
            }

            return apiRes_;
        }
    }

}
