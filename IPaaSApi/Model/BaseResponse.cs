using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Integration.Data.IPaaSApi.Model
{
    public class BaseResponse
    {
        [JsonProperty("id", Order = 1)]
        public long Id { get; set; }

        [JsonProperty("tracking_guid", Order = 5)]
        public Guid? TrackingGuid { get; set; }

        [JsonProperty("custom_fields", Order = 980)]
        public List<GenericCustomFieldResponse> CustomFields { get; set; }

        [JsonProperty("external_ids", Order = 999)]
        public List<GenericExternalIdResponse> ExternalIds { get; set; }

        public virtual object GetPrimaryId()
        {
            throw new NotImplementedException("GetPrimaryId was not implemented for this Response method");
        }
    }
}
