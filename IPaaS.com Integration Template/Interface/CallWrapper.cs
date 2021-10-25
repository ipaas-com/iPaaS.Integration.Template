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

namespace Integration.Data.Interface
{
    [DataContract]
    public class CallWrapper : Integration.Abstract.CallWrapper
    {
        [DataMember]
        public RestClient _integrationClient;
        public Connection _integrationConnection;
        private Settings _integrationSettings;

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

        public new void EstablishConnection(Integration.Abstract.Connection connection, Integration.Abstract.Settings settings)
        {
            _integrationSettings = (Settings)settings;
            _integrationConnection = (Connection)connection;

            _integrationClient = new RestClient(_integrationSettings.APIUser);
            _integrationClient.AddDefaultHeader("Content-Type", "application/json");
            _integrationClient.AddDefaultHeader("Accept", "application/json");
            _integrationClient.Timeout = 90000;

            // To test connectivity, we just request the server-time
            // Use the most appropriate end-point from the Integrated 3rd party API
            RestSharp.RestRequest req = new RestSharp.RestRequest("/v2/server-time", RestSharp.Method.GET);
            req.AddHeader("Authorization", string.Format("Basic {0}", Convert.ToBase64String(Encoding.ASCII.GetBytes(string.Format("{0}:{1}", _integrationSettings.APIUser, _integrationSettings.APIPassword)))));

            RestResponse resp = (RestSharp.RestResponse)_integrationClient.Execute(req);
            if (resp.ErrorException != null)
            {
                _connectionMessage = resp.ErrorException.Message;
                throw new Exception(resp.ErrorException.Message);
            }
            else if (resp.StatusCode != System.Net.HttpStatusCode.OK)
            {
                string errMsg = resp.Content;
                _connectionMessage = errMsg;
                throw new Exception(errMsg);
            }
            else
            {
                _connected = true;
            }
        }

        // This is just a short function to simplify the clapback registration calls. Every call from this class will have the same externalsystemid and direction.
        public void RegisterClapbackCall(int MappingCollectionType, object id)
        {
            // If there is no connection (e.g. we are running this from the Test project), skip this step.
            if (_integrationConnection == null)
                return;

            _integrationConnection.ClapbackTrackerFunction(_integrationConnection.ExternalSystemId, (int)MappingCollectionType, Convert.ToString(id), (int)TM_MappingDirection.FROM_IPAAS);
        }
    }
}