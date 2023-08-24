using Integration.DataModels;
using Integration.Abstract.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace Integration.Data.Interface
{
    static class Identity
    {
        public static string AppName = "Integration";                     // rename this
    }

    public class MetaData : Integration.Abstract.MetaData
    {
        public override void LoadMetaData()
        {
            Info = new Integration.Abstract.Model.IntegrationInfo();
            Info.IntegrationFilename = String.Format("{0}.Data.dll", Identity.AppName);
            Info.IntegrationNamespace = String.Format("{0}.Data.Interface", Identity.AppName);
            Info.Name = Identity.AppName;
            Info.ApiVersion = "1.0"; // Enter the Integration API Version, not the 3rd-Party Application Version
            Info.VersionMajor = 1;
            Info.VersionMinor = 0;

            Scopes = GetScopes();
            Presets = GetPresets();
            Tables = GetTables();
        }

        private List<Scope> GetScopes()
        {
            Scopes = new List<Integration.Abstract.Model.Scope>();
            return Scopes;
        }

        private List<Preset> GetPresets()
        {
            var presets = new List<Preset>();
            presets.Add(new Preset() { Name = "API Url", DataType = "string", IsRequired = true });
            presets.Add(new Preset() { Name = "API Key", DataType = "string", IsRequired = true });
            return presets;
        }

        // Enter 1 for each supported MappingCollection
        // Example: tables.Add(GenerateTableInfo("Customer", "Helptext", (int) Integration.Constants.TM_MappingCollectionType.CUSTOMER, typeof(Customer)));
        private List<TableInfo> GetTables()
        {
            var tables = new List<TableInfo>();
            tables.Add(GenerateTableInfo("Customer", "Helptext", (int)Integration.Constants.TM_MappingCollectionType.CUSTOMER, typeof(TemplateModel)));
            return tables;
        }

        /// <summary>
        /// This is a quick and dirty way to populate all fields and properties from a given class. For most fields in most classes, this will be fine,
        /// but there is no sophisticated handling for e.g. JsonIgnore'd fields. So any modifications to the standard output of this method would need to be handled
        /// manually (e.g. you could use the proc to generate a fieldinfo list, then add or remove fields as necessary)
        /// </summary>
        /// <param name="name"></param>
        /// <param name="description"></param>
        /// <param name="mappingCollectionTypeId"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        private TableInfo GenerateTableInfo(string name, string description, int mappingCollectionTypeId, Type type)
        {
            var table = new TableInfo() { Name = name, Description = description, MappingCollectionTypeId = mappingCollectionTypeId };
            table.Fields = new List<FieldInfo>();

            foreach (var property in type.GetProperties())
            {
                if (!property.IsDefined(typeof(iPaaSIgnore), false) && !property.GetGetMethod().IsVirtual)
                    table.Fields.Add(new FieldInfo() { Name = property.Name });
            }

            foreach (var field in type.GetFields())
            {
                if (!field.IsDefined(typeof(iPaaSIgnore), false))
                    table.Fields.Add(new FieldInfo() { Name = field.Name });
            }
            return table;
        }
    }
}
