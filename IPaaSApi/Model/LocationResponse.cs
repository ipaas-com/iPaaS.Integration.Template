using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Integration.Data.IPaaSApi.Model
{
    public class LocationResponse : BaseResponse
    {
        #region Properties
        [JsonProperty("name", Order = 10)]
        public string Name { get; set; }

        [JsonProperty("description", Order = 15)]
        public string Description { get; set; }

        [JsonProperty("location_groups", Order = 20)]
        public object LocationGroups { get; set; }
        #endregion

        #region Method(s)
        public override object GetPrimaryId()
        {
            return Convert.ToString(Id);
        }
        #endregion
    }
}
