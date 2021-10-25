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
    public class ConversionFunctions : Integration.Abstract.ConversionFunctions 
    {
        private static Connection ContextConnection { get { return (Connection)Integration.Abstract.ConversionFunctions.ContextConnection; } }
        private static Settings IntegrationSettings { get { return (Settings)ContextConnection.Settings; } }
        private static CallWrapper IntegrationCallWrapper { get { return (CallWrapper)ContextConnection.CallWrapper; } }

        [ThreadStatic]
        private static IPaaSApiCallWrapper _iPaaSApiCallWrapper;
        public static IPaaSApiCallWrapper iPaaSApiCallWrapper
        {
            get
            {
                if (_iPaaSApiCallWrapper == null)
                    _iPaaSApiCallWrapper = new IPaaSApiCallWrapper();
                return _iPaaSApiCallWrapper;
            }
        }
    }
}
