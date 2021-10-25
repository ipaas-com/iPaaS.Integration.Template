﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Integration.DataModels;

using static Integration.Constants;
using Integration.Abstract.Helpers;
using Integration.Abstract.Model;
using Integration.Data.Utilities;
using Integration.Data.IPaaSApi;
using Integration.Data;

namespace Integration.Data.Interface
{
    public class TranslationUtilities : Integration.Abstract.TranslationUtilities
    {
        public override string GetPrimaryId(Integration.Abstract.Connection connection, int MappingCollectionType, object SourceObject, long? MappingCollectionId = null)
        {
            if (SourceObject == null)
                throw new Exception(string.Format("Call to GetPrimaryId with unhandled parameters: System={0} {1}, sourceObject is null", Identity.Name, MappingCollectionType));

            // In most cases we use the GetPrimaryId() call in the model class. If there are exceptions, they can be handled here.  
            if (SourceObject is AbstractIntegrationData)
            {
                return Convert.ToString(((AbstractIntegrationData)SourceObject).GetPrimaryId());
            }
            
            // If we make it this far and don't have a matching type, we have an error
            throw new Exception(string.Format("Call to GetPrimaryId with unhandled parameters: System={0}, {1}, sourceObject type={2}.  No matching object", Identity.Name, MappingCollectionType, SourceObject.GetType().Name));
        }

        public override void SetPrimaryId(Integration.Abstract.Connection connection, int MappingCollectionType, object SourceObject, string PrimaryId)
        {
            if (SourceObject is AbstractIntegrationData)
                ((AbstractIntegrationData)SourceObject).SetPrimaryId(PrimaryId);
            else if (SourceObject is ParentOnly) //There is primary id for ParentOnly data, so there is nothing to do here.
                return;
            else
                //if we make it this far and don't have a matching type, we have an error
                throw new Exception(string.Format("Call to SetPrimaryId with unhandled parameters: System={0}, {1}, sourceObject type={2}", Identity.Name, MappingCollectionType, SourceObject.GetType().Name));
        }

        public override object GetDestinationObject(Integration.Abstract.Connection connection, int MappingCollectionType)
        {
            switch ((TM_MappingCollectionType)MappingCollectionType)
            {
                case TM_MappingCollectionType.CUSTOMER:
                    return new Integration.DataModels.Customer();
                default:
                    // Add Error Log Here???
                    throw new Exception(string.Format("Call to GetDestinationObject with unhandled parameters: {0}, systemType: {1}", MappingCollectionType, Identity.Name));
            }
        }

        public override Task InitializeData(Integration.Abstract.Connection connection, int mappingCollectionType)
        {
            // nothing to initialize
            return null;
        }

