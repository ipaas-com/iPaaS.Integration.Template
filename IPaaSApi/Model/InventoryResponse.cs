using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Integration.Data.IPaaSApi.Model
{
    /// <summary>
    /// Represents inventory data nested within a product or variant response.
    /// </summary>
    public class InventoryResponse : BaseResponse
    {
        #region Properties
        [JsonProperty("parent_id", Order = 10)]
        public long? ParentId { get; set; }

        [JsonProperty("location_id", Order = 15)]
        public string LocationId { get; set; }

        [JsonProperty("qty_on_hand", Order = 20)]
        public double? QtyOnHand { get; set; }

        [JsonProperty("qty_available", Order = 25)]
        public double? QtyAvailable { get; set; }

        [JsonProperty("cost", Order = 30)]
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
