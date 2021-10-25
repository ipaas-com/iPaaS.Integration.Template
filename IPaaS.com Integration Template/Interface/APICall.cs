using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Integration.Constants;
using Integration;

namespace Integration.Data.Interface
{
    class APICall
    {
        public RestClient _integrationClient;
        private Connection _connection;
        private DateTime lastRestRequestCreateDT;   // We use this to determine the total time taken for a given request. We log the DT when a rest request is made (the first step of each
                                                    // wrapped call) and then find the difference in the ResponseHandler.
        public string URL;
        public string Action;
        public string Action_CustomerFacing;
        public Method RequestMethod;
        public Type ResponseType;
        public Guid TrackingGuid;
        public TM_MappingCollectionType CollectionType;

        private List<Parameter> parameters;
        public bool PUTtoPOST = false;

        public APICall(CallWrapper wrapper, string URL, string Action, string Action_CustomerFacing, Type ResponseType, Guid? TrackingGuid = null, 
            TM_MappingCollectionType CollectionType = TM_MappingCollectionType.NONE, Method RequestMethod = Method.GET)
        {
            this._integrationClient = wrapper._integrationClient;
            this._connection = wrapper._integrationConnection;

            this.URL = URL;
            this.Action = Action;
            this.Action_CustomerFacing = Action_CustomerFacing;
            this.ResponseType = ResponseType;
            if (TrackingGuid.HasValue)
                this.TrackingGuid = TrackingGuid.Value;
            else
                this.TrackingGuid = Guid.NewGuid();
            this.CollectionType = CollectionType;
            this.RequestMethod = RequestMethod;
        }

