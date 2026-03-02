using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Integration.Data.IPaaSApi.Model
{
    /// <summary>
    /// Represents the address associated with a transaction line item.
    /// </summary>
    public class TransactionLineAddressResponse
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("shipping_method")]
        public string ShippingMethod { get; set; }

        [JsonProperty("shipping_method_description")]
        public string ShippingMethodDescription { get; set; }

        [JsonProperty("first_name")]
        public string FirstName { get; set; }

        [JsonProperty("last_name")]
        public string LastName { get; set; }

        [JsonProperty("shipping_amount")]
        public double ShippingAmount { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("ship_date")]
        public DateTime? ShipDate { get; set; }

        [JsonProperty("phone_number")]
        public string PhoneNumber { get; set; }

        [JsonProperty("company")]
        public string Company { get; set; }

        [JsonProperty("address_1")]
        public string Address1 { get; set; }

        [JsonProperty("address_2")]
        public string Address2 { get; set; }

        [JsonProperty("address_3")]
        public string Address3 { get; set; }

        [JsonProperty("city")]
        public string City { get; set; }

        [JsonProperty("region")]
        public string Region { get; set; }

        [JsonProperty("country")]
        public string Country { get; set; }

        [JsonProperty("postal_code")]
        public string PostalCode { get; set; }

        [JsonProperty("is_primary_billing")]
        public bool IsPrimaryBilling { get; set; }

        [JsonProperty("is_primary_shipping")]
        public bool IsPrimaryShipping { get; set; }

        [JsonProperty("customer_id")]
        public long CustomerId { get; set; }
    }
}
