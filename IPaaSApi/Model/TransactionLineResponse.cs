using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Integration.Data.IPaaSApi.Model
{
    public class TransactionLineResponse : BaseResponse
    {
        #region Properties
        [JsonProperty("parent_id", Order = 10)]
        public long? ParentId { get; set; }

        [JsonProperty("sequence_number", Order = 15)]
        public int SequenceNumber { get; set; }

        [JsonProperty("status", Order = 20)]
        public string Status { get; set; }

        [JsonProperty("sku", Order = 25)]
        public string Sku { get; set; }

        [JsonProperty("description", Order = 30)]
        public string Description { get; set; }

        [JsonProperty("type", Order = 35)]
        public string Type { get; set; }

        [JsonProperty("qty", Order = 40)]
        public decimal Qty { get; set; }

        [JsonProperty("qty_shipped", Order = 45)]
        public decimal QtyShipped { get; set; }

        [JsonProperty("unit_price", Order = 50)]
        public decimal UnitPrice { get; set; }

        [JsonProperty("extended_price", Order = 55)]
        public decimal ExtendedPrice { get; set; }

        [JsonProperty("original_unit_price", Order = 60)]
        public decimal OriginalUnitPrice { get; set; }

        [JsonProperty("discount_amount", Order = 65)]
        public decimal DiscountAmount { get; set; }

        [JsonProperty("discount_percent", Order = 70)]
        public decimal DiscountPercent { get; set; }

        [JsonProperty("tax_percent", Order = 75)]
        public decimal TaxPercent { get; set; }

        [JsonProperty("estimated_tax_amount", Order = 80)]
        public decimal EstimatedTaxAmount { get; set; }

        [JsonProperty("weight", Order = 85)]
        public decimal? Weight { get; set; }

        [JsonProperty("address", Order = 90)]
        public TransactionLineAddressResponse Address { get; set; }

        [JsonProperty("line_info", Order = 95)]
        public Dictionary<string, string> LineInfo { get; set; }

        [JsonProperty("options", Order = 100)]
        public Dictionary<string, string> Options { get; set; }
        #endregion

        #region Constructor(s)
        public TransactionLineResponse() { Options = new Dictionary<string, string>(); }
        #endregion

        #region Method(s)
        public override object GetPrimaryId()
        {
            return Convert.ToString(Id);
        }
        #endregion
    }
}
