using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Integration.Data.IPaaSApi.Model
{
    public class ExternalIdResponse
    {
        #region Properties
        [JsonProperty("id", Order = 10)]
        public long Id { get; set; }

        [JsonProperty("system_id", Order = 20)]
        public long SystemId { get; set; }

        [JsonProperty("system_name", Order = 25)]
        public string SystemName { get; set; }

        [JsonProperty("external_id", Order = 30)]
        public string ExternalId { get; set; }

        [JsonProperty("internal_id", Order = 35)]
        public string InternalId { get; set; }
        #endregion
    }
}