        public override async Task<ResponseObject> ModelGetAsync(Integration.Abstract.Connection connection, int mappingCollectionType, object id)
        {
            object response = null;

            var conn = (Connection)connection;
            var wrapper = conn.CallWrapper;

            var modelObject = GetDestinationObject(connection, mappingCollectionType);
            if (modelObject == null)
                throw new Exception(string.Format("Call to CollectionGet with unhandled parameters: System={0} {1}, sourceObject could not be created", Identity.Name, mappingCollectionType));

            if (modelObject is AbstractIntegrationData)
            {
                response = await ((AbstractIntegrationData)modelObject).Get(wrapper, id);

                var retVal = new ResponseObject();
                StandardUtilities.AssignQuotaValues(retVal, response);
                return retVal;
            }

            // If we make it this far and don't have a matching type, we have an error
            throw new Exception(string.Format("Call to CollectionGet with unhandled parameters: System={0}, {1}, sourceObject type={2}", Identity.Name, mappingCollectionType, modelObject.GetType().Name));
        }
        public override async Task<ResponseObject> ModelCreateAsync(Integration.Abstract.Connection connection, int mappingCollectionType, object sourceObject, object id)
        {
            object response = null;

            var conn = (Connection)connection;
            var wrapper = conn.CallWrapper;

            if (sourceObject == null)
                throw new Exception(string.Format("Call to CollectionCreate with unhandled parameters: System={0} {1}, sourceObject is null", Identity.Name, mappingCollectionType));

            if (sourceObject is AbstractIntegrationData)
            {
                response = await ((AbstractIntegrationData)sourceObject).Create(wrapper);

                var retVal = new ResponseObject();
                StandardUtilities.AssignQuotaValues(retVal, response);
                return retVal;
            }

            // If we make it this far and don't have a matching type, we have an error
            throw new Exception(string.Format("Call to CollectionCreate with unhandled parameters: System={0}, {1}, sourceObject type={2}", Identity.Name, mappingCollectionType, sourceObject.GetType().Name));
        }
        public override async Task<ResponseObject> ModelUpdateAsync(Integration.Abstract.Connection connection, int mappingCollectionType, object sourceObject, object id)
        {
            object response = null;

            var conn = (Connection)connection;
            var wrapper = conn.CallWrapper;

            if (sourceObject == null)
                throw new Exception(string.Format("Call to CollectionUpdate with unhandled parameters: System={0} {1}, sourceObject is null", Identity.Name, mappingCollectionType));

            // In most cases we use the GetPrimaryId() call in the model class. There are a few exceptions we need to check for first though:
            if (sourceObject is AbstractIntegrationData)
            {
                response = await ((AbstractIntegrationData)sourceObject).Update(wrapper);

                var retVal = new ResponseObject();
                StandardUtilities.AssignQuotaValues(retVal, response);
                return retVal;
            }

            // If we make it this far and don't have a matching type, we have an error
            throw new Exception(string.Format("Call to CollectionUpdate with unhandled parameters: System={0}, {1}, sourceObject type={2}", Identity.Name, mappingCollectionType, sourceObject.GetType().Name));
        }
        public override async Task<ResponseObject> ModelDeleteAsync(Integration.Abstract.Connection connection, int mappingCollectionType, object id)
        {
            var conn = (Connection)connection;
            var wrapper = conn.CallWrapper;

            var Object = GetDestinationObject(connection, mappingCollectionType);
            if (Object == null)
                throw new Exception(string.Format("Call to CollectionDelete with unhandled parameters: System={0} {1}, sourceObject could not be created", Identity.Name, mappingCollectionType));

            // In most cases we use the GetPrimaryId() call in the model class. There are a few exceptions we need to check for first though:
            if (Object is AbstractIntegrationData)
            {
                await ((AbstractIntegrationData)Object).Delete(wrapper, id);

                var retVal = new ResponseObject();
                retVal.TotalAPICallsMade = 1;
                return retVal;
            }

            // If we make it this far and don't have a matching type, we have an error
            throw new Exception(string.Format("Call to CollectionDelete with unhandled parameters: System={0}, {1}, ObjectId={2}", Identity.Name, mappingCollectionType, id));
        }

        public override List<TranslationExternalId> CollectAdditionalExternalIds(Integration.Abstract.Connection connection, int MappingCollectionType, object SourceObject, object DestinationObject)
        {
            var conn = (Connection)connection;
            var wrapper = conn.CallWrapper;

            // Add tasks here

            return null;
        }

        // Some iPaaS objects have child collections which are obtained from seperate 3rd Party end-points.
        // In this case, we want to create an association for each childCollection with the MappingCollectionType.  A childMapping Collection must be created in iPaaS that corresponds to each.
        // If this integration does not have child records, then return an empty collection
        public override List<ChildMapping> GetChildMappings(Integration.Abstract.Connection connection, int MappingCollectionType, long MappingResponseId, int MappingDirection)
        {
            var retVal = new List<ChildMapping>();
            switch ((TM_MappingCollectionType)MappingCollectionType)
            {
                // This example demonstrates how to process the CUSTOMER_CATEGORY ToIpaas and assigning the value to the customerGroup field during the CUSTOMER HookEvent.

                //case TM_MappingCollectionType.CUSTOMER:
                //    retVal.Add(new ChildMapping() { Field = "customerGroup", MappingCollectionType = (int)TM_MappingCollectionType.CUSTOMER_CATEGORY });
                //    break;
                default:
                    break;
            }
            return retVal;
        }
        
