using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Integration.Data.IPaaSApi.Model
{
    public class CompanyResponse : BaseResponse
    {
        #region Properties
        [JsonProperty("name", Order = 10)]
        public string Name { get; set; }

        [JsonProperty("department", Order = 15)]
        public string Department { get; set; }

        [JsonProperty("description", Order = 20)]
        public string Description { get; set; }

        [JsonProperty("email_address", Order = 25)]
        public string EmailAddress { get; set; }

        [JsonProperty("phone_number", Order = 30)]
        public string PhoneNumber { get; set; }

        [JsonProperty("fax_number", Order = 35)]
        public string FaxNumber { get; set; }

        [JsonProperty("url", Order = 40)]
        public string Url { get; set; }

        [JsonProperty("account_number", Order = 45)]
        public string AccountNumber { get; set; }

        [JsonProperty("tax_id_number", Order = 50)]
        public string TaxIdNumber { get; set; }

        [JsonProperty("parent_company_id", Order = 55)]
        public string ParentCompanyId { get; set; }
        #endregion

        #region Method(s)
        public override object GetPrimaryId()
        {
            return Convert.ToString(Id);
        }
        #endregion
    }
}
