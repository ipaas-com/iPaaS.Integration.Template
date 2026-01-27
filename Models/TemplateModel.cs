using Integration.Abstract.Helpers;
using Integration.Abstract.Model;
using Integration.Data.Interface;
using Integration.Data.Utilities;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Integration.Abstract.Constants;
using static Integration.Constants;

namespace Integration.DataModels
{
    // In this file, we are creating a Data Model to hold the properties to be received from a 3rd Party API Get request or which will be delivered in a Post/Put call.
    // Each Data Model should inherit either AbstractIntegrationData or AbstractIntegrationData or AbstractIntegrationDataWithCustomFields.
    // - Use the latter if the 3rd party object supports a list or dictionary of key/value pairs representing custom fields).

    public class TemplateModel : AbstractIntegrationData
    {
        #region Properties
        [JsonProperty("id", NullValueHandling = NullValueHandling.Ignore)]
        [iPaaSMetaData(Description="This is a unique identifier for the TemplateModel object.", Type=SY_DataType.NUMBER, Required=true)]
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
                HandleInvalidPrimaryId(PrimaryId, ThrowErrorOnInvalid, "Template");
            else
                Id = Id_value;
        }

        public override async Task<object> Get(CallWrapper activeCallWrapper, object id)
        {

            //var apiCall = new APICall(activeCallWrapper, $"/admin/api/{activeCallWrapper?.ApiVersion}/customers/" + id + ".json", "Customer_GET(id: " + id + ")",
            //    "LOAD Customer (" + id + ")", typeof(RequestCustomer), activeCallWrapper?.TrackingGuid,
            //    Constants.TM_MappingCollectionType.CUSTOMER);
            //var output = (RequestCustomer)await apiCall.ProcessRequestAsync();
            //return output.customer;

            throw new NotImplementedException();
        }

        public override async Task<object> Create(CallWrapper activeCallWrapper)
        {
            throw new NotImplementedException();
        }

        public override async Task<object> Update(CallWrapper activeCallWrapper)
        {
            throw new NotImplementedException();
        }

        public override async Task<object> Delete(CallWrapper activeCallWrapper, object id)
        {
            throw new NotImplementedException();
        }

        public override Task<List<BulkTransferRequest>> Poll(CallWrapper activeCallWrapper, string filter)
        {
            throw new NotImplementedException();
        }

        public new List<Features> GetFeatureSupport()
        {

            var retValToIPaaS = new Features();
            retValToIPaaS.MappingCollectionType = (int)TM_MappingCollectionType.PRODUCT;
            retValToIPaaS.MappingDirectionId = (int)TM_MappingDirection.TO_IPAAS;
            retValToIPaaS.Support = Integration.Abstract.Model.Features.SupportLevel.Full;
            retValToIPaaS.AdditionalInformation = "";
            retValToIPaaS.AllowInitialization = false;

            retValToIPaaS.CollisionHandlingSupported = false;
            retValToIPaaS.CustomfieldSupported = true;
            retValToIPaaS.IndependentTransferSupported = true;
            retValToIPaaS.PollingSupported = false;
            retValToIPaaS.RecordMatchingSupported = false;
            retValToIPaaS.ExternalWebhookSupportId = (int)WH_ExternalSupport.FULL_SUPPORT;

            retValToIPaaS.SupportedEndpoints.Add(new FeatureSupportEndpoint() { Value = "/Template/{Id}", Note = "" });

            retValToIPaaS.ExternalIdFormats.Add(new ExternalIdFormat() { RecordExternalIdFormat = "{{Id}}" });

            retValToIPaaS.ExternalDataTypes.Add(new FeatureSupportDataType() { Value = "Template", Note = "The Template table" });

            retValToIPaaS.SupportedMethods.Add((int)TM_SyncType.ADD);
            retValToIPaaS.SupportedMethods.Add((int)TM_SyncType.UPDATE);
            retValToIPaaS.SupportedMethods.Add((int)TM_SyncType.ADD_AND_UPDATE);
            retValToIPaaS.SupportedMethods.Add((int)TM_SyncType.DELETE);
            retValToIPaaS.SupportedMethods.Add((int)TM_SyncType.DELETE_TRIGGERED_UPDATE);

            //If the feature supports mapping FROM_IPAAS, add that here as well

            var retVal = new List<Features>();
            retVal.Add(retValToIPaaS);

            return retVal;
        }
        #endregion

        #region CustomMethods

        #endregion
    }
}
