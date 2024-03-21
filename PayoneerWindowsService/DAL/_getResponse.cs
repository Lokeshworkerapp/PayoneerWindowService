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
using RestSharp;

namespace PayoneerWindowsService.DAL
{

    public enum HttpStatusCode
    {
        NoContent = 204
    }

    public class _getResponse
    {
        public static string RestResponse(string url, RestSharp.Method method, object reqObj,
     string Token, string log_method_name, string txn_no)
        {
            string baseUrl = ConfigurationManager.AppSettings["Baseurl"].ToString();

            string fullUrl = baseUrl + url;

            string responseContent = string.Empty;

            string jsonRequest = string.Empty;

            if (log_method_name == "Payoneer_RegisterPayeeFormat")
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

                string content_data = string.Concat(response.Content);

                responseContent = (response.Content.Length.Equals(0) ? response.StatusCode.ToString() : response.Content);


                //if (log_method_name == "Fyorin_Create_SubAccount" || log_method_name == "Fyorin_Contacts_Create")
                //{


                //    if (response.StatusCode.ToString() == "NoContent" || response.StatusCode.ToString() == "Conflict")
                //    {


                //        responseContent = response.Headers.ToList().Find(x => x.Name == "generated-id").Value.ToString();


                //    }
                //    else
                //    {
                //        responseContent = (response.Content.Length.Equals(0) ? response.StatusCode.ToString() : response.Content);
                //    }

                //}
                //else
                //{
                //    responseContent = (response.Content.Length.Equals(0) ? response.StatusCode.ToString() : response.Content);
                //}



            }
            catch (Exception exp)
            {
                responseContent = exp.Message;
            }

            if (!log_method_name.Equals("Payoneer_Login"))
            {
                LogWriter.Add(log_method_name, jsonRequest, responseContent, txn_no, 2);
            }

            return responseContent;
        }
    }

}