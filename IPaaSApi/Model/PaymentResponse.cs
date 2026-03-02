using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Integration.Data.IPaaSApi.Model
{
    /// <summary>
    /// Represents a payment applied to a transaction, nested within a transaction response.
    /// </summary>
    public class PaymentResponse : BaseResponse
    {
        #region Properties
        [JsonProperty("method", Order = 10)]
        public string Method { get; set; }

        [JsonProperty("amount", Order = 15)]
        public decimal Amount { get; set; }

        [JsonProperty("status", Order = 20)]
        public string Status { get; set; }

        [JsonProperty("description", Order = 25)]
        public string Description { get; set; }

        [JsonProperty("method_info", Order = 30)]
        public Dictionary<string, string> MethodInfo { get; set; }
        #endregion

        #region Constructor(s)
        public PaymentResponse() { MethodInfo = new Dictionary<string, string>(); }
        #endregion

        #region Method(s)
        public override object GetPrimaryId()
        {
            return Convert.ToString(Id);
        }
        #endregion
    }
}
