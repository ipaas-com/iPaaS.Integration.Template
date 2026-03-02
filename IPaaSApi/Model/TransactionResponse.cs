using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Integration.Data.IPaaSApi.Model
{
    public class TransactionResponse : BaseResponse
    {
        #region Properties
        [JsonProperty("system_id", Order = 10)]
        public long SystemId { get; set; }

        [JsonProperty("transaction_parent_id", Order = 15)]
        public long? ParentId { get; set; }

        [JsonProperty("transaction_number", Order = 20)]
        public string TransactionNumber { get; set; }

        [JsonProperty("customer_id", Order = 22)]
        public long? CustomerId { get; set; }

        [JsonProperty("email_address", Order = 25)]
        public string EmailAddress { get; set; }

        [JsonProperty("type", Order = 30)]
        public string Type { get; set; }

        [JsonProperty("status", Order = 35)]
        public string Status { get; set; }

        [JsonProperty("discount_amount", Order = 40)]
        public decimal DiscountAmount { get; set; }

        [JsonProperty("tax_amount", Order = 45)]
        public decimal TaxAmount { get; set; }

        [JsonProperty("shipping_amount", Order = 50)]
        public decimal ShippingAmount { get; set; }

        [JsonProperty("subtotal", Order = 55)]
        public decimal Subtotal { get; set; }

        [JsonProperty("total", Order = 60)]
        public decimal Total { get; set; }

        [JsonProperty("total_qty", Order = 65)]
        public decimal TotalQty { get; set; }

        [JsonProperty("created_datetime", Order = 70)]
        public DateTimeOffset CreatedDateTime { get; set; }

        [JsonProperty("updated_datetime", Order = 75)]
        public DateTimeOffset? UpdateDateTime { get; set; }

        [JsonProperty("lines", Order = 80)]
        public List<TransactionLineResponse> Lines { get; set; }

        [JsonProperty("taxes", Order = 85)]
        public List<TaxResponse> Taxes { get; set; }

        [JsonProperty("payments", Order = 90)]
        public List<PaymentResponse> Payments { get; set; }

        [JsonProperty("tracking", Order = 95)]
        public List<TrackingResponse> Tracking { get; set; }

        [JsonProperty("addresses", Order = 100)]
        public List<ShippingAddressResponse> Addresses { get; set; }

        [JsonProperty("notes", Order = 105)]
        public List<NoteResponse> Notes { get; set; }
        #endregion

        #region Method(s)
        public override object GetPrimaryId()
        {
            return Convert.ToString(Id);
        }
        #endregion
    }
}