        public void AddBodyParameter(object Value)
        {
            string bodyJSON = JsonConvert.SerializeObject(Value, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
            AddParameter("application/json", bodyJSON, ParameterType.RequestBody);
        }

        public void AddParameter(string Name, object Value, ParameterType Type)
        {
            if (parameters == null)
                parameters = new List<Parameter>();
            parameters.Add(new Parameter(Name, Value, Type));
        }

        public async Task<object> ProcessRequestAsync()
        {
            RestSharp.RestRequest req = CreateRestRequest(URL);
            req.Method = RequestMethod;
            if(parameters != null)
            req.Parameters.AddRange(parameters);

            if (_integrationClient.Encoding == null)
                _integrationClient.Encoding = (new RestClient()).Encoding;

            var resp = await _integrationClient.ExecuteAsync(req);
            IntegrationAPIResponse IntegrationResponse = HandleResponse((RestSharp.RestResponse)resp, Action, Action_CustomerFacing, mappingCollectionType: CollectionType);
            if (IntegrationResponse.action == IntegrationAPIResponse.ResponseAction.Retry)
                await ProcessRequestAsync();
            else if (IntegrationResponse.action == IntegrationAPIResponse.ResponseAction.PUTtoPOST)
            {
                this.PUTtoPOST = true; //If there is a PUTtoPOST response, set the flag and kick it back to the call wrapper
                return null;
            }

            //If this was a delete request or there is no data, do not attempt a deserialization
            if (RequestMethod == Method.DELETE || IntegrationResponse.data == null)
                return null;

            object retVal = JsonConvert.DeserializeObject(Convert.ToString(IntegrationResponse.data), ResponseType);
            return retVal;
        }

        private RestSharp.RestRequest CreateRestRequest(string url)
        {
            RestSharp.RestRequest req = new RestRequest(url, Method.GET);
            req.RequestFormat = DataFormat.Json;
            req.AddHeader("Authorization", string.Format("Basic {0}", Convert.ToBase64String(Encoding.ASCII.GetBytes(string.Format("{0}:{1}", this._connection.Settings.APIUser, this._connection.Settings.APIPassword)))));
            lastRestRequestCreateDT = DateTime.Now;
            return req;
        }

        private IntegrationAPIResponse HandleResponse(RestSharp.RestResponse resp, string action, string action_CustomerFacing, string scope = null, TM_MappingCollectionType mappingCollectionType = TM_MappingCollectionType.NONE)
        {
            if (resp.ErrorException != null)
            {
                LogRequest(resp.Request, action, true, scope);
                _connection.Logger.Log_ActivityTracker(string.Format("Failed API Call to {0}", Identity.Name), "Recieved ErrorException from " + action_CustomerFacing + ". See Tech log for more details", "Error", (int)mappingCollectionType);
                _connection.Logger.Log_Technical("E", string.Format("{0} CallWrapper.{1}", Identity.Name, action), resp.ErrorException.Message);
                throw new Exception(resp.ErrorException.Message);
            }

            IntegrationAPIResponse APIResponse = new IntegrationAPIResponse();

            APIResponse = new IntegrationAPIResponse();
            APIResponse.data = resp.Content;

            if (resp.StatusCode == System.Net.HttpStatusCode.NoContent)
            {
                _connection.Logger.Log_Technical("D", string.Format("{0} CallWrapper.{1}", Identity.Name, action), "Recieved no content HTTP status");
            }
            else if (resp.StatusCode != System.Net.HttpStatusCode.OK && resp.StatusCode != System.Net.HttpStatusCode.Created)
            {
                LogRequest(resp.Request, action, true);
                //string errMsg = ProcessFullErrorMessage(resp);
                string errMsg = "";

                //There may be a lot of variant in how error responses appear. Sometimes no consisentency even within a single call. E.g. a
                //  NotFound error might be in one format, but a permissions issue is in an entirely different format. So we just deserialize to 
                //  a dictionary and handle what is there as we can.
                Dictionary<string, object> errorResponseDictionary;
                try
                {
                    errorResponseDictionary = JsonConvert.DeserializeObject<Dictionary<string, object>>(resp.Content);
                }
                catch(Exception ex)
                {
                    try
                    {
                        //Construct Data Errors per the Third Party Integration Documentation
                        var errorResponseDictionaryList = JsonConvert.DeserializeObject<List<Dictionary<string, object>>>(resp.Content);
                        if (errorResponseDictionaryList.Count == 1)
                            errorResponseDictionary = errorResponseDictionaryList[0];
                        else if(errorResponseDictionaryList.Count > 1)
                        {
                            errorResponseDictionary = errorResponseDictionaryList[0];
                            errorResponseDictionary.Add("Full Details", resp.Content);
                        }
                        else
                            errorResponseDictionary = new Dictionary<string, object>();
                    }
                    catch(Exception ex2)
                    {
                        errorResponseDictionary = new Dictionary<string, object>();
                    }
                }
                // ===================================================
                // Clean messages Here
                // ===================================================

                //if (errorResponseDictionary.ContainsKey("title"))
                //    errMsg += ": " + Convert.ToString(errorResponseDictionary["title"]);

                ////TODO(low priority): This is in JSON, we might want to format it better. 
                ////We check that it isn't an empty array {} as well
                //if (errorResponseDictionary.ContainsKey("errors") && Convert.ToString(errorResponseDictionary["errors"]) != "{}")
                //    errMsg += ": " + Convert.ToString(errorResponseDictionary["errors"]);

                ////Note: errorS is the typically v3 style and v2 is usually error, but everything is chaos and we can't really be sure.
                //if (errorResponseDictionary.ContainsKey("error"))
                //    errMsg += ": " + Convert.ToString(errorResponseDictionary["error"]);

                //if (errorResponseDictionary.ContainsKey("message"))
                //    errMsg += ": " + Convert.ToString(errorResponseDictionary["message"]);

                //if (errorResponseDictionary.ContainsKey("details"))
                //    errMsg += ": " + Convert.ToString(errorResponseDictionary["details"]);

                //if (errorResponseDictionary.ContainsKey("Full Details"))
                //    errMsg += ": Additional Errors" + Convert.ToString(errorResponseDictionary["Full Details"]);

                //If we have no clean messages added, then we add the whole thing.
                if (errMsg == "")
                    errMsg += ": " + resp.Content;

                errMsg = string.Format("Error calling {0} CallWrapper.{1}{2}  (Http Code: {3})", Identity.Name, action, errMsg, Convert.ToString(resp.StatusCode));

                _connection.Logger.Log_Technical("E", string.Format("{0} CallWrapper.{1}", Identity.Name, action), errMsg);
                throw new Exception(errMsg);
            }

            LogRequest(resp.Request, action, false, scope);
            _connection.Logger.Log_ActivityTracker(string.Format("Succesful API Call to {0}", Identity.Name), "Successful call to " + action_CustomerFacing, "Complete", (int)mappingCollectionType);
            _connection.Logger.Log_Technical("D", string.Format("{0}CallWrapper.{1}", Identity.Name, action), "Success");
            _connection.Logger.Log_Technical("V", string.Format("{0}CallWrapper.{1}", Identity.Name, action), resp.Content);
            APIResponse.action = IntegrationAPIResponse.ResponseAction.Continue;
            return APIResponse;
        }

        //The UseErrorSeverity parameter allows us to specify the LogSeverity as Error on the request log. This will let us store the full request
        //  for any failed calls, which should ease in debugging.
        private void LogRequest(IRestRequest request, string action, bool UseErrorSeverity, string scope = null)
        {
            string logSeverity;

            if (UseErrorSeverity)
                logSeverity = "E";
            else
                logSeverity = "D";

            if (request == null)
                _connection.Logger.Log_Technical("W", string.Format("{0} CallWrapper.{1}:RestReqeuest", Identity.Name, action), "Unable to retrieve request. This generally indicates an error was returned from the 3rd Party.");
            else
            {
                _connection.Logger.Log_Technical(logSeverity, "CallWrapper." + action + ":RestReqeuest", "Resource: " + request.Resource);

                foreach (var param in request.Parameters)
                {
                    if (param.Name != "Authorization" && param.Name != "Content-Type" && param.Name != "Content_Type" && param.Name != "Accept" && param.Name != "Basic")
                        _connection.Logger.Log_Technical(logSeverity, string.Format("{0} CallWrapper.{1}:RestReqeuest", Identity.Name, action), string.Format("Parameter: {0}={1}", param.Name, param.Value));
                }
            }
        }

        private class IntegrationAPIResponse
        {
            public enum ResponseAction
            {
                Continue,
                Error,
                Retry,
                PUTtoPOST //This is the action to take if we attempt to PUT an item, but are told that the item does not exist. In that case, we should remove the ID and POST it
            }

            public ResponseAction action;
            public object data;
            public IntegrationAPIResponseMeta meta;

            //The following fields will be populated on an error:
            public int status;
            public string title;
            public object errors;
        }

        private class IntegrationAPIResponseMeta
        {
            public string type;
            public IntegrationAPIPagination pagination;
        }

        private class IntegrationAPIPagination
        {
            public int? total;
            public int? count;
            public int? per_page;
            public int? current_page;
            public int? total_pages;
            public object links;
            public bool too_many;
        }

    }
}
