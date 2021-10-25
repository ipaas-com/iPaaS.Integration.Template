using Integration.Abstract.Helpers;
using Integration.Data.Interface;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Integration.Constants;
using Integration.Data.Utilities;

namespace Integration.DataModels
{
    public class Customer : AbstractIntegrationDataWithCustomFields
    {
        #region Properties
        [JsonProperty("id", NullValueHandling = NullValueHandling.Ignore)]
        public int? Id { get; set; } //id of the customer

        //[JsonProperty("third_party_field_name", NullValueHandling = NullValueHandling.Ignore)]
        //public string ThirdPartyFieldName { get; set; }

        //[JsonIgnore]
        //public string LocalOnlyFieldName { get; set; } 
        #endregion

        #region OperationMethods
        public override object GetPrimaryId()
        {
            return Convert.ToString(Id);
        }

        public override void SetPrimaryId(string PrimaryId, bool ThrowErrorOnInvalid = false)
        {
            int Id_value;
            if (!int.TryParse(PrimaryId, out Id_value))
                HandleInvalidPrimaryId(PrimaryId, ThrowErrorOnInvalid, "Customer");
            else
                Id = Id_value;
        }

        public override async Task<object> Get(CallWrapper activeCallWrapper, object id)
        {
            var apiCall = new APICall(activeCallWrapper, "/urlpath/{id}/", "{{Enter Activity Logger HelpText:}} LOAD Customer(id: " + id + ")", "LOAD Customer (" + id + ")", typeof(Customer), activeCallWrapper.TrackingGuid, TM_MappingCollectionType.CUSTOMER);
            apiCall.AddParameter("Id", id, ParameterType.UrlSegment);
            var output = (Customer)await apiCall.ProcessRequestAsync();
            return output;
        }

        public override async Task<object> Create(CallWrapper activeCallWrapper)
        {
            var apiCall = new APICall(activeCallWrapper, "/urlpath/customers", "{{Enter Activity Logger HelpText:}}", "CREATE Customer", typeof(DataModels.Customer), activeCallWrapper.TrackingGuid, TM_MappingCollectionType.CUSTOMER, Method.POST);

            // Check to see if your external id value can be present.. 
            Id = null;

            apiCall.AddBodyParameter(this);
            var output = (DataModels.Customer)await apiCall.ProcessRequestAsync();

            activeCallWrapper.RegisterClapbackCall((int)TM_MappingCollectionType.CUSTOMER, output.Id);
            return output;
        }

        public override async Task<object> Update(CallWrapper activeCallWrapper)
        {
            var apiCall = new APICall(activeCallWrapper, "/urlpath/{id}/", "{{Enter Activity Logger HelpText:}} " + Convert.ToString(Id) + "/", "UPDATE Customer (" + Convert.ToString(Id) + ")",
                typeof(DataModels.Customer), activeCallWrapper.TrackingGuid, TM_MappingCollectionType.CUSTOMER, Method.PUT);
            apiCall.AddParameter("id", Id, ParameterType.UrlSegment);
            apiCall.AddBodyParameter(this);

            DataModels.Customer output;

            if (apiCall.PUTtoPOST)
                output = (DataModels.Customer)await Create(activeCallWrapper);
            else
                output = (DataModels.Customer)await apiCall.ProcessRequestAsync();

            activeCallWrapper.RegisterClapbackCall((int)TM_MappingCollectionType.CUSTOMER, Id);
            return output;
        }

        public override async Task<object> Delete(CallWrapper activeCallWrapper, object id)
        {
            var Object = (new Customer()).Get(activeCallWrapper, id);

            // Instead of deleting a customer, we are going to unsubscribe them.
            var apiCall = new APICall(activeCallWrapper, "/urlpath/{id}", "{{Enter Activity Logger HelpText:}} {email: " + Convert.ToString(id) + "}", "Unsubscribe Customer (" + Convert.ToString(id) + ")", null, activeCallWrapper.TrackingGuid,
                TM_MappingCollectionType.CUSTOMER, Method.DELETE);

            apiCall.AddParameter("id", Id, ParameterType.UrlSegment);

            await apiCall.ProcessRequestAsync();

            return null;
        }

        #endregion

        #region CustomMethods

        #endregion
    }
}
