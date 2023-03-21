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
    }
}
