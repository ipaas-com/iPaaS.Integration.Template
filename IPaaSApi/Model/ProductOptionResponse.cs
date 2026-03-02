using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Integration.Data.IPaaSApi.Model
{
    /// <summary>
    /// Represents a product option nested within a product response.
    /// </summary>
    public class ProductOptionResponse : BaseResponse
    {
        #region Properties
        [JsonProperty("option_name", Order = 10)]
        public string OptionName { get; set; }

        [JsonProperty("type", Order = 15)]
        public string Type { get; set; }

        [JsonProperty("order", Order = 20)]
        public int? Order { get; set; }

        [JsonProperty("values", Order = 25)]
        public List<OptionValueResponse> Values { get; set; }
        #endregion

        #region Method(s)
        public override object GetPrimaryId()
        {
            return Convert.ToString(Id);
        }
        #endregion
    }

    /// <summary>
    /// Represents a specific value for a product option.
    /// </summary>
    public class OptionValueResponse : BaseResponse
    {
        #region Properties
        [JsonProperty("option_id", Order = 10)]
        public string OptionId { get; set; }

        [JsonProperty("option_name", Order = 15)]
        public string OptionName { get; set; }

        [JsonProperty("value", Order = 20)]
        public string Value { get; set; }

        [JsonProperty("detail", Order = 25)]
        public string Detail { get; set; }

        [JsonProperty("order", Order = 30)]
        public int? Order { get; set; }
        #endregion

        #region Method(s)
        public override object GetPrimaryId()
        {
            return Convert.ToString(Id);
        }
        #endregion
    }
}
