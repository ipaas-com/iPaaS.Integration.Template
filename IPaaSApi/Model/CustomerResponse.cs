using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Integration.Data.IPaaSApi.Model
{
    public class CustomerResponse : BaseResponse
    {
        #region Properties
        [JsonProperty("customer_number", Order = 5)]
        public string CustomerNumber { get; set; }

        [JsonProperty("first_name", Order = 10)]
        public string FirstName { get; set; }

        [JsonProperty("last_name", Order = 15)]
        public string LastName { get; set; }

        [JsonProperty("email_address", Order = 20)]
        public string EmailAddress { get; set; }

        [JsonProperty("company", Order = 25)]
        public string Company { get; set; }

        [JsonProperty("comment", Order = 30)]
        public string Comment { get; set; }

        [JsonProperty("category_id", Order = 35)]
        public long? CategoryId { get; set; }

        [JsonProperty("created_datetime", Order = 40)]
        public DateTimeOffset CreatedDateTime { get; set; }

        [JsonProperty("updated_datetime", Order = 45)]
        public DateTimeOffset? UpdateDateTime { get; set; }

        #endregion

        #region Constructor(s)
        #endregion

        #region Method(s)
        public override object GetPrimaryId()
        {
            return Convert.ToString(Id);
        }
        #endregion
    }
}
