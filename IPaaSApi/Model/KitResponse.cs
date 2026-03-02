using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Integration.Data.IPaaSApi.Model
{
    /// <summary>
    /// Represents a kit nested within a product or variant response.
    /// </summary>
    public class KitResponse : BaseResponse
    {
        #region Properties
        [JsonProperty("sku", Order = 10)]
        public string Sku { get; set; }

        [JsonProperty("description", Order = 15)]
        public string Description { get; set; }

        [JsonProperty("type", Order = 20)]
        public string Type { get; set; }

        [JsonProperty("components", Order = 25)]
        public List<KitComponentResponse> Components { get; set; }
        #endregion

        #region Method(s)
        public override object GetPrimaryId()
        {
            return Convert.ToString(Id);
        }
        #endregion
    }

    /// <summary>
    /// Represents a component item within a kit.
    /// </summary>
    public class KitComponentResponse : BaseResponse
    {
        #region Properties
        [JsonProperty("sku", Order = 10)]
        public string Sku { get; set; }

        [JsonProperty("quantity", Order = 15)]
        public double Quantity { get; set; }

        [JsonProperty("unit", Order = 20)]
        public string Unit { get; set; }

        [JsonProperty("type", Order = 25)]
        public string Type { get; set; }
        #endregion

        #region Method(s)
        public override object GetPrimaryId()
        {
            return Convert.ToString(Id);
        }
        #endregion
    }
}
