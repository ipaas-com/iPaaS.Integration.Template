using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Integration.Data.IPaaSApi.Model
{
    public class GiftCardResponse : BaseResponse
    {
        #region Properties
        [JsonProperty("number", Order = 10)]
        public string Number { get; set; }

        [JsonProperty("description", Order = 15)]
        public string Description { get; set; }

        [JsonProperty("type", Order = 20)]
        public string Type { get; set; }

        [JsonProperty("original_amount", Order = 25)]
        public decimal OriginalAmount { get; set; }

        [JsonProperty("balance", Order = 30)]
        public decimal Balance { get; set; }

        [JsonProperty("status", Order = 35)]
        public string Status { get; set; }

        [JsonProperty("purchase_date", Order = 40)]
        public DateTime? PurchaseDate { get; set; }

        [JsonProperty("customer_id", Order = 45)]
        public long? CustomerId { get; set; }

        [JsonProperty("customer_name", Order = 50)]
        public string CustomerName { get; set; }

        [JsonProperty("customer_email", Order = 55)]
        public string CustomerEmail { get; set; }

        [JsonProperty("transaction_id", Order = 60)]
        public long? TransactionId { get; set; }

        [JsonProperty("receiving_customer", Order = 65)]
        public string ReceivingCustomer { get; set; }

        [JsonProperty("receiving_customer_email", Order = 70)]
        public string ReceivingCustomerEmail { get; set; }

        [JsonProperty("system_id", Order = 75)]
        public long? SystemId { get; set; }

        [JsonProperty("expiration_date", Order = 80)]
        public DateTime? ExpirationDate { get; set; }

        [JsonProperty("activities", Order = 85)]
        public List<GiftCardActivityResponse> Activities { get; set; }
        #endregion

        #region Method(s)
        public override object GetPrimaryId()
        {
            return Convert.ToString(Id);
        }
        #endregion
    }
}
