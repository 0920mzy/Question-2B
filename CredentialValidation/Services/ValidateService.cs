using CredentialValidation.Models;
using CredentialValidation.Utils;
using EasyNetQ;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Utils;

namespace CredentialValidation.Services
{
    public class ValidateService
    {
        public static ResponseModel Validate (UserModel user)
        {
            ResponseModel responseModel = new ResponseModel();
            if (user == null) return new ResponseModel() { StatusCode = 500, Task = null };
            List<KeyValuePair<string, string>> paramList = new List<KeyValuePair<string, string>>();
            paramList.Add(new KeyValuePair<string, string>("email", user.Email));
            paramList.Add(new KeyValuePair<string, string>("password", user.Password));
            HttpResponseMessage response = HttpClientHelper.GetInstance().Post(Constants.VALIDATE_URL, paramList);
            if (response.StatusCode != System.Net.HttpStatusCode.OK)
            {
                responseModel.StatusCode = (int)System.Net.HttpStatusCode.Unauthorized;
                responseModel.Task = null;
                return responseModel;
            }
            else
            {
                try
                {
                    string curTask = response.Content.ReadAsStringAsync().Result;
                    TokenModel curToken = JsonConvert.DeserializeObject<TokenModel>(curTask);
                    if (curToken == null || string.IsNullOrWhiteSpace(curToken.Token))
                    {
                        responseModel.StatusCode = (int)System.Net.HttpStatusCode.Unauthorized;
                        responseModel.Task = null;
                    }
                    else
                    {
                        string rabbit_conn_str = Environment.GetEnvironmentVariable("RABBITMQ_CONN");
                        using (var bus = RabbitHutch.CreateBus(rabbit_conn_str))
                        {
                            bus.SendReceive.Send("TaskQueue", user.Task);
                        }
                        responseModel.StatusCode = (int)System.Net.HttpStatusCode.OK;
                        responseModel.Task = user.Task;
                    }
                }
                catch(Exception ex)
                {
                    responseModel.StatusCode = (int)System.Net.HttpStatusCode.Unauthorized;
                    responseModel.Task = null;
                }
                return responseModel;

            }
        }
    }
}
