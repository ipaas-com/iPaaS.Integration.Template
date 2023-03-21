[//https://daringfireball.net/projects/markdown/syntax]: # (Reference this page for Markdown syntax)

# IntegrationTemplate
Provides a template project to assist Integrators with development of new integrations for iPaaS.com.

## BEFORE YOU BEGIN
### Having The Right Expectations

The feeling of success comes when we achieve within our expectations of ourselves.  Having unrealistic expectations will lead to feeling overwhelmed or frustrated.
To that end, let's try to set some proper expectations for developing a new integration.

If you are an experienced developer and are familiar with the iPaaS.com platform and all the concepts within, you can expect to build, upload and test a new integration with one or two flows supported in 4-8 hours.
And in that case, more time will be spent building Development Tests, Documenting and Logging properly and prepraring the integration for certification than the development effort itself.
As developers, we understand that frustration ^^^, but if we are honest with ourselves, we know its necessary and the right thing to do ;)

If this is your first time developing an integration on iPaaS.com, prepare plenty of time without a time sensitive milestone.  We have seen full integrations built in under a week and others that take weeks to get a single flow working.
The length of time to your first integration working depends mostly upon the ease of use and feature-set of the external application and how quickly you digest the iPaaS.com concepts into familiarity.   

What we promise is if you take your time, write your code with pride, and don't take short cuts - the effort will be very rewarding.
Best of luck.. and Skill!

### Prerequisites

Register an account on the iPaaS.com Staging environment.  https://stagingportal.ipaas.com
Request permission to become an integrator.
Once approved, you will be able to register new integrations in the Integrator Experience.

`Important: In order to upload and test a new integration, you will need to reference the Integration Id assigned by iPaaS.com during registration.`


## RESEARCH AND PLANNING

iPaaS.com refers to data flows as having 2 required properties:  
  1. **A MappingCollection** which represents the type of data to be transferred.  E.G. Customer, Product, Transaction, TransactionLine, Etc.
  2. **A Direction** which indicates the direction of data transfer.  All data will either flow "To iPaaS" or "From iPaaS".

`Important: It is recommended to make a list of all the Data Flows the new integration is planning to support.
If this is your first integration project, we recommend choosing a single MappingCollection to integrate initially.`

## SETUP

