using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Integration.Data.IPaaSApi.Model
{
    public class ProductVariantResponse : BaseResponse
    {
        #region Properties
        [JsonProperty("sku", Order = 10)]
        public string Sku { get; set; }

        [JsonProperty("parent_id", Order = 15)]
        public long ParentId { get; set; }

        [JsonProperty("default_price", Order = 20)]
        public double? DefaultPrice { get; set; }

        [JsonProperty("msrp", Order = 25)]
        public double? Msrp { get; set; }

        [JsonProperty("sale_price", Order = 30)]
        public double? SalePrice { get; set; }

        [JsonProperty("cost", Order = 35)]
        public double? Cost { get; set; }

        [JsonProperty("qty_on_hand", Order = 40)]
        public double? QtyOnHand { get; set; }

        [JsonProperty("qty_available", Order = 45)]
        public double? QtyAvailable { get; set; }

        [JsonProperty("barcode", Order = 50)]
        public string Barcode { get; set; }

        [JsonProperty("status", Order = 55)]
        public string Status { get; set; }

        [JsonProperty("width", Order = 60)]
        public double? Width { get; set; }

        [JsonProperty("height", Order = 65)]
        public double? Height { get; set; }

        [JsonProperty("depth", Order = 70)]
        public double? Depth { get; set; }

        [JsonProperty("weight", Order = 75)]
        public double? Weight { get; set; }

        [JsonProperty("kit", Order = 80)]
        public KitResponse Kit { get; set; }

        [JsonProperty("inventory", Order = 85)]
        public List<InventoryResponse> Inventory { get; set; }

        [JsonProperty("categories", Order = 90)]
        public List<CategoryAssignmentResponse> Categories { get; set; }

        [JsonProperty("options", Order = 95)]
        public List<OptionValueResponse> Options { get; set; }

        [JsonProperty("alternate_ids", Order = 100)]
        public List<ProductAlternateIdResponse> AlternateIds { get; set; }

        [JsonProperty("related_products", Order = 105)]
        public List<ProductRelatedResponse> RelatedProducts { get; set; }
        #endregion

        #region Method(s)
        public override object GetPrimaryId()
        {
            return Convert.ToString(Id);
        }
        #endregion
    }
}
