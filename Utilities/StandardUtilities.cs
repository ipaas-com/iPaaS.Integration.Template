using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using static Integration.Constants;
using Integration.DataModels;
using Integration.Abstract.Model;

namespace Integration.Data.Utilities
{
    public class StandardUtilities
    {
        public enum ValidatePKDataType_Enum
        {
            integer,
            string_with_one_bar,
            string_with_two_bars
        }

        public static void ValidatePKDataType(ValidatePKDataType_Enum PkDataType, object Id, int MappingCollectionType, string Location)
        {
            if (PkDataType == ValidatePKDataType_Enum.integer)
            {
                int testVal;
                if (!int.TryParse(Convert.ToString(Id), out testVal))
                    throw new Exception("Invalid key value at " + Location + ". MappingCollectionType = " + MappingCollectionType.ToString() + ", ExpectedDataType = Integer, PKValue = " + Convert.ToString(Id));
            }
            else if (PkDataType == ValidatePKDataType_Enum.string_with_one_bar)
            {
                string testVal = Convert.ToString(Id);
                if (testVal.Count(x => x == '|') != 1)
                    throw new Exception("Invalid key value at " + Location + ". MappingCollectionType = " + MappingCollectionType.ToString() + ", ExpectedDataType = String with one separator, PKValue = " + Convert.ToString(Id));
            }
            else if (PkDataType == ValidatePKDataType_Enum.string_with_two_bars)
            {
                string testVal = Convert.ToString(Id);
                if (testVal.Count(x => x == '|') != 2)
                    throw new Exception("Invalid key value at " + Location + ". MappingCollectionType = " + MappingCollectionType.ToString() + ", ExpectedDataType = String with two separator, PKValue = " + Convert.ToString(Id));
            }
        }

        public static void AssignQuotaValues(ResponseObject responseObject, object payload)
        {
            if (payload == null)
                return;

            responseObject.Payload = payload;
            if (payload is AbstractIntegrationData)
            {
                var abstractData = (AbstractIntegrationData)payload;
                responseObject.TotalAPICallsMade = abstractData.TotalAPICalls;
            }
        }
    }
}
