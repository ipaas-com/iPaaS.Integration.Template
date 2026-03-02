using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Integration.Data.IPaaSApi.Model
{
    public class ProductResponse : BaseResponse
    {
        #region Properties
        [JsonProperty("sku", Order = 10)]
        public string Sku { get; set; }

        [JsonProperty("description", Order = 15)]
        public string Description { get; set; }

        [JsonProperty("name", Order = 20)]
        public string Name { get; set; }

        [JsonProperty("type", Order = 25)]
        public string Type { get; set; }

        [JsonProperty("tracking_method", Order = 30)]
        public string TrackingMethod { get; set; }

        [JsonProperty("default_price", Order = 35)]
        public double? DefaultPrice { get; set; }

        [JsonProperty("msrp", Order = 40)]
        public double? Msrp { get; set; }

        [JsonProperty("sale_price", Order = 45)]
        public double? SalePrice { get; set; }

        [JsonProperty("tax_class", Order = 50)]
        public string TaxClass { get; set; }

        [JsonProperty("barcode", Order = 55)]
        public string Barcode { get; set; }

        [JsonProperty("status", Order = 60)]
        public string Status { get; set; }

        [JsonProperty("unit", Order = 65)]
        public string Unit { get; set; }

        [JsonProperty("allow_discounts", Order = 70)]
        public bool? AllowDiscounts { get; set; }

        [JsonProperty("allow_backorders", Order = 75)]
        public bool? AllowBackorders { get; set; }

        [JsonProperty("in_stock_threshold", Order = 80)]
        public double? InStockThreshold { get; set; }

        [JsonProperty("width", Order = 85)]
        public double? Width { get; set; }

        [JsonProperty("height", Order = 90)]
        public double? Height { get; set; }

        [JsonProperty("depth", Order = 95)]
        public double? Depth { get; set; }

        [JsonProperty("weight", Order = 100)]
        public double? Weight { get; set; }

        [JsonProperty("kit", Order = 105)]
        public KitResponse Kit { get; set; }

        [JsonProperty("variants", Order = 110)]
        public List<ProductVariantResponse> Variants { get; set; }

        [JsonProperty("inventory", Order = 115)]
        public List<InventoryResponse> Inventory { get; set; }

        [JsonProperty("categories", Order = 120)]
        public List<CategoryAssignmentResponse> Categories { get; set; }

        [JsonProperty("options", Order = 125)]
        public List<ProductOptionResponse> Options { get; set; }

        [JsonProperty("units", Order = 130)]
        public List<ProductUnitResponse> Units { get; set; }

        [JsonProperty("alternate_ids", Order = 135)]
        public List<ProductAlternateIdResponse> AlternateIds { get; set; }

        [JsonProperty("related_products", Order = 140)]
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
