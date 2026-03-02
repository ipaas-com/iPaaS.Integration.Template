using Integration.Data.Interface;
using Integration.Data.IPaaSApi.Model;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using static Integration.Constants;

namespace Integration.Data.IPaaSApi
{
    /// <summary>
    /// A call wrapper for making API calls to the iPaaS API. This is used by the external dll to make calls to iPaaS.
    /// It also allows us to centralize our error handling for iPaaS API calls in one place.
    ///
    /// Method naming convention: {Entity}_{Action}Async
    ///   Examples: Customer_GetByIdAsync, Product_SearchBySkuAsync, ExternalId_LookupSpecialAsync
    /// </summary>
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
            Integrators,
            Products,
            SSO,
            Subscriptions,
            Transactions,
            Employees,
            Messages,
            Hooks
        }

        private Connection _connection;
        private Settings _settings;

        public new void EstablishConnection(Integration.Abstract.Connection connection, Integration.Abstract.Settings settings)
        {
            _connection = (Connection)connection;
            _settings = (Settings)settings;
        }

        private RestClient CreateClient(EndpointURL endpoint)
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
                case EndpointURL.Integrators:
                    url = _settings.IPaaSApi_Integrators;
                    break;
                case EndpointURL.Products:
                    url = _settings.IPaaSApi_Products;
                    break;
                case EndpointURL.SSO:
                    url = _settings.IPaaSApi_SSO;
                    break;
                case EndpointURL.Subscriptions:
                    url = _settings.IPaaSApi_Subscriptions;
                    break;
                case EndpointURL.Transactions:
                    url = _settings.IPaaSApi_Transactions;
                    break;
                case EndpointURL.Employees:
                    url = _settings.IPaaSApi_EmployeeUrl;
                    break;
                case EndpointURL.Messages:
                    url = _settings.IPaaSApi_MediaUrl;
                    break;
                default:
                    throw new Exception("Unhandled endpoint type: " + endpoint.ToString());
            }

            var restClient = new RestClient(url);
            restClient.AddDefaultHeader("Content-Type", "application/json");
            restClient.AddDefaultHeader("Content_Type", "application/json");
            restClient.UseSerializer(() => new Utilities.RestSharpNewtonsoftSerializer());
            return restClient;
        }

        private RestRequest createRequest(RestClient client, string url)
        {
            //If we have a tracking guid add it. We need to check if the url already has urlparams. If it does, we add an & rather than a ?
            if (_settings.TrackingGuid != Guid.Empty)
                url += (url.Contains("?") ? "&" : "?") + "trackingGuid=" + _settings.TrackingGuid;

            RestSharp.RestRequest req = new RestRequest(url, Method.Get);
            req.RequestFormat = DataFormat.Json;
            req.AddHeader("Authorization", "Bearer " + _settings.IPaaSApi_Token);
            return req;
        }

        //Check if the iPaaS response indicates the call failed or not. If it failed, we throw an error.
        private void HandleResponse(RestSharp.RestResponse resp, string action, bool notFoundIsError = true)
        {
            if ((resp.ErrorException != null && resp.StatusCode != System.Net.HttpStatusCode.NotFound) || (resp.StatusCode == System.Net.HttpStatusCode.NotFound && notFoundIsError))
            {
                string errMsg = ProcessFullErrorMessage(resp);
                _connection.Logger.Log_ActivityTracker("Recieved ErrorException from externalSystem's iPaaSCallWrapper." + action + ". See Tech log for more details", "Failed API Call to iPaaS (via external dll)", "Error", 0);
                _connection.Logger.Log_TechnicalException($"ExternalSystem.IPaaSCallWrapper.{action}:ErrorException", resp.ErrorException);
                _connection.Logger.Log_Technical("E", $"ExternalSystem.IPaaSCallWrapper.{action}:ErrorMessage", errMsg);
                throw new Exception(errMsg);
            }
            else if (resp.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                //If the status code is not found, we don't want to throw an exception, but we do want to log it.
                string errMsg = ProcessFullErrorMessage(resp).Replace("Error:", "");
                _connection.Logger.Log_ActivityTracker("Recieved NotFound from externalSystem's iPaaSCallWrapper." + action + ". This is not necessarliy an error", "API Call to iPaaS (via external dll) returned NotFound", "Info", 0);
                _connection.Logger.Log_Technical("D", $"ExternalSystem.IPaaSCallWrapper.{action}:NotFound", errMsg + ". This is not necessarily an error");
            }
            else
            {
                _connection.Logger.Log_ActivityTracker("Recieved Ok from externalSystem's iPaaSCallWrapper." + action + "", "API Call to iPaaS (via external dll) was succesful", "Verbose", 0);
                _connection.Logger.Log_Technical("D", $"ExternalSystem.IPaaSCallWrapper.{action}:Success", "Successful call");
            }
        }

        /// <summary>
        /// Convert the RestResponse into a fully parsed, human readable error message.
        /// </summary>
        private string ProcessFullErrorMessage(RestSharp.RestResponse resp)
        {
            string errMsg = "Error:";
            if (!string.IsNullOrEmpty(resp.ErrorMessage))
                errMsg += " " + resp.ErrorMessage;

            if (!string.IsNullOrEmpty(resp.Content))
                errMsg += " " + resp.Content;

            // This is where Coy likes to hide his errors
            if (!string.IsNullOrEmpty(resp.StatusDescription))
                errMsg += " " + resp.StatusDescription;

            errMsg += " (Http Code: " + resp.StatusCode.ToString() + ")";
            return errMsg;
        }

        // =====================================================================
        // External ID Mapping Methods
        // =====================================================================

        /// <summary>
        /// Looks up an iPaaS internal ID given an external ID.
        /// Calls POST v2/External/LookupSpecial (or v1/ for Employees).
        /// Returns null if not found.
        /// </summary>
        public async Task<string> ExternalId_LookupSpecialAsync(EndpointURL endpoint, string externalId, int mappingCollectionType, long systemId, bool allowDeleted = false)
        {
            var client = CreateClient(endpoint);
            string url = "v2/External/LookupSpecial";
            if (endpoint == EndpointURL.Employees)
                url = "v1/External/LookupSpecial";

            var tablename = ConvertMappingCollectionTypeToTableName(mappingCollectionType);

            var request = createRequest(client, url);
            request.Method = Method.Post;
            var externalIdSpecial = new ExternalIdSpecialRequest() { ExternalId = externalId, SystemId = systemId, TableName = tablename, TrackingGuid = _settings.TrackingGuid };

            string bodyJSON = JsonConvert.SerializeObject(externalIdSpecial, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
            request.AddParameter("application/json", bodyJSON, ParameterType.RequestBody);

            var resp = await client.ExecuteAsync(request);
            //Check if the response is an error. NotFound will be returned if the data is not linked, so we need to set notFoundIsError = false
            HandleResponse(resp, "ExternalId_LookupSpecialAsync", notFoundIsError: false);

            var output = resp.Content;

            if (string.IsNullOrEmpty(output) || resp.StatusCode != System.Net.HttpStatusCode.OK)
                return null;

            //Remove quotes, if there are any
            if (output.StartsWith("\"") && output.EndsWith("\""))
            {
                output = output.Substring(1, output.Length - 2);

                //The output has come to us formatted with escape chars as literals. (e.g. 30" Waist would show up as "30\" Waist")
                output = System.Text.RegularExpressions.Regex.Unescape(output);
            }

            //Deleted external IDs need to be handled here
            if (output.StartsWith("DELETED:"))
            {
                if (allowDeleted)
                    return output.Substring(8);
                else
                    return null;
            }

            return output;
        }

        /// <summary>
        /// Looks up an external ID given an iPaaS internal ID.
        /// Calls GET v2/External/LookupExternal/{id}/{systemId}/{tablename} (or v1/ for Employees).
        /// Returns null if not found.
        /// </summary>
        public async Task<string> ExternalId_LookupExternalAsync(EndpointURL endpoint, string id, int mappingCollectionType, long systemId, bool allowDeleted = false)
        {
            var tablename = ConvertMappingCollectionTypeToTableName(mappingCollectionType);

            string URL = "v2/External/LookupExternal/{id}/{systemId}/{tablename}";
            if (endpoint == EndpointURL.Employees)
                URL = "v1/External/LookupExternal/{id}/{systemId}/{tablename}";

            var client = CreateClient(endpoint);
            var request = createRequest(client, URL);
            request.AddParameter("id", id, ParameterType.UrlSegment);
            request.AddParameter("systemId", systemId, ParameterType.UrlSegment);
            request.AddParameter("tablename", tablename, ParameterType.UrlSegment);

            var resp = await client.ExecuteAsync(request);
            //Check if the response is an error. NotFound will be returned if the data is not linked, so we need to set notFoundIsError = false
            HandleResponse(resp, "ExternalId_LookupExternalAsync", notFoundIsError: false);

            var output = resp.Content;

            //if there is no matching id, resp will have a status code of NotFound
            if (string.IsNullOrEmpty(output) || resp.StatusCode != System.Net.HttpStatusCode.OK)
                return null;

            var outputStr = Convert.ToString(output);
            if (outputStr.StartsWith("\"") && outputStr.EndsWith("\""))
                outputStr = outputStr.Substring(1, outputStr.Length - 2);

            //If the response indicates that we are returning deleted data, what we return depends on if we allow deleted data or not.
            //  For delete hook lookups, we do allow deleted data, but other calls do not.
            if (outputStr.StartsWith("DELETED:"))
            {
                if (allowDeleted)
                    outputStr = output.Substring(8);
                else
                    outputStr = null;
            }

            return outputStr;
        }

        /// <summary>
        /// Creates or updates an external ID mapping in iPaaS.
        /// Calls POST v2/External/UpdateExternalId (or v1/ for Employees).
        /// </summary>
        public async Task<ExternalIdRequest> ExternalId_UpdateAsync(EndpointURL endpoint, int mappingCollectionType, long systemId, string externalId, string internalId)
        {
            var tablename = ConvertMappingCollectionTypeToTableName(mappingCollectionType);

            var idRequest = new ExternalIdRequest();
            idRequest.TableName = tablename;
            idRequest.SystemId = systemId;
            idRequest.InternalId = internalId;
            idRequest.ExternalId = externalId;

            string URL = "v2/External/UpdateExternalId";
            if (endpoint == EndpointURL.Employees)
                URL = "v1/External/UpdateExternalId";

            var client = CreateClient(endpoint);
            var request = createRequest(client, URL);
            request.Method = Method.Post;
            string bodyJSON = JsonConvert.SerializeObject(idRequest, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
            request.AddParameter("application/json", bodyJSON, ParameterType.RequestBody);

            var resp = await client.ExecuteAsync(request);
            //Check if the response is an error.
            HandleResponse(resp, "ExternalId_UpdateAsync");

            var response = JsonConvert.DeserializeObject<ExternalIdRequest>(resp.Content);
            return response;
        }

        /// <summary>
        /// Looks up an iPaaS internal ID by the iPaaS system ID using the Spaceport lookup.
        /// Calls GET v2/External/LookupSpaceport/{systemId}/{tablename}/{id}.
        /// Returns null if not found.
        /// </summary>
        public async Task<string> ExternalId_LookupSpaceportAsync(EndpointURL endpoint, string id, int mappingCollectionType, long systemId, bool allowDeleted = false)
        {
            var tablename = ConvertMappingCollectionTypeToTableName(mappingCollectionType);

            string URL = "v2/External/LookupSpaceport/{systemId}/{tablename}/{id}";

            var client = CreateClient(endpoint);
            var request = createRequest(client, URL);
            request.AddParameter("id", id, ParameterType.UrlSegment);
            request.AddParameter("systemId", systemId, ParameterType.UrlSegment);
            request.AddParameter("tablename", tablename, ParameterType.UrlSegment);

            var resp = await client.ExecuteAsync(request);
            //Check if the response is an error. NotFound will be returned if the data is not linked, so we need to set notFoundIsError = false
            HandleResponse(resp, "ExternalId_LookupSpaceportAsync", notFoundIsError: false);

            var output = resp.Content;

            //if there is no matching id, resp will have a status code of NotFound
            if (string.IsNullOrEmpty(output) || resp.StatusCode != System.Net.HttpStatusCode.OK)
                return null;

            var outputStr = Convert.ToString(output);
            if (outputStr.StartsWith("\"") && outputStr.EndsWith("\""))
                outputStr = outputStr.Substring(1, outputStr.Length - 2);

            //If the response indicates that we are returning deleted data, what we return depends on if we allow deleted data or not.
            //  For delete hook lookups, we do allow deleted data, but other calls do not.
            if (outputStr.StartsWith("DELETED:"))
            {
                if (allowDeleted)
                    outputStr = output.Substring(8);
                else
                    outputStr = null;
            }

            return outputStr;
        }

        // =====================================================================
        // Integration / System Type Methods
        // =====================================================================

        /// <summary>
        /// Gets an integration (system type) by its ID.
        /// Calls GET v2/Integration/{id}.
        /// </summary>
        public async Task<SystemTypeResponse> Integration_GetByIdAsync(long id)
        {
            var client = CreateClient(EndpointURL.Subscriptions);
            var request = createRequest(client, "v2/Integration/{id}");
            request.AddParameter("id", id, ParameterType.UrlSegment);

            var resp = await client.ExecuteAsync(request);
            HandleResponse(resp, "Integration_GetByIdAsync", notFoundIsError: false);

            if (string.IsNullOrEmpty(resp.Content) || resp.StatusCode != System.Net.HttpStatusCode.OK)
                return null;

            return JsonConvert.DeserializeObject<SystemTypeResponse>(resp.Content);
        }

        // =====================================================================
        // Customer Methods
        // =====================================================================

        /// <summary>
        /// Gets a customer by their iPaaS ID.
        /// Calls GET v2/Customer/{id}.
        /// </summary>
        public async Task<CustomerResponse> Customer_GetByIdAsync(long id)
        {
            var client = CreateClient(EndpointURL.Customers);
            var request = createRequest(client, "v2/Customer/{id}");
            request.AddParameter("id", id, ParameterType.UrlSegment);

            var resp = await client.ExecuteAsync(request);
            HandleResponse(resp, "Customer_GetByIdAsync", notFoundIsError: false);

            if (string.IsNullOrEmpty(resp.Content) || resp.StatusCode != System.Net.HttpStatusCode.OK)
                return null;

            return JsonConvert.DeserializeObject<CustomerResponse>(resp.Content);
        }

        /// <summary>
        /// Searches for customers by email address.
        /// Calls GET v2/Customers?action=equals&EmailAddress={email}.
        /// </summary>
        public async Task<List<CustomerResponse>> Customer_SearchByEmailAsync(string email)
        {
            var client = CreateClient(EndpointURL.Customers);
            var request = createRequest(client, $"v2/Customers?action=equals&EmailAddress={Uri.EscapeDataString(email)}");

            var resp = await client.ExecuteAsync(request);
            HandleResponse(resp, "Customer_SearchByEmailAsync", notFoundIsError: false);

            if (string.IsNullOrEmpty(resp.Content) || resp.StatusCode != System.Net.HttpStatusCode.OK)
                return null;

            return JsonConvert.DeserializeObject<List<CustomerResponse>>(resp.Content);
        }

        /// <summary>
        /// Gets a customer category definition by its iPaaS ID.
        /// Calls GET v2/Customer/Category/{id}.
        /// </summary>
        public async Task<CustomerCategoryResponse> CustomerCategory_GetByIdAsync(string id)
        {
            var client = CreateClient(EndpointURL.Customers);
            var request = createRequest(client, "v2/Customer/Category/{id}");
            request.AddParameter("id", id, ParameterType.UrlSegment);

            var resp = await client.ExecuteAsync(request);
            HandleResponse(resp, "CustomerCategory_GetByIdAsync", notFoundIsError: false);

            if (string.IsNullOrEmpty(resp.Content) || resp.StatusCode != System.Net.HttpStatusCode.OK)
                return null;

            return JsonConvert.DeserializeObject<CustomerCategoryResponse>(resp.Content);
        }

        /// <summary>
        /// Gets a customer company by its iPaaS ID.
        /// Calls GET v2/Customer/Company/{id}.
        /// </summary>
        public async Task<CompanyResponse> Company_GetByIdAsync(string id)
        {
            var client = CreateClient(EndpointURL.Customers);
            var request = createRequest(client, "v2/Customer/Company/{id}");
            request.AddParameter("id", id, ParameterType.UrlSegment);

            var resp = await client.ExecuteAsync(request);
            HandleResponse(resp, "Company_GetByIdAsync", notFoundIsError: false);

            if (string.IsNullOrEmpty(resp.Content) || resp.StatusCode != System.Net.HttpStatusCode.OK)
                return null;

            return JsonConvert.DeserializeObject<CompanyResponse>(resp.Content);
        }

        /// <summary>
        /// Searches for customer companies. Pass a filter string such as "?action=equals&Name=Acme" or leave empty to return all.
        /// Calls GET v2/Customer/Companies{filter}.
        /// </summary>
        public async Task<List<CompanyResponse>> Company_SearchAsync(string filter = "")
        {
            var client = CreateClient(EndpointURL.Customers);
            var request = createRequest(client, "v2/Customer/Companies" + filter);

            var resp = await client.ExecuteAsync(request);
            HandleResponse(resp, "Company_SearchAsync", notFoundIsError: false);

            if (string.IsNullOrEmpty(resp.Content) || resp.StatusCode != System.Net.HttpStatusCode.OK)
                return null;

            return JsonConvert.DeserializeObject<List<CompanyResponse>>(resp.Content);
        }

        // =====================================================================
        // Catalog / Product Methods
        // =====================================================================

        /// <summary>
        /// Gets a product by its iPaaS ID.
        /// Calls GET v2/Catalog/Product/{id}.
        /// </summary>
        public async Task<ProductResponse> Product_GetByIdAsync(string id)
        {
            var client = CreateClient(EndpointURL.Products);
            var request = createRequest(client, "v2/Catalog/Product/{id}");
            request.AddParameter("id", id, ParameterType.UrlSegment);

            var resp = await client.ExecuteAsync(request);
            HandleResponse(resp, "Product_GetByIdAsync", notFoundIsError: false);

            if (string.IsNullOrEmpty(resp.Content) || resp.StatusCode != System.Net.HttpStatusCode.OK)
                return null;

            return JsonConvert.DeserializeObject<ProductResponse>(resp.Content);
        }

        /// <summary>
        /// Searches for products by SKU.
        /// Calls GET v2/Catalog/Products?action=equals&Sku={sku}.
        /// </summary>
        public async Task<List<ProductResponse>> Product_SearchBySkuAsync(string sku)
        {
            var client = CreateClient(EndpointURL.Products);
            var request = createRequest(client, $"v2/Catalog/Products?action=equals&Sku={Uri.EscapeDataString(sku)}");

            var resp = await client.ExecuteAsync(request);
            HandleResponse(resp, "Product_SearchBySkuAsync", notFoundIsError: false);

            if (string.IsNullOrEmpty(resp.Content) || resp.StatusCode != System.Net.HttpStatusCode.OK)
                return null;

            return JsonConvert.DeserializeObject<List<ProductResponse>>(resp.Content);
        }

        /// <summary>
        /// Gets a product variant by its iPaaS ID.
        /// Calls GET v2/Catalog/Product/Variant/{id}.
        /// </summary>
        public async Task<ProductVariantResponse> ProductVariant_GetByIdAsync(string id)
        {
            var client = CreateClient(EndpointURL.Products);
            var request = createRequest(client, "v2/Catalog/Product/Variant/{id}");
            request.AddParameter("id", id, ParameterType.UrlSegment);

            var resp = await client.ExecuteAsync(request);
            HandleResponse(resp, "ProductVariant_GetByIdAsync", notFoundIsError: false);

            if (string.IsNullOrEmpty(resp.Content) || resp.StatusCode != System.Net.HttpStatusCode.OK)
                return null;

            return JsonConvert.DeserializeObject<ProductVariantResponse>(resp.Content);
        }

        /// <summary>
        /// Searches for product variants by SKU.
        /// Calls GET v2/Catalog/Product/Variants?action=equals&Sku={sku}.
        /// </summary>
        public async Task<List<ProductVariantResponse>> ProductVariant_SearchBySkuAsync(string sku)
        {
            var client = CreateClient(EndpointURL.Products);
            var request = createRequest(client, $"v2/Catalog/Product/Variants?action=equals&Sku={Uri.EscapeDataString(sku)}");

            var resp = await client.ExecuteAsync(request);
            HandleResponse(resp, "ProductVariant_SearchBySkuAsync", notFoundIsError: false);

            if (string.IsNullOrEmpty(resp.Content) || resp.StatusCode != System.Net.HttpStatusCode.OK)
                return null;

            return JsonConvert.DeserializeObject<List<ProductVariantResponse>>(resp.Content);
        }

        /// <summary>
        /// Gets a catalog category by its iPaaS ID.
        /// Calls GET v2/Catalog/Category/{id}.
        /// </summary>
        public async Task<CatalogCategoryResponse> CatalogCategory_GetByIdAsync(string id)
        {
            var client = CreateClient(EndpointURL.Products);
            var request = createRequest(client, "v2/Catalog/Category/{id}");
            request.AddParameter("id", id, ParameterType.UrlSegment);

            var resp = await client.ExecuteAsync(request);
            HandleResponse(resp, "CatalogCategory_GetByIdAsync", notFoundIsError: false);

            if (string.IsNullOrEmpty(resp.Content) || resp.StatusCode != System.Net.HttpStatusCode.OK)
                return null;

            return JsonConvert.DeserializeObject<CatalogCategoryResponse>(resp.Content);
        }

        /// <summary>
        /// Gets a location by its iPaaS ID.
        /// Calls GET v2/Catalog/Location/{id}.
        /// </summary>
        public async Task<LocationResponse> Location_GetByIdAsync(string id)
        {
            var client = CreateClient(EndpointURL.Products);
            var request = createRequest(client, "v2/Catalog/Location/{id}");
            request.AddParameter("id", id, ParameterType.UrlSegment);

            var resp = await client.ExecuteAsync(request);
            HandleResponse(resp, "Location_GetByIdAsync", notFoundIsError: false);

            if (string.IsNullOrEmpty(resp.Content) || resp.StatusCode != System.Net.HttpStatusCode.OK)
                return null;

            return JsonConvert.DeserializeObject<LocationResponse>(resp.Content);
        }

        /// <summary>
        /// Searches for locations. Pass a filter string such as "?action=equals&Name=Main" or leave empty to return all.
        /// Calls GET v2/Catalog/Locations{filter}.
        /// </summary>
        public async Task<List<LocationResponse>> Location_SearchAsync(string filter = "")
        {
            var client = CreateClient(EndpointURL.Products);
            var request = createRequest(client, "v2/Catalog/Locations" + filter);

            var resp = await client.ExecuteAsync(request);
            HandleResponse(resp, "Location_SearchAsync", notFoundIsError: false);

            if (string.IsNullOrEmpty(resp.Content) || resp.StatusCode != System.Net.HttpStatusCode.OK)
                return null;

            return JsonConvert.DeserializeObject<List<LocationResponse>>(resp.Content);
        }

        // =====================================================================
        // Transaction Methods
        // =====================================================================

        /// <summary>
        /// Gets a transaction by its iPaaS ID.
        /// Calls GET v2/Transaction/{id}.
        /// </summary>
        public async Task<TransactionResponse> Transaction_GetByIdAsync(string id)
        {
            var client = CreateClient(EndpointURL.Transactions);
            var request = createRequest(client, "v2/Transaction/{id}");
            request.AddParameter("id", id, ParameterType.UrlSegment);

            var resp = await client.ExecuteAsync(request);
            HandleResponse(resp, "Transaction_GetByIdAsync", notFoundIsError: false);

            if (string.IsNullOrEmpty(resp.Content) || resp.StatusCode != System.Net.HttpStatusCode.OK)
                return null;

            return JsonConvert.DeserializeObject<TransactionResponse>(resp.Content);
        }

        /// <summary>
        /// Searches for transactions. Pass a filter string such as "?action=equals&TransactionNumber=1234" or leave empty.
        /// Calls GET v2/Transactions{filter}.
        /// </summary>
        public async Task<List<TransactionResponse>> Transaction_SearchAsync(string filter = "")
        {
            var client = CreateClient(EndpointURL.Transactions);
            var request = createRequest(client, "v2/Transactions" + filter);

            var resp = await client.ExecuteAsync(request);
            HandleResponse(resp, "Transaction_SearchAsync", notFoundIsError: false);

            if (string.IsNullOrEmpty(resp.Content) || resp.StatusCode != System.Net.HttpStatusCode.OK)
                return null;

            return JsonConvert.DeserializeObject<List<TransactionResponse>>(resp.Content);
        }

        /// <summary>
        /// Gets all payment methods.
        /// Calls GET v2/PaymentMethods.
        /// </summary>
        public async Task<List<PaymentMethodResponse>> PaymentMethod_GetAllAsync()
        {
            var client = CreateClient(EndpointURL.Transactions);
            var request = createRequest(client, "v2/PaymentMethods");

            var resp = await client.ExecuteAsync(request);
            HandleResponse(resp, "PaymentMethod_GetAllAsync");

            if (string.IsNullOrEmpty(resp.Content) || resp.StatusCode != System.Net.HttpStatusCode.OK)
                return null;

            return JsonConvert.DeserializeObject<List<PaymentMethodResponse>>(resp.Content);
        }

        /// <summary>
        /// Searches for shipping methods. Pass a filter string such as "?action=equals&Name=UPS" or leave empty to return all.
        /// Calls GET v2/ShippingMethods{filter}.
        /// </summary>
        public async Task<List<ShippingMethodResponse>> ShippingMethod_SearchAsync(string filter = "")
        {
            var client = CreateClient(EndpointURL.Transactions);
            var request = createRequest(client, "v2/ShippingMethods" + filter);

            var resp = await client.ExecuteAsync(request);
            HandleResponse(resp, "ShippingMethod_SearchAsync", notFoundIsError: false);

            if (string.IsNullOrEmpty(resp.Content) || resp.StatusCode != System.Net.HttpStatusCode.OK)
                return null;

            return JsonConvert.DeserializeObject<List<ShippingMethodResponse>>(resp.Content);
        }

        // =====================================================================
        // Gift Card Methods
        // =====================================================================

        /// <summary>
        /// Gets a gift card by its iPaaS ID.
        /// Calls GET v2/GiftCard/{id}.
        /// </summary>
        public async Task<GiftCardResponse> GiftCard_GetByIdAsync(string id)
        {
            var client = CreateClient(EndpointURL.Giftcards);
            var request = createRequest(client, "v2/GiftCard/{id}");
            request.AddParameter("id", id, ParameterType.UrlSegment);

            var resp = await client.ExecuteAsync(request);
            HandleResponse(resp, "GiftCard_GetByIdAsync", notFoundIsError: false);

            if (string.IsNullOrEmpty(resp.Content) || resp.StatusCode != System.Net.HttpStatusCode.OK)
                return null;

            return JsonConvert.DeserializeObject<GiftCardResponse>(resp.Content);
        }

        // =====================================================================
        // Helper Methods
        // =====================================================================

        /// <summary>
        /// Maps a TM_MappingCollectionType value to the appropriate EndpointURL.
        /// </summary>
        public static EndpointURL MappingCollectionTypeToEndpointURL(int mappingCollectionType)
        {
            switch ((TM_MappingCollectionType)mappingCollectionType)
            {
                case TM_MappingCollectionType.ALTERNATE_ID_TYPE:
                case TM_MappingCollectionType.CATALOG_CATEGORY_SET:
                case TM_MappingCollectionType.KIT:
                case TM_MappingCollectionType.KIT_COMPONENT:
                case TM_MappingCollectionType.LOCATION:
                case TM_MappingCollectionType.LOCATION_GROUP:
                case TM_MappingCollectionType.PRODUCT:
                case TM_MappingCollectionType.PRODUCT_ALTERNATE_ID:
                case TM_MappingCollectionType.PRODUCT_CATEGORY:
                case TM_MappingCollectionType.PRODUCT_CATEGORY_ASSIGNMENT:
                case TM_MappingCollectionType.PRODUCT_INVENTORY:
                case TM_MappingCollectionType.PRODUCT_OPTION:
                case TM_MappingCollectionType.PRODUCT_OPTION_VALUE:
                case TM_MappingCollectionType.PRODUCT_UNIT:
                case TM_MappingCollectionType.PRODUCT_VARIANT:
                case TM_MappingCollectionType.PRODUCT_VARIANT_CATEGORY_ASSIGNMENT:
                case TM_MappingCollectionType.PRODUCT_VARIANT_INVENTORY:
                case TM_MappingCollectionType.PRODUCT_VARIANT_OPTION:
                case TM_MappingCollectionType.PRODUCT_VARIANT_OPTION_VALUE:
                    return EndpointURL.Products;
                case TM_MappingCollectionType.CUSTOMER:
                case TM_MappingCollectionType.CUSTOMER_ADDRESS:
                case TM_MappingCollectionType.CUSTOMER_CATEGORY:
                case TM_MappingCollectionType.CUSTOMER_CONTACT:
                    return EndpointURL.Customers;
                case TM_MappingCollectionType.PAYMENT_METHOD:
                case TM_MappingCollectionType.SHIPMENT:
                case TM_MappingCollectionType.SHIPMENT_LINE:
                case TM_MappingCollectionType.SHIPPING_METHOD:
                case TM_MappingCollectionType.TRANSACTION:
                case TM_MappingCollectionType.TRANSACTION_ADDRESS:
                case TM_MappingCollectionType.TRANSACTION_DISCOUNT:
                case TM_MappingCollectionType.TRANSACTION_LINE:
                case TM_MappingCollectionType.TRANSACTION_NOTE:
                case TM_MappingCollectionType.TRANSACTION_TAX:
                case TM_MappingCollectionType.TRANSACTION_PAYMENT:
                case TM_MappingCollectionType.TRANSACTION_TRACKING_NUMBER:
                    return EndpointURL.Transactions;
                case TM_MappingCollectionType.GIFT_CARD:
                case TM_MappingCollectionType.GIFT_CARD_ACTIVITY:
                    return EndpointURL.Giftcards;
                default:
                    throw new Exception("MappingCollectionTypeToEndpointURL: Unsupported type - " + mappingCollectionType.ToString());
            }
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
                    return "Transaction Address";
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
