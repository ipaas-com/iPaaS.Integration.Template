using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Integration.DataModels
{
    public abstract class AbstractIntegrationDataWithCustomFields : AbstractIntegrationData
    {
        [JsonIgnore]
        public List<Field> CustomFields { get; set; }
    }
}
