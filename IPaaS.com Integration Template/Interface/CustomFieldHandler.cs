using Integration.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Integration.Data.Interface
{
    public class CustomFieldHandler : Integration.Abstract.CustomFieldHandler
    {

        public override object GetValueCustomField(object inputObject, string propertyName)
        {
            if(inputObject is AbstractIntegrationDataWithCustomFields)
            {
                var customFields = ((AbstractIntegrationDataWithCustomFields)inputObject).CustomFields;
                if (customFields == null)
                    customFields = new List<Field>();
                var matchingName = customFields.Find(x => x.Key == propertyName);
                
                if (matchingName == null)
                    return null;

                return matchingName.Value;
            }

            return null;

        }

        public override bool SetValueCustomField(object inputObject, string propertyName, object propertyVal)
        {
            if (inputObject is AbstractIntegrationDataWithCustomFields)
            {
                var customFields = ((AbstractIntegrationDataWithCustomFields)inputObject).CustomFields;
                if (customFields == null)
                {
                    ((AbstractIntegrationDataWithCustomFields)inputObject).CustomFields = new List<Field>();
                    customFields = ((AbstractIntegrationDataWithCustomFields)inputObject).CustomFields;
                }
                var customFieldMatch = customFields.Find(x => x.Key == propertyName);
                if (customFieldMatch == null)
                {
                    var customFieldNew = new Integration.DataModels.Field();
                    customFieldNew.Key = propertyName;
                    customFieldNew.Value = Convert.ToString(propertyVal);
                    customFields.Add(customFieldNew);
                }
                else
                    customFieldMatch.Value = Convert.ToString(propertyVal);
                return true;
            }
            else
                return false;
        }

        public override List<KeyValuePair<string, object>> GetCustomFieldKVPs(object inputObject)
        {
            if (inputObject is AbstractIntegrationDataWithCustomFields)
            {
                var customFields = ((AbstractIntegrationDataWithCustomFields)inputObject).CustomFields;
                var retVal = new List<KeyValuePair<string, object>>();
                foreach (var customField in customFields)
                    retVal.Add(new KeyValuePair<string, object>(customField.Key, customField.Value));
                return retVal;
            }
            else
                return null;
        }

    }
}
