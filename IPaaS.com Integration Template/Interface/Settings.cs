using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Integration.Data.Interface
{
    public class Settings : Integration.Abstract.Settings
    {
        public string Url { get { return this.GetSetting("API Url", required: true); } }
        public string APIUser { get { return this.GetSetting("API User", required: true); } }
        public string APIPassword { get { return this.GetSetting("API Password", required: true); } }
    }
}
