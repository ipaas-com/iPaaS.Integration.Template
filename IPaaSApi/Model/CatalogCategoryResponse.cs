using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Integration.Data.IPaaSApi.Model
{
    /// <summary>
    /// Represents a catalog (product) category returned by v2/Catalog/Category/{id}.
    /// </summary>
    public class CatalogCategoryResponse : BaseResponse
    {
        #region Properties
        [JsonProperty("name", Order = 10)]
        public string Name { get; set; }

        [JsonProperty("description", Order = 15)]
        public string Description { get; set; }

        [JsonProperty("parent_id", Order = 20)]
        public long? ParentId { get; set; }

        [JsonProperty("category_sets", Order = 25)]
        public List<GenericResponse> CategorySets { get; set; }
        #endregion

        #region Method(s)
        public override object GetPrimaryId()
        {
            return Convert.ToString(Id);
        }
        #endregion
    }
}
