using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Integration.Data.IPaaSApi.Model
{
    /// <summary>
    /// Represents an alternate ID for a product or variant nested within a product/variant response.
    /// </summary>
    public class ProductAlternateIdResponse : BaseResponse
    {
        #region Properties
        [JsonProperty("sku", Order = 10)]
        public string Sku { get; set; }

        [JsonProperty("unit", Order = 15)]
        public string Unit { get; set; }

        [JsonProperty("alternate_id_type_id", Order = 20)]
        public long? AlternateIdTypeId { get; set; }

        [JsonProperty("alternate_id", Order = 25)]
        public string AlternateId { get; set; }

        [JsonProperty("description", Order = 30)]
        public string Description { get; set; }
        #endregion

        #region Method(s)
        public override object GetPrimaryId()
        {
            return Convert.ToString(Id);
        }
        #endregion
    }
}
