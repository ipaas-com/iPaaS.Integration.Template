using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Integration.Data.IPaaSApi.Model
{
    /// <summary>
    /// Represents a tax entry nested within a transaction response.
    /// </summary>
    public class TaxResponse : BaseResponse
    {
        #region Properties
        [JsonProperty("authority", Order = 10)]
        public string Authority { get; set; }

        [JsonProperty("amount", Order = 15)]
        public decimal Amount { get; set; }

        [JsonProperty("orginal_amount", Order = 20)]
        public decimal OrginalAmount { get; set; }

        [JsonProperty("tax_percent", Order = 25)]
        public decimal TaxPercent { get; set; }
        #endregion

        #region Method(s)
        public override object GetPrimaryId()
        {
            return Convert.ToString(Id);
        }
        #endregion
    }
}
