using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Integration.Data.Interface
{
    // Whenever an integration is subscribed to, information that is unique to the customer may be needed by the integration. 
    // Settings defined below will be populated by iPaaS.com at runtime with the corresponding "Preset" value saved by the subscriber.
    // GetSetting method will collect the Preset value defined in the integration console or in MetaData.cs\MetaData\GetPresets().
    public class Settings : Integration.Abstract.Settings
    {
        public string Url { get { return this.GetSetting("API Url", required: true); } }
        public string APIUser { get { return this.GetSetting("API User", required: true); } }
        public string APIPassword { get { return this.GetSetting("API Password", required: true); } }

        public string IPaaSApi_EmployeeUrl 
        { 
            get 
                {   //Depending on the environment, the employee url might be under one of two listings 
                    var retVal = GetSetting("Employees_URL");
                    if(string.IsNullOrEmpty(retVal))
                        retVal = GetSetting("3D4X_URL");
                    return retVal;
                } 
        }

        public string IPaaSApi_MessageUrl
        {
            get
            {   //Depending on the environment, the employee url might be under one of two listings 
                var retVal = GetSetting("Messages_URL");
                if (string.IsNullOrEmpty(retVal))
                    retVal = GetSetting("KUP24_URL");
                return retVal;
            }
        }
    }
}
