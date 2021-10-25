using Integration.Data.IPaaSApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Integration.Data.Interface
{
    public class Connection : Integration.Abstract.Connection
    {
        //  By overwriting the ActiveConnection property of the abstract Connection class, we get a typed connection. And since that includes overridden values
        //  for Settings, call wrappers, etc, we can avoid having to do type conversions throughout the other calls. 

        public new Settings Settings
        {
            get { return (Settings)((Integration.Abstract.Connection)this).Settings; }
            set { ((Integration.Abstract.Connection)this).Settings = value; }
        }

        public new CallWrapper CallWrapper
        {
            get { return (CallWrapper)((Integration.Abstract.Connection)this).CallWrapper; }
            set { ((Integration.Abstract.Connection)this).CallWrapper = value; }
        }

        private IPaaSApiCallWrapper _iPaaSApiCallWrapper;
        public IPaaSApiCallWrapper IPaasApiCallWrapper
        {
            get
            {
                if (_iPaaSApiCallWrapper == null)
                {
                    _iPaaSApiCallWrapper = new IPaaSApiCallWrapper();
                    _iPaaSApiCallWrapper.EstablishConnection((Integration.Abstract.Connection)this, Settings);
                }

                return _iPaaSApiCallWrapper;
            }
        }
    }
}
