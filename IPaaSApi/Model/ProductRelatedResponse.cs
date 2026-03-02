using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Integration.Data.IPaaSApi.Model
{
    /// <summary>
    /// Represents a related product nested within a product or variant response.
    /// </summary>
    public class ProductRelatedResponse : BaseResponse
    {
        #region Properties
        [JsonProperty("product_id", Order = 10)]
        public long ProductId { get; set; }

        [JsonProperty("product_sku", Order = 15)]
        public string ProductSku { get; set; }

        [JsonProperty("is_variant", Order = 20)]
        public bool IsVariant { get; set; }

        [JsonProperty("related_sku", Order = 25)]
        public string RelatedSku { get; set; }

        [JsonProperty("related_id", Order = 30)]
        public long RelatedId { get; set; }

        [JsonProperty("related_type", Order = 35)]
        public string RelatedType { get; set; }
        #endregion

        #region Method(s)
        public override object GetPrimaryId()
        {
            return Convert.ToString(Id);
        }
        #endregion
    }
}
