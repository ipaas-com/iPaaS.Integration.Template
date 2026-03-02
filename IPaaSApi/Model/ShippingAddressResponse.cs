using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Integration.Data.IPaaSApi.Model
{
    /// <summary>
    /// Represents a shipping/billing address nested within a transaction response.
    /// </summary>
    public class ShippingAddressResponse : BaseResponse
    {
        #region Properties
        [JsonProperty("shipping_method", Order = 10)]
        public string ShippingMethod { get; set; }

        [JsonProperty("shipping_method_description", Order = 15)]
        public string ShippingMethodDescription { get; set; }

        [JsonProperty("first_name", Order = 20)]
        public string FirstName { get; set; }

        [JsonProperty("last_name", Order = 25)]
        public string LastName { get; set; }

        [JsonProperty("shipping_amount", Order = 30)]
        public double ShippingAmount { get; set; }

        [JsonProperty("type", Order = 35)]
        public string Type { get; set; }

        [JsonProperty("ship_date", Order = 40)]
        public DateTime? ShipDate { get; set; }

        [JsonProperty("phone_number", Order = 45)]
        public string PhoneNumber { get; set; }

        [JsonProperty("company", Order = 50)]
        public string Company { get; set; }

        [JsonProperty("address_1", Order = 55)]
        public string Address1 { get; set; }

        [JsonProperty("address_2", Order = 60)]
        public string Address2 { get; set; }

        [JsonProperty("address_3", Order = 65)]
        public string Address3 { get; set; }

        [JsonProperty("city", Order = 70)]
        public string City { get; set; }

        [JsonProperty("region", Order = 75)]
        public string Region { get; set; }

        [JsonProperty("country", Order = 80)]
        public string Country { get; set; }

        [JsonProperty("postal_code", Order = 85)]
        public string PostalCode { get; set; }

        [JsonProperty("is_primary_billing", Order = 90)]
        public bool IsPrimaryBilling { get; set; }

        [JsonProperty("is_primary_shipping", Order = 95)]
        public bool IsPrimaryShipping { get; set; }

        [JsonProperty("customer_id", Order = 100)]
        public long? CustomerId { get; set; }
        #endregion

        #region Method(s)
        public override object GetPrimaryId()
        {
            return Convert.ToString(Id);
        }
        #endregion
    }
}
