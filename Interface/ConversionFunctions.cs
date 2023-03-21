using Integration.Data.IPaaSApi;
using Integration.Data.IPaaSApi.Model;
using Integration.Data.Utilities;
using Integration.DataModels;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Integration.Constants;

namespace Integration.Data.Interface
{
    // Static methods can be used for performing routine transformations that are specific to the external system.
    // Methods provided here can be accessed by the subscriber from within their UI portal when building mappings.
    public class ConversionFunctions : Integration.Abstract.ConversionFunctions 
    {
        [ThreadStatic]
        private static IPaaSApiCallWrapper _iPaaSApiCallWrapper;
        public static IPaaSApiCallWrapper iPaaSApiCallWrapper
        {
            get
            {
                if (_iPaaSApiCallWrapper == null)
                    _iPaaSApiCallWrapper = new IPaaSApiCallWrapper();
                _iPaaSApiCallWrapper.EstablishConnection(ContextConnection, ContextConnection.Settings);
                return _iPaaSApiCallWrapper;
            }
        }

        private static new Connection ContextConnection { get { return (Connection)Integration.Abstract.ConversionFunctions.ContextConnection; } }
        private static Settings IntegrationSettings { get { return (Settings)ContextConnection.Settings; } }
        private static CallWrapper IntegrationCallWrapper { get { return (CallWrapper)ContextConnection.CallWrapper; } }
    }
}
