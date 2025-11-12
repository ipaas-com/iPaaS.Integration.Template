using Integration.Abstract.Helpers;
using Integration.Abstract.Model;
using Integration.DataModels;
using Integration.Template.Utilities;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using static Integration.Constants;

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
            Info.IntegrationFilename = $"{Identity.AppName}.Data.dll";
            Info.IntegrationNamespace = $"{Identity.AppName}.Data.Interface";
            Info.Name = Identity.AppName;
            Info.ApiVersion = "1.0"; // Enter the Integration API Version, not the 3rd-Party Application Version
            Info.VersionMajor = 1;
            Info.VersionMinor = 0;
            Info.VersionPatch = 0;

            Info.OAuthAuthorizationTypeId = (int)ST_SystemTypeAuthorization.NONE; //Set your OAuth type here
            //Info.OAuthUrlTemplate = ""; //Set your OAuth Url Template here, if applicable
            //Info.OAuthIdentifierField = "";//Set your OAuth Identifier field here, if applicable
            //Info.OAuthSuccessCallbackField = ""; //Set your OAuth Success Callback field here, if applicable

            Scopes = GetScopes();
            Presets = GetPresets();
            Tables = GetTables();
            FeatureSupport = GetFeatures();
            WebhookConfigurationSettings = GetWebhookConfiguration();
        }

        private List<Scope> GetScopes()
        {
            Scopes = new List<Integration.Abstract.Model.Scope>();
            //These are some examples of scopes you may want to create
            Scopes.Add(new Scope() { Name = "giftcard/created", Description = "Gift Card Created", MappingCollectionTypeId = (int)TM_MappingCollectionType.GIFT_CARD, ScopeActionId = (int)ScopeAction.CREATED });
            Scopes.Add(new Scope() { Name = "customer/category/initialize", Description = "Initialize customer categories", MappingCollectionTypeId = (int)TM_MappingCollectionType.CUSTOMER_CATEGORY, ScopeActionId = (int)ScopeAction.INITIALIZE });
            Scopes.Add(new Scope() { Name = "product/poll", Description = "Poll the destination system for updated products", MappingCollectionTypeId = (int)TM_MappingCollectionType.PRODUCT, ScopeActionId = (int)ScopeAction.POLL });
            return Scopes;
        }

        private List<Preset> GetPresets()
        {
            var presets = new List<Preset>();
            presets.Add(new Preset() { Name = "API Url", DataType = "string", IsRequired = true, SortOrder = 0, DefaultValue = "www.myapi.com" });
            presets.Add(new Preset() { Name = "API Key", DataType = "string", IsRequired = true, SortOrder = 1 });

            presets.Add(new Preset() { Name = "Multiple Choice Example", DataType = "enum", IsRequired = true, SortOrder = 2,
                PresetValues = new List<PresetValue>() { new PresetValue(){ Value = "Choice A", Description = "A first choice" }, new PresetValue(){ Value = "Option B" } } });

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

        private List<Features> GetFeatures()
        {
            var features = new List<Features>();

            features.Add((new TemplateModel()).GetFeatureSupport());
            
            return features;
        }

        private WebhookConfiguration GetWebhookConfiguration()
        {
            var webhookConfiguration = new WebhookConfiguration();
            webhookConfiguration.HasMultiple = true;
            webhookConfiguration.MultipleSelector_JsonPath = "$.notifications[*]";
            webhookConfiguration.FieldDefinitions = new List<WebhookConfigurationField>();

            var authToken = new WebhookConfigurationField();
            authToken.ValueName = WebhookConfigurationField.ValueName_Enum.AuthToken;
            authToken.RetrievalType = WebhookConfigurationField.RetrievalType_Enum.Header;
            authToken.RetrievalValue = "Authorization";
            authToken.ProcessingType = WebhookConfigurationField.ProcessingType_Enum.Expression;
            authToken.ProcessingValue = "RetrievedValue.ToString().Replace(\"Bearer \",\"\")";
            webhookConfiguration.FieldDefinitions.Add(authToken);

            var scope = new WebhookConfigurationField();
            scope.ValueName = WebhookConfigurationField.ValueName_Enum.Scope;
            scope.RetrievalType = WebhookConfigurationField.RetrievalType_Enum.BodyJsonPath;
            scope.RetrievalValue = "$.scope";
            scope.ProcessingType = WebhookConfigurationField.ProcessingType_Enum.AsIs;
            webhookConfiguration.FieldDefinitions.Add(scope);

            var externalId = new WebhookConfigurationField();
            externalId.ValueName = WebhookConfigurationField.ValueName_Enum.ExternalId;
            externalId.RetrievalType = WebhookConfigurationField.RetrievalType_Enum.BodyJsonPath;
            externalId.RetrievalValue = "$.external_id";
            externalId.ProcessingType = WebhookConfigurationField.ProcessingType_Enum.AsIs;
            webhookConfiguration.FieldDefinitions.Add(externalId);

            return webhookConfiguration;
        }

        /// <summary>
        /// This is a quick and dirty way to populate all fields and properties from a given class. For most fields in most classes, this will be fine,
        /// but there is no sophisticated handling for e.g. JsonIgnore'd fields. So any modifications to the standard output of this method would need to be handled
        /// manually (e.g. you could use the proc to generate a FieldInfo list, then add or remove fields as necessary)
        /// </summary>
        /// <param name="name"></param>
        /// <param name="description"></param>
        /// <param name="mappingCollectionTypeId"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        private TableInfo GenerateTableInfo(string name, string description, int mappingCollectionTypeId, Type type)
        {
            var table = new TableInfo() { Name = name, Description = description, MappingCollectionTypeId = mappingCollectionTypeId };
            table.Fields = new List<Abstract.Model.FieldInfo>();


            var members = new List<System.Reflection.MemberInfo>();
            members.AddRange(type.GetProperties());
            members.AddRange(type.GetFields());

            foreach(var member in members)
            {
                //Do not use iPaaS ignore fields
                if (member.IsDefined(typeof(iPaaSMetaDataAttribute), true))
                    continue;


                var fieldInfo = new Abstract.Model.FieldInfo
                {
                    Name = member.Name,
                    Type = GetFieldType(member) //Add the initial guess at the field type
                };

                if (member.IsDefined(typeof(iPaaSMetaDataAttribute), true))
                {
                    // Check for iPaaSMetaData attribute. Copy the description, required, and type, if present
                    var meta = member.GetCustomAttribute<iPaaSMetaDataAttribute>();
                    if (meta != null)
                    {
                        fieldInfo.Description = meta.Description;
                        if (meta.HasRequired) //We have special flags to indicate whether this was set or not
                            fieldInfo.Required = meta.Required;
                        if (meta.HasType)//We have special flags to indicate whether this was set or not
                            fieldInfo.Type = meta.Type.ToString().ToLower();
                    }
                }

                table.Fields.Add(fieldInfo);
            }
            return table;
        }

        /// <summary>
        /// This only provides a generic guess at the data type. It will correctly identify int, long, bool, DateTime, Guid, and string types. All other types will be returned as "none"
        /// Manually adjust as necessary after generating the FieldInfo list. For example, a password or date field must be manually adjusted.
        /// </summary>
        /// <param name="memberInfo"></param>
        /// <returns></returns>
        private string GetFieldType(MemberInfo memberInfo)
        {
            //Convert the memberInfo int SY_DataType
            Type type;
            if(memberInfo is PropertyInfo property)
                type = property.PropertyType;
            else
                type = ((System.Reflection.FieldInfo)memberInfo).FieldType;

            if (type == typeof(int) || type == typeof(int?) || type == typeof(long) || type == typeof(long?))
                return "number";
            if (type == typeof(bool) || type == typeof(bool?))
                return "bool";
            if (type == typeof(DateTime) || type == typeof(DateTime?))
                return "datetime";
            if (type == typeof(Guid) || type == typeof(Guid?))
                return "guid";
            if (type == typeof(string))
                return "string";
            return "none";
        }
    }
}
