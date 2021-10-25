using Integration.Data.Interface;
using Integration.Data.IPaaSApi.Model;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Text;
using static Integration.Constants;

namespace Integration.Data.IPaaSApi
{
    public class IPaaSApiCallWrapper : Integration.Abstract.CallWrapper
    {
        private bool _connected = false;
        public override bool Connected { get { return _connected; } }

        private string _connectionMessage;
        public override string ConnectionMessage { get { return _connectionMessage; } }

        public enum EndpointURL
        {
            Customers,
            Giftcards,
            Integrations,
            Products,
            SSO,
            Transactions
        }

        private Connection _connection;
        private Settings _settings;

        public new void EstablishConnection(Integration.Abstract.Connection connection, Integration.Abstract.Settings settings)
        {
            _connection = (Connection)connection;
            _settings = (Settings)settings;
        }

        private RestClient createClient(EndpointURL endpoint)
        {
            string url;
            switch (endpoint)
            {
                case EndpointURL.Customers:
                    url = _settings.IPaaSApi_Customers;
                    break;
                case EndpointURL.Giftcards:
                    url = _settings.IPaaSApi_GiftCards;
                    break;
                case EndpointURL.Integrations:
                    url = _settings.IPaaSApi_Integrations;
                    break;
                case EndpointURL.Products:
                    url = _settings.IPaaSApi_Products;
                    break;
                case EndpointURL.SSO:
                    url = _settings.IPaaSApi_SSO;
                    break;
                case EndpointURL.Transactions:
                    url = _settings.IPaaSApi_Transactions;
                    break;
                default:
                    throw new Exception("Unhandled endpoint type: " + endpoint.ToString());
            }

            var restClient = new RestClient(url);
            restClient.AddDefaultHeader("Content-Type", "application/json");
            restClient.AddDefaultHeader("Content_Type", "application/json");
            restClient.ClearHandlers();
            return restClient;
        }

        private RestRequest createRequest(RestClient client, string url)
        {
            url += "?trackingGuid=" + _settings.TrackingGuid;

            RestSharp.RestRequest req = new RestRequest(url, Method.GET);
            req.RequestFormat = DataFormat.Json;
            //req.JsonSerializer = _jsonSerialser;
            req.AddHeader("Authorization", "Bearer " + _settings.IPaaSApi_Token);
            return req;
        }

