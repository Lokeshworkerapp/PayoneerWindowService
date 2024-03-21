using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayoneerWindowsService.DAL
{
  public class _getLoginResponse
    {
        public static string RestResponse(string url, RestSharp.Method method, object reqObj,
        string Token, string log_method_name, string txn_no)
        {
            string responseContent = string.Empty;
            string jsonRequest = string.Empty;
            string fullUrl_ = url;

            // Serialize the request body to JSON
            jsonRequest = JsonConvert.SerializeObject(reqObj);

            var client = new RestClient();
            var request = new RestRequest(fullUrl_, method);

            // Add headers
            request.AddHeader("Content-Type", "application/json");

            if (!string.IsNullOrEmpty(Token))
            {
                request.AddHeader("Authorization", "Basic " + Token);
            }

            // Add the request body
            request.AddParameter("application/json", jsonRequest, ParameterType.RequestBody);

            try
            {
                IRestResponse response = client.Execute(request);
                responseContent = response.Content;
            }
            catch (Exception exp)
            {
                responseContent = exp.Message;
            }

            return responseContent;
        }
    }
}
