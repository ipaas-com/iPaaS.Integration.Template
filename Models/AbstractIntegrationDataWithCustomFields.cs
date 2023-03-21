using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Integration.DataModels
{
    // if the 3rd party object supports a list or dictionary of key/value pairs representing custom fields),
    // We will use the CustomFields property to normalize those provided by the 3rd party.
    // When organizing the data in this manner, iPaaS.com provides users the ability for CustomField handling and mapping within their subscription settings and formulas.

    // If you perform inheritence of AbstractIntegrationDataWithCustomFields, you must develop handlers in CustomFields.cs
    public abstract class AbstractIntegrationDataWithCustomFields : AbstractIntegrationData
    {
        [JsonIgnore]
        public List<Field> CustomFields { get; set; }
    }
}