        public long? LookupIPaaSId_GET(EndpointURL endpoint, string Id, int MappingCollectionType, long SystemId)
        {
            var client = createClient(endpoint);
            string url = "v2/External/LookupSpecial";

            var tablename = ConvertMappingCollectionTypeToTableName(MappingCollectionType);

            var request = createRequest(client, url);
            var externalIdSpecial = new ExternalIdSpecialRequest() { ExternalId = Id, SystemId = SystemId, TableName = tablename, TrackingGuid = _settings.TrackingGuid };

            string bodyJSON = JsonConvert.SerializeObject(externalIdSpecial, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
            request.AddParameter("application/json", bodyJSON, ParameterType.RequestBody);

            var resp = client.Execute(request);
            var output = resp.Content;

            if (string.IsNullOrEmpty(output) || resp.StatusCode != System.Net.HttpStatusCode.OK)
                return null;
            else
                return Convert.ToInt64(output);
        }

        public string MappingExternalId_GET(EndpointURL endpoint, long Id, int MappingCollectionType, long SystemId)
        {
            var tablename = ConvertMappingCollectionTypeToTableName(MappingCollectionType);

            string URL = "v2/External/LookupExternal/{id}/{systemId}/{tablename}";

            var client = createClient(endpoint);
            var request = createRequest(client, URL);
            var resp = client.Execute(request);
            var output = resp.Content;

            var outputStr = Convert.ToString(output);
            if (outputStr.StartsWith("\"") && outputStr.EndsWith("\""))
                outputStr = outputStr.Substring(1, outputStr.Length - 2);
            return outputStr;
        }

        public ExternalIdResponse MappingExternalId_POST(EndpointURL endpoint, int MappingCollectionType, long SystemId, string ExternalId, long? InternalId)
        {
            var tablename = ConvertMappingCollectionTypeToTableName(MappingCollectionType);
            string action = "MappingSpaceportId_POST(id:" + ExternalId + ",systemId:" + Convert.ToString(SystemId) + ",tablename:" + tablename + ")";
            string action_CustomerFacing = "CREATE iPaaS.com Id";

            var idRequest = new ExternalIdRequest();
            idRequest.TableName = tablename;
            idRequest.SystemId = SystemId;
            idRequest.InternalId = InternalId;
            idRequest.ExternalId = ExternalId;

            string URL = "v2/External/UpdateExternalId";
            var client = createClient(endpoint);
            var request = createRequest(client, URL);
            var resp = client.Execute(request);

            var response = JsonConvert.DeserializeObject<ExternalIdResponse>(resp.Content);
            return response;
        }

        private string ConvertMappingCollectionTypeToTableName(int mappingCollectionType)
        {
            TM_MappingCollectionType type;
            try
            {
                type = (TM_MappingCollectionType)mappingCollectionType;
            }
            catch (Exception ex)
            {
                throw new Exception("Unable to determine table name for mapping collection type " + mappingCollectionType, ex);
            }

            switch (type)
            {
                case TM_MappingCollectionType.PRODUCT_CATEGORY:
                    return "Product Category";
                case TM_MappingCollectionType.CUSTOMER:
                    return "customer";
                case TM_MappingCollectionType.CUSTOMER_ADDRESS:
                    return "Address";
                case TM_MappingCollectionType.CUSTOMER_CATEGORY:
                    return "customer category";
                case TM_MappingCollectionType.GIFT_CARD:
                    return "Gift Card";
                case TM_MappingCollectionType.GIFT_CARD_ACTIVITY:
                    return "Gift Card Activity";
                case TM_MappingCollectionType.LOCATION:
                    return "location";
                case TM_MappingCollectionType.PAYMENT_METHOD:
                    return "Payment Method";
                case TM_MappingCollectionType.PRODUCT:
                case TM_MappingCollectionType.PRODUCT_UNIT:
                    return "product";
                case TM_MappingCollectionType.PRODUCT_INVENTORY:
                    return "product inventory";
                case TM_MappingCollectionType.PRODUCT_OPTION:
                    return "Product Option";
                case TM_MappingCollectionType.PRODUCT_OPTION_VALUE:
                    return "Product Option Value";
                case TM_MappingCollectionType.PRODUCT_VARIANT:
                    return "Product Variant";
                case TM_MappingCollectionType.PRODUCT_VARIANT_INVENTORY:
                    return "Product Variant Inventory";
                case TM_MappingCollectionType.PRODUCT_VARIANT_OPTION:
                    return "Product Variant Option";
                case TM_MappingCollectionType.SHIPPING_METHOD:
                    return "shipping method";
                case TM_MappingCollectionType.SHIPMENT:
                case TM_MappingCollectionType.TRANSACTION:
                    return "transaction";
                case TM_MappingCollectionType.TRANSACTION_ADDRESS:
                    return "Transaction Address"; //There is no CP Transaction Address table (yet)
                case TM_MappingCollectionType.TRANSACTION_LINE:
                    return "Transaction Line";
                case TM_MappingCollectionType.TRANSACTION_PAYMENT:
                    return "transaction payment";
                case TM_MappingCollectionType.TRANSACTION_TAX:
                    return "transaction tax";
                case TM_MappingCollectionType.TRANSACTION_NOTE:
                    return "transaction comment";
                case TM_MappingCollectionType.TRANSACTION_TRACKING_NUMBER:
                    return "transaction tracking";
                case TM_MappingCollectionType.KIT:
                    return "kit";
                case TM_MappingCollectionType.KIT_COMPONENT:
                    return "kit component";
                case TM_MappingCollectionType.ALTERNATE_ID_TYPE:
                    return "alternate id type";
                case TM_MappingCollectionType.PRODUCT_ALTERNATE_ID:
                    return "product alternate id";
                default:
                    throw new Exception("Unable to determine table name for mapping collection type " + type.ToString());
            }
        }
    }
}
