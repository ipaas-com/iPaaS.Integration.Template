using Integration.Data.Interface;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Integration.DataModels
{
    abstract public class AbstractIntegrationData
    {

        [JsonIgnore]
        public int TotalAPICalls = 1;

        public abstract object GetPrimaryId();

        public abstract void SetPrimaryId(string PrimaryId, bool ThrowErrorOnInvalid = false);

        public abstract Task<object> Get(CallWrapper activeCallWrapper, object _id);

        //public abstract Task<List<object>> Initialize(CallWrapper activeCallWrapper);

        public abstract Task<object> Create(CallWrapper activeCallWrapper);

        public abstract Task<object> Update(CallWrapper activeCallWrapper);

        public abstract Task<object> Delete(CallWrapper activeCallWrapper, object _id);

        public void HandleInvalidPrimaryId(string PrimaryId, bool ThrowErrorOnInvalid, string ClassName, string FieldName = "")
        {
            if (ThrowErrorOnInvalid)
                throw new Exception("Invalid data passed to " + ClassName + ".SetPrimaryKey" + (string.IsNullOrEmpty(FieldName) ? "" : "." + FieldName) + ": " + PrimaryId);
            else
                return;
        }
    }
}
