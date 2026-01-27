using Integration.Abstract.Helpers;
using Integration.Abstract.Model;
using Integration.Data.Interface;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Integration.DataModels
{
    // All Data Models should inherit from AbstractIntegrationData.

    abstract public class AbstractIntegrationData : System.Attribute
    {
        // =====================================================
        // The properties are used by iPaaS.com to track and
        // assist subscribers with throttling across the
        // integration ecosystem.

        [iPaaSIgnore]
        [JsonIgnore]
        public long QuotaResponseRequestsLeft;

        [iPaaSIgnore]
        [JsonIgnore]
        public long QuotaResponseRequestQuota;

        [iPaaSIgnore]
        [JsonIgnore]
        public long QuotaResponseResetMS;

        [iPaaSIgnore]
        [JsonIgnore]
        public long QuotaResponseWindowMS;

        [iPaaSIgnore]
        [JsonIgnore]
        public int TotalAPICalls = 1;

        // =====================================================
        // Do not modify.  This is used by the system.attribute
        // inheritance enabling [iPaaSIgnore]

        [JsonIgnore]
        public override object TypeId { get { return base.TypeId; } }

        // =====================================================
        // Override this method to retrieve the Primary Key
        // of the external object when retrieving data
        // from iPaaS.com

        public abstract object GetPrimaryId();

        // =====================================================
        // Override this method to retrieve the Primary Key of
        // the external object when sending data to iPaaS.com

        public abstract void SetPrimaryId(string PrimaryId, bool ThrowErrorOnInvalid = false);

        // =====================================================
        // Override these methods to perform API calls for
        // that Mapping Collection type

        public abstract Task<object> Get(CallWrapper activeCallWrapper, object _id);

        public abstract Task<object> Create(CallWrapper activeCallWrapper);

        public abstract Task<object> Update(CallWrapper activeCallWrapper);

        public abstract Task<object> Delete(CallWrapper activeCallWrapper, object _id);

        public abstract Task<List<BulkTransferRequest>> Poll(CallWrapper activeCallWrapper, string filter);

        //A virtual method that can be overridden by child classes to indicate what features are supported by this data model. This can be called from the MetaData class
        //to determine what features are supported by the data model.
        public virtual List<Features> GetFeatureSupport()
        {
            return null;
        }

        // =====================================================
        // Use this helper function to Error on Primary Key issues

        public void HandleInvalidPrimaryId(string PrimaryId, bool ThrowErrorOnInvalid, string ClassName, string FieldName = "")
        {
            if (ThrowErrorOnInvalid)
                throw new Exception("Invalid data passed to " + ClassName + ".SetPrimaryKey" + (string.IsNullOrEmpty(FieldName) ? "" : "." + FieldName) + ": " + PrimaryId);
            else
                return;
        }

        // =====================================================
    }

    // =====================================================
    // DO NOT MODIFY
    // The iPaaSIgnore attribute is used to exclude properties from iPaaS.com metadata.
    // Any property tagged with this attribute will be excluded when executing MetaData.cs\GenerateTableInfo during upload.

    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, Inherited = false, AllowMultiple = false)]
    public class iPaaSIgnore : Attribute
    {
        //... Do Nothing
    }
}
