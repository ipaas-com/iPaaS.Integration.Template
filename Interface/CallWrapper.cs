using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using System.Runtime.Serialization;
using static Integration.Constants;
using System.Runtime.Caching;

namespace Integration.Data.Interface
{
    [DataContract]
    public class CallWrapper : Integration.Abstract.CallWrapper
    {
        [DataMember]
        public RestClient _integrationClient;
        public Connection _integrationConnection;
        public Settings _integrationSettings;

        [DataMember]
        private DateTime lastRestRequestCreateDT; // We use this to determine the total time taken for a given request. We log the DT when a rest request is made (the first step of each
                                                  // wrapped call) and then find the difference in the ResponseHandler.
        [DataMember]
        private bool _connected = false;
        public override bool Connected { get { return _connected; } }

        [DataMember]
        private string _connectionMessage;

        public override string ConnectionMessage { get { return _connectionMessage; } }

        public CallWrapper() { }

        public async Task EstablishConnection(Integration.Abstract.Connection connection, Integration.Abstract.Settings settings)
        {
            _integrationSettings = (Settings)settings;
            _integrationConnection = (Connection)connection;

            _integrationClient = new RestClient(_integrationSettings.Url);
            _integrationClient.AddDefaultHeader("Content-Type", "application/json");
            _integrationClient.AddDefaultHeader("Accept", "application/json");
            _integrationClient.UseSerializer(() => new Utilities.RestSharpNewtonsoftSerializer());

            string ValidationResponse = await ValidateConnection();

            if (ValidationResponse != "Success")
            {
                _connectionMessage = ValidationResponse;
                throw new Exception(ValidationResponse);
            }
            else
            {
                _connected = true;
            }
        }

        // This is just a short function to simplify the clapback registration calls. Every call from this class will have the same ExternalSystemId and direction.
        public void RegisterClapbackCall(int MappingCollectionType, object id)
        {
            // If there is no connection (e.g. we are running this from the Test project), skip this step.
            if (_integrationConnection == null)
                return;

            _integrationConnection.ClapbackTrackerFunctionAsync(_integrationConnection.ExternalSystemId, (int)MappingCollectionType, Convert.ToString(id), (int)TM_MappingDirection.FROM_IPAAS);
        }

        public async Task<string> ValidateConnection()
        {
            //To test connectivity, we just request Helloworld or some lightweight equivalent end-point from the 3rd party API.
            RestSharp.RestRequest req = new RestSharp.RestRequest("/v1/helloworld", RestSharp.Method.Get);
            req.AddHeader("Authorization", string.Format("Basic {0}", Convert.ToBase64String(Encoding.ASCII.GetBytes(string.Format("{0}:{1}", _integrationSettings.APIUser, _integrationSettings.APIPassword)))));
            RestResponse resp = (RestResponse)await _integrationClient.ExecuteAsync(req);

            string ResponseVal = "";
            if (resp.ErrorException != null)
            {
                ResponseVal = resp.ErrorException.Message;
            }
            else if (resp.StatusCode != System.Net.HttpStatusCode.OK)
            {
                ResponseVal = resp.Content;
            }
            else
            {
                return "Success";
            }

            return "Unknown Issue Executing ValidateConnection";
        }

        //Some calls, especially TableEntryAsync are expensive and often repeated during the course of a single transfer. We use .NET's built in MemoryCache
        //  feature to store cached values. This cache exists only through the life of a single transfer.
        private static MemoryCache _memoryCache;
        public static void ValidateMemoryCache()
        {
            if (_memoryCache == null)
                _memoryCache = new MemoryCache($"{Identity.AppName}:General");
        }

        public object CheckCache(string cacheKey, string location)
        {
            //Initialize (if necessary) and search for cached data
            ValidateMemoryCache();
            var cachedVal = _memoryCache.Get(cacheKey);

            if (cachedVal != null)
                _integrationConnection.Logger.Logger_Technical("V", location, $"Returning cached value for table data: {cacheKey}");
            return cachedVal;
        }

        public void SaveToCache(string cacheKey, object value)
        {
            ValidateMemoryCache();
            _memoryCache.Add(cacheKey, value, DateTimeOffset.Now.AddSeconds(60));//Add any data returned here to the local cache
        }
    }
}