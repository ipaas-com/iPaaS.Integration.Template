using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Integration.Data.IPaaSApi.Model
{
    /// <summary>
    /// Represents a tracking entry nested within a transaction response.
    /// </summary>
    public class TrackingResponse : BaseResponse
    {
        #region Properties
        [JsonProperty("shipping_method", Order = 10)]
        public string ShippingMethod { get; set; }

        [JsonProperty("shipping_method_description", Order = 15)]
        public string ShippingMethodDescription { get; set; }

        [JsonProperty("tracking_number", Order = 20)]
        public string TrackingNumber { get; set; }

        [JsonProperty("cost", Order = 25)]
        public double? Cost { get; set; }
        #endregion

        #region Method(s)
        public override object GetPrimaryId()
        {
            return Convert.ToString(Id);
        }
        #endregion
    }
}