        public override object HandlePrerequisite(Integration.Abstract.Connection connection, TransferRequest transferRequest)
        {
            // ===================================================================
            //If there are priority actions to be performed before attempting this HookEvent, Specify to run first here.
            // ===================================================================

            var conn = (Connection)connection;
            var wrapper = conn.CallWrapper;

            // This example demonstrates how to initiate a HookEvent for CUSTOMER_CATEGORY using the value in the customerGroup field before processing the current CUSTOMER HookEvent.

            if (transferRequest.MappingDirection == (int)TM_MappingDirection.TO_IPAAS)
            {
                switch ((TM_MappingCollectionType)transferRequest.MappingCollectionType)
                {
                    // This example demonstrates how to initiate a HookEvent for CUSTOMER_CATEGORY using the value in the customerGroup field before processing the current CUSTOMER HookEvent.
                    // The payload in the Transfer Request would include the body for the requested hook.  Replace: new Dictionary<string, string>()

                    //case TM_MappingCollectionType.CUSTOMER:
                    //            var CustomerBillingAddressRequest = new TransferRequest(connection: conn,
                    //                    mappingCollectionType: (int)TM_MappingCollectionType.CUSTOMER_ADDRESS, mappingDirection: transferRequest.MappingDirection, payload: new Dictionary<string, string>(),
                    //                    scope: "customer/address/created", childRequest: true);

                    //            conn.DataHandlerFunction(CustomerBillingAddressRequest);
                    //    break;
                    default:
                        connection.Logger.Log_Technical("V", string.Format("{0}.TranslationUtiltiies.HandlePrerequisite", Identity.Name), "No prereqs required.");
                        break;
                }
            }
            return null;
        }

        public override object HandlePostActions(Integration.Abstract.Connection connection, TransferRequest transferRequest)
        {
            // ===================================================================
            //If there are secondary actions to be performed upon completion of the current HookEvent, associate it here.
            // ===================================================================

            var conn = (Connection)connection;
            var integration_wrapper = (CallWrapper)conn.CallWrapper;
            var ipaas_wrapper = (IPaaSApiCallWrapper)conn.IPaasApiCallWrapper;

            if (transferRequest.MappingDirection == (int)TM_MappingDirection.TO_IPAAS)
            {
                switch ((TM_MappingCollectionType)transferRequest.MappingCollectionType)
                {
                    // This example demonstrates how to create a customer address Request after completing the customer.
                    // The payload in the Transfer Request would include the body for the requested hook.  Replace: new Dictionary<string, string>()

                    //case TM_MappingCollectionType.CUSTOMER:
                    //            var CustomerBillingAddressRequest = new TransferRequest(connection: conn,
                    //                    mappingCollectionType: (int)TM_MappingCollectionType.CUSTOMER_ADDRESS, mappingDirection: transferRequest.MappingDirection, payload: new Dictionary<string, string>(),
                    //                    scope: "customer/address/created", childRequest: true);

                    //            conn.DataHandlerFunction(CustomerBillingAddressRequest);
                    //            return null;
                    default:
                        connection.Logger.Log_Technical("V", string.Format("{0}.TranslationUtiltiies.HandlePostActions", Identity.Name), "No post actions required.");
                        break;
                }
            }

            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="mappingCollectionType"></param>
        /// <returns></returns>
        public override async Task<ResponseObject> UpdateWebhookSubscriptionAsync(Integration.Abstract.Connection connection, string scope, bool subscribed)
        {
            // If External webhooks are supported in this integration, this is where you handle turning them on

            var retVal = new ResponseObject();
            retVal.TotalAPICallsMade = 0;
            return retVal;
        }

        /// <summary>
        /// Allows an external DLL to estimate how many API calls will be needed for a each CREATE events in order to claim the slots against rate limit settings at the beginning. 
        /// Once the transfer is complete, actual API calls will be compared to the claimed API calls and will adjust the available API call limit up or down as necessary. 
        /// </summary>
        /// <param name="mappingCollectionType"></param>
        /// <returns></returns>
        public new long EstimateTotalAPICallsMade(Integration.Abstract.Connection connection, int mappingCollectionType, object sourceObject)
        {
            // By Default, we estimate 1
            long retVal = 1;

            //switch ((TM_MappingCollectionType)mappingCollectionType)
            //{
            //    // This example demonstrates how to adjust the estimate higher than 1 if certain POST events have multiple calls within their CallWrapper. 
            //    // We always have to serialize and deserialize the sourceObject into the local model before we can access it.
            //    case TM_MappingCollectionType.CUSTOMER:
            //        var sourceObject_JSON = JsonConvert.SerializeObject(sourceObject);
            //        var sourceObject_ExtCustomer = JsonConvert.DeserializeObject<Integration.Data.IPaaSApi.Model.CustomerResponse>(sourceObject_JSON);

            //        //If a collection requires multiple POST calls, in this case 2 each, then calculate it this way
            //        retVal += (sourceObject_ExtCustomer.Options == null ? 0 : sourceObject_ExtCustomer.Options.Count * 2);
            //        break;
            //}

            return retVal;
        }
    }
}
