using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Integration.Data.IPaaSApi.Model
{
    /// <summary>
    /// Represents a single activity/transaction on a gift card, nested within a GiftCardResponse.
    /// </summary>
    public class GiftCardActivityResponse : BaseResponse
    {
        #region Properties
        [JsonProperty("activity_type", Order = 10)]
        public string ActivityType { get; set; }

        [JsonProperty("timestamp", Order = 15)]
        public DateTime Timestamp { get; set; }

        [JsonProperty("location", Order = 20)]
        public string Location { get; set; }

        [JsonProperty("transaction_reference_id", Order = 25)]
        public string TransactionReferenceId { get; set; }

        [JsonProperty("amount", Order = 30)]
        public decimal Amount { get; set; }
        #endregion

        #region Method(s)
        public override object GetPrimaryId()
        {
            return Convert.ToString(Id);
        }
        #endregion
    }
}