Create .NET Core 3.1 Class Library project named {{your integration name}}.Data
1. Register Nuget Dependencies
    * ipaas.integration.sdk (latest stable).  The documentation for the abstract methods contained within can be found [iPaaS.com Integration SDK](https://github.com/ipaas-com/iPaaS.Integration.SDK]).
    * NewtonsoftJSON (latest stable)
    * RestSharp (latest stable)

2. Copy the contents of this template file structure to your new project:
    * Helpers
    * Interface
    * Models
    * Utilities
    * Constants.cs

`Important: The Interface files within this Template project are precoded to belong to the Namespace "Integration.Data".
All of the files within the Interface folder should be changed to have a namespace that matches {{your integration name}}.Data`

## Interface Definitions

Please refer to these file definitions as you work to complete your Integration.

**Interface\APICall.cs**  
> Contains a generic class to support all the API Calls which will be performed by your integration with the external application.  
  Use this template to get you started, but it should be modified to accommodate the API documentation provided by the external application provider.

**Interface\CallWrapper.cs**  
> The CallWrapper class will override methods from Integration.Abstract.CallWrapper enabling iPaaS.com to 
  establish a connection and coordinate delivery and collection of information necessary for the accountability of data between systems.

**Interface\Connection.cs**  
> The Connection class will override methods from Integration.Abstract.Connection.  This is the master class for a given connection to a given site.
  Other interface classes will be created as locally-typed instances, such as CallWrapper, Settings, TranslationUtilities, CustomFieldHandler, Etc

**Interface\ConversionFunction.cs**  
> The Connection class enables Integrators to build Static methods for performing routine transformations that are specific to the external system.
  Methods provided here can be accessed by the subscriber from within their UI portal when building mappings.

**Interface\CustomFieldHandler.cs**  
> The CustomField class will override methods from Integration.Abstract.CustomFieldHandler enabling Integrators to build handling for custom fields since custom fields can be implemented differently in each external system.

**Interface\DevelopmentTests.cs**  
> The DevelopmentTests class is only used during integration development testing.  Methods created here will be called by the Integrator Console 
  enabling Integrators during the certification steps to demonstrate that individual flows from the integration are performing as desired.

**Interface\Metadata.cs**  
> The MetaData class will override methods from Integration.Abstract.MetaData enabling Integrators to define meta data about the external systems.  
  Meta data is populated in the marketplace and can also be managed from within the Integrator UI Experience. 

**Interface\Settings.cs**  
> The Settings class will override methods from Integration.Abstract.Settings enabling Integrators to define run-time settings that are unique to each subscriber.

**Interface\TranslationUtilities.cs**  
> The TranslationUtilities class will override methods from Integration.Abstract.TranslationUtilities enabling Integrators to define how operations carried out by iPaaS should perform with the external application.
  Methods like GetDestinationObject() links the MappingCollection to a local data model class.  You can also define Child object relations, Handle scenarios where more than one external id will be present, and also build PreRequisites or PostActions.


## BUILDING

In this section, we will build or modify all the files needed to certify an integration in iPaaS.com.  

### Getting Started

Interface\Metadata.cs

1. Register the new Integration and Namespace with iPaaS Metadata using LoadMetaData() in Interface\Metadata.cs.  Populate all property values.

### Prepare the APICall helper class
The methods in APICall.cs were built to assist with all the API Calls which will be performed by your integration with the external application.  
Use this template to get started, but it should be modified to accommodate the API documentation provided by the external application provider.

1. Apply Authorization in CreateRestRequest(...)
2. Handle Errors in HandleResponse(...)

### Prepare the CallWrapper class
The CallWrapper class will be populated by iPaaS at runtime with information specific to the transfer request to be used by the integration and will return processing information back to iPaaS about how the transfer performed.

1. Modify EstablishConnection(...) to define how iPaaS will establish a connection at runtime.
2. Modify ValidateConnection(...) to define how the external system will confirm for iPaaS that a successful connection has been made.  Validating a connection is typically performed by calling a non-authenticated end-point.

### MappingCollections
Please refer to the information gathered in the Research and Planning section in order to complete the following steps.

For each MappingCollection that will be supported, you will need to build a data model consistent with the external API properties.
Please note that after Data models are built, they will be associated with specific tasks later.  [See translation utilities for more](#translation-utilities).
1. Add properties for each external field 
2. Implement the correct Abstract Inheritence Model (See Template.cs)
    * DataModels.AbstractIntegrationData (see AbstractIntegrationData.cs)
    * DataModels.AbstractIntegrationDataWithCustomFields  (see AbstractIntegrationDataWithCustomFields.cs)
3. Implement Methods:
    * GetPrimaryId(...)
    * SetPrimaryId(...)
    * Get(...)
    * Create(...)
    * Update(...)
    * Delete(...)
4. Register the Data Model with iPaaS Metadata using GetTables() in Interface\Metadata.cs
5. Register the Data Model with GetDestinationObject() in Interface\TranslationUtilities.cs.  This enables iPaaS to work with an empty datamodel prior to transforming data from mappings.

`Important! Not all properties in your Data Models will be mappable in either direction.  If you did not already do so, consider tagging properties like this in your Data Models with [iPaaSIgnore].  This will exclude the registration of these fields with iPaaS Metadata.  Subscribers will NOT be able to create mappings for these fields.`  

### Create Presets and Settings
Whenever an integration is subscribed to, information that is unique to the customer may be needed by the integration to perform certain tasks.  
Integrators can define Setting properties in Settings.cs which will receive the subscribers saved inputs at Runtime. 

1. Register any Preset fields with iPaaS Metadata using GetPresets() in Interface\Metadata.cs.  These fields will presented to the subscribers upon subscribing to the integration and save their unique inputs.
2. Create and associate the local Setting properties with the Preset names so iPaaS can deliver the unique customer values into at runtime, which can be referenced by any runtime methods.

### Custom Field Handling
Build handling for custom fields since they can be implemented differently in each external system.

1. Modify the methods in CustomFieldHandler.cs (This is required if AbstractIntegrationDataWithCustomFields was inherited by any Data Model in your integration)
    * GetValueCustomField(...)
    * SetValueCustomField(...)
    * GetCustomFieldKVPs(...)

### Translation Utilities
Whenever a transfer runtime event occurs, the methods in TranslationUtilties.cs will be executed.  

1. Some of the methods in this Template project have been abstracted into the Data Models, so that the method in TranslationUtilities will call a corresponding method in the Data Model directly such as GetPrimaryId(...), SetPrimaryId(...), ModelGetAsync(...), ModelCreateAsync(...), ModelUpdateAsync(...), and ModelDeleteAsync(...).  
Since you should have already created your Data Models by now, you will be familiar with these corresponding methods.

2. In this Template project, the Method ValidateConnection() in Interface\TranslationUtilities.cs utilizes the ValidateConnection(...) defined in the CallWrapper.  If using this preferred method, there is no update requied.

The remaining steps are optional if they apply to the external system.  (Read more about each method within CallWrapper.cs):

3. Define the Method HandlePrerequisite() in Interface\TranslationUtilities.cs (If applicable)  
4. Define the Method HandlePostActions() in Interface\TranslationUtilities.cs (If applicable)  
5. Define the Method GetChildMappings() in Interface\TranslationUtilities.cs (If applicable)  
6. Define the Method CollectAdditionalExternalIds() in Interface\TranslationUtilities.cs (If applicable)  
7. Define the Method EstimateTotalAPICallsMade() in Interface\TranslationUtilities.cs (If applicable)  
8. Define the Method UpdateWebhookSubscriptionAsync() in Interface\TranslationUtilities.cs (If applicable)  

## Logging Activity with iPaaS.com
iPaaS.com provides an easy way to log technical activity from within your integration at runtime for diagnosing issues, both during testing and post production.

During runtime, iPaaS.com will provide your integration a Connection object that contains a helper class named Logger that contains this method:
    * Log_Technical(string severity, string location, string details)

1. Severity will indicate under what conditions iPaaS.com will record the entry:
    * E = Error (Always record the entry)
    * D = Debug (Only record the entry when running in Debug mode)
    * W = ??
    * V = Verbose (Always record the entry)

2. Location is a personalized text entry that will indicate to you and your subscribers where in the integration the log entry is originating from.  This is typically the Class and Method, but you can choose whatever is most manageable for you.

3. Details is a personalized text entry that describes what is occuring.

These are some examples of how to Log an entry:

> **Example: ??**  
_connection.Logger.Log_Technical("W", string.Format("{0} CallWrapper.{1}", Identity.AppName, "MethodName"), "Calculating Custom Fields: Step 1");  

> **Example: Debug**  
_connection.Logger.Log_Technical("D", string.Format("{0} CallWrapper.{1}", Identity.AppName, "MethodName"), "Custom Fields Found: " & items.count());  

> **Example: Error**  
_connection.Logger.Log_Technical("E", string.Format("{0} CallWrapper.{1}", Identity.AppName, "MethodName"), resp.ErrorException.Message);  


1. Describe Here

## Testing your Integration on iPaaS.com

When you are ready to test your integration with data in your staging environment account, you can use the [iPaaS.com Integration Development Utility](https://github.com/ipaas-com/iPaaS.Integration.DevelopmentUtility)

1. For each Data Flow that you intend to provide support for, you must create a static method in DevelopmentTests.cs to demonstrate each feature.
Here are some guidelines on naming conventions and objectives your tests should meet in order to pass certification.  

    * A
    * B

## Uploading your Integration
When you are ready to upload your file to your staging environment, you can use the [iPaaS.com Integration Development Utility](https://github.com/ipaas-com/iPaaS.Integration.DevelopmentUtility)

## Building Mappings

After you have successfully upload your new Integration:

1. Return to your account portal in the subscriber experience  
2. Subscribe to your new integration from the marketplace  
3. Configure your subscription by entering credentials from your external system sandbox  
4. Build mappings in your subscriber account  

## Building a Webhook Receiver

If the external application you are certifying an integration for provides webhooks, chances are that the webhook format is unique to that system.
iPaaS.com provides a Dynamic Hook receiver, which you can configure to receive webhooks in the format provided by the external application.

1. Return to your account portal in the integrator experience to configure the Webhook Receiver.

## Certifying your Integration

1. Describe Here

Once all testing is completed, we will generate Template mappings in your integration from the subscriber account that we tested on.

