using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Integration.Data.IPaaSApi.Model
{
    /// <summary>
    /// Represents a unit of measure for a product nested within a product response.
    /// </summary>
    public class ProductUnitResponse : BaseResponse
    {
        #region Properties
        [JsonProperty("name", Order = 10)]
        public string Name { get; set; }

        [JsonProperty("conversion", Order = 15)]
        public double? Conversion { get; set; }

        [JsonProperty("default_price", Order = 20)]
        public double? DefaultPrice { get; set; }

        [JsonProperty("msrp", Order = 25)]
        public double? Msrp { get; set; }

        [JsonProperty("sale_price", Order = 30)]
        public double? SalePrice { get; set; }
        #endregion

        #region Method(s)
        public override object GetPrimaryId()
        {
            return Convert.ToString(Id);
        }
        #endregion
    }
}
