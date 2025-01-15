# IntegrationTemplate
The following provides a template project to assist iPaaS.com Integrators with development of new integrations for the iPaaS.com platform.

# BEFORE YOU BEGIN 

## Overview

iPaaS.com is a queue-based Integration Platform as a Service designed to increase accountability and transparency of data between APIs.

To achieve this, iPaaS.com divides the traditional Systems Integrator role into two primary roles with an accountability structure around each that enables delegation of responsibility:
-   Integrator: Uses domain expertise in a third-party software ant it’s API to integrate that technology with iPaaS.com.
-   Managed integration Service Providers (MiSP): Provide implementation service functions by those who have general knowledge about operational objectives across many technologies.

The MiSP program is designed to empower No-Code/Low-Code operators trained in iPaaS.com a standardized way to facilitate ongoing and bespoke requirements across many technologies without having to establish a relationship between Integrators and MiSPs directly and without having to become domain experts in a particular technology. iPaaS.com subscribers (customers) can engage companies they are familiar with and have a trusted relationship to aid them with ongoing management of their integration ecosystem in a framework with which they are already trained on.

The intention of the Integrator SDK aims to disconnect the ongoing maintenance and management responsibilities of the Integrators from the customer. The SDK guides Integrators in building an integration to support many different operational requirements while not making “hard coding” decisions. The Integrator will aim to build tooling to aid both Subscribers and MiSPs including: 
-   Connections to external systems
-   Supporting documentation 
-   Template mappings 
-   Real-time activity logging for routine maintenance. 

The final version of the Integration is certified in a fully reusable way.

## Having The Right Expectations

Having unrealistic expectations will lead to feeling overwhelmed or frustrated. To that end, let's try to set some proper expectations for developing a new integration.

If you are an experienced developer and are familiar with the iPaaS.com platform and all the concepts within, you can expect to build, upload and test a new integration with a minimal amount of time. Once those flows are in place, more time will be spent building development tests, building out default mapping templates, writing custom conversion functions, documenting and logging properly, and preparing the integration for certification than the development effort itself. 

If this is your first time developing an integration on iPaaS.com, prepare plenty of time without a time-sensitive milestone. We have seen full integrations built in under a week and others that take weeks to get a single flow working. The length of time to your first integration working depends mostly upon the ease of use and feature set of the external application and how quickly you digest the iPaaS.com concepts into familiarity.

## Prerequisites

An account on the iPaaS.com Staging environment with integrator permissions. To obtain access to these features, please schedule a call or reach out to partnerships@ipaas.com to request to join our Integrator Partnership Program. You will need to accept the terms of our Integrator partnership program prior to receiving access to our Integrator features.

Please note that Managed integration Service Provider (MiSP), Tech, and other partners are not automatically Integrator Partners. Such partners still need to agree to the terms related to writing integrations for our Marketplace before being provided with Integrator permissions in our platform. 

Additionally, please be aware that our staging environment is at a different URL than the production environment where MiSP partner accounts and sandboxes, and Subscriber accounts and sandboxes are found. 

If you’ve already signed an Integrator agreement, but are having trouble accessing these features on the iPaaS.com staging environment (stagingportal.ipaas.com), please specify that when reaching out to partnerships@ipaas.com and let us know if you’ve already tried resetting your passwords at https://stagingportal.ipaas.com/auth/forget. We’ll be able to verify the users in your organization that have access, and will follow appropriate security protocols to verify any new users.

Important: To upload and test a new integration, you will need to reference the Integration ID assigned by iPaaS.com during registration and login with the credentials used to register the integration.

## Learn more about iPaaS.com

The following information about the platform may be helpful as a reference when you are building your integration.

### Data Orchestration and Flow

iPaaS.com is a queue-based, hub-and-spoke, rules-based integration platform that performs translations in near real-time. It is NOT architected as an API Proxy that brokers between one API and another. When data is intended to transfer inbound (To iPaaS.com), the external system will notify iPaaS.com via a webhook that data was created/changed/deleted.

The webhook contains the information necessary for iPaaS.com to identify which subscriber the data is being processed for, the type of data (Product, Customer, Etc.) and the unique identifier for the data record. iPaaS.com does not handle any other data in the webhook payload.

Webhooks are received by iPaaS.com and queued for processing. iPaaS.com enables subscribers to override standard rate-limits and other settings that could influence the order of operations. This data is evaluated when processing the queue.
Once an item is selected to be processed from the queue, it is assigned a Tracking Id as well as other meta-data that will facilitate the accountable transfer of data between systems.

iPaaS.com normalizes data into standardized Data Modules. External system data Inbound (To iPaaS.com) will be transformed from the raw JSON returned by the Integration using subscriber-defined business rules called Mapping Collections which can be modified through the UI.

When data is modified in iPaaS.com, either by originating through a certified Integration or via the iPaaS.com API, iPaaS.com will initiate orchestration of all outbound data (From iPaaS.com) to the waiting system.
Subscribers can enable/disable which Data should move between systems.

### Data Modules and MappingCollections

Data Modules are a collection of data models typically representing an Industry focus.
For example, commerce will use a collection of data models surrounding Products, Inventory, Customers, Transactions, Gift Cards, and more.

These modules are how subscribers will access data in transit and can be dynamically altered via the UI by the subscriber or MiSP to include custom fields.

All fields, including custom fields, can be mapped to as a destination value for any data being transferred To iPaaS.com. Data Models cannot be created by Subscribers or Integrators.

Each Data Model has a corresponding MappingCollection name which it will be related to by the Integration.

## iPaaS.com Processing Engine Order of Events


  ![Integration Process Flow (To iPaaS)](https://cms.ipaas.com/sites/default/files/2023-04/iPaaSExternalDLLProcessFlow-ToiPaaS_0.png "Integration Process Flow (To iPaaS)")
  
  ![Integration Process Flow (From iPaaS)](https://cms.ipaas.com/sites/default/files/2023-04/iPaaSExternalDLLProcessFlow-FromiPaaS_0.png "Integration Process Flow (From iPaaS)")


## RESEARCH AND PLANNING

iPaaS.com refers to data flows as having 2 required properties:

1.  A MappingCollection which represents the type of data to be transferred. E.G. Customer, Product, Transaction, TransactionLine, Etc.
2.  A Direction that indicates the direction of data transfer. All data will either flow "To iPaaS.com" or "From iPaaS.com".

Important: It is recommended to make a list of all the Data Flows the new integration is planning to support. If this is your first integration project, we recommend choosing a single MappingCollection to integrate initially.
SETUP
Create .NET Core 3.1 Class Library project named {{your integration name}}.Data

1. Register NuGet Dependencies
     * ipaas.integration.sdk (latest stable). The documentation for the abstract methods contained within can be found iPaaS.com Integration SDK.
     * NewtonsoftJSON (latest stable)
     * RestSharp (latest stable)
2. Copy the contents of this template file structure to your new project:
     * Helpers
     * Interface
     * Models
     * Utilities
     * Constants.cs

`Important: The Interface files within this Template project are precoded to belong to the Namespace "Integration.Data". All of the files within the Interface folder should be changed to have a namespace that matches {{your integration name}}.Data`

## Interface Definitions

Please refer to these file definitions as you work to complete your Integration.

### Interface\APICall.cs

Contains a generic class to support all the API Calls which will be performed by your integration with the external application.

Use this template to get you started, but it should be modified to accommodate the API documentation provided by the external application provider.

### Interface\CallWrapper.cs

The CallWrapper class will override methods from Integration.Abstract.CallWrapper enabling iPaaS.com to establish a connection and coordinate delivery and collection of information necessary for the accountability of data between systems.

### Interface\Connection.cs

The Connection class will override methods from Integration.Abstract.Connection. This is the master class for a given connection to a given site. Other interface classes will be created as locally-typed instances, such as CallWrapper, Settings, TranslationUtilities, CustomFieldHandler, Etc

### Interface\ConversionFunction.cs

The Connection class enables Integrators to build Static methods for performing routine transformations that are specific to the external system. Methods provided here can be accessed by the subscriber from within their UI portal when building mappings.

### Interface\CustomFieldHandler.cs

The CustomField class will override methods from Integration.Abstract.CustomFieldHandler enabling Integrators to build handling for custom fields since custom fields can be implemented differently in each external system.

### Interface\DevelopmentTests.cs

The DevelopmentTests class is only used during integration development testing. Methods created here will be called by the Integrator Console enabling Integrators during the certification steps to demonstrate that individual flows from the integration are performing as desired.

### Interface\Metadata.cs

The MetaData class will override methods from Integration.Abstract.MetaData enabling Integrators to define metadata about the external systems.
Metadata is populated in the marketplace and can also be managed from within the Integrator UI Experience.

### Interface\Settings.cs

The Settings class will override methods from Integration.Abstract.Settings enabling Integrators to define run-time settings that are unique to each subscriber.

### Interface\TranslationUtilities.cs

The TranslationUtilities class will override methods from Integration.Abstract.TranslationUtilities enabling Integrators to define how operations carried out by iPaaS.com should be performed with the external application. Methods like GetDestinationObject() links the MappingCollection to a local data model class. You can also define Child object relations, Handle scenarios where more than one external id will be present, and also build PreRequisites or PostActions.

## BUILDING

How you build or modify all the files needed to certify an integration for iPaaS.com.

### Create the Integration

Either through the UI or API create the integration so you have the integration ID for when you need to upload the integration.

### Prepare the MetaData class

Configure the new Integration with iPaaS Metadata using LoadMetaData() in Interface\Metadata.cs. Populate all property values.

### Prepare the APICall helper class

The methods in APICall.cs were built to assist with all the API Calls which will be performed by your integration with the external application.
Use this template to get started, but it should be modified to accommodate the API documentation provided by the external application provider.

1.  Apply Authorization in CreateRestRequest(...)
2.  Handle Errors in HandleResponse(...)

### Prepare the CallWrapper class
The CallWrapper class will be populated by iPaaS.com at runtime with information specific to the transfer request used by the integration and will return processing information back to iPaaS about how the transfer performed.

1.  Modify EstablishConnection(...) to define how iPaaS will establish a connection at runtime.
2.  Modify ValidateConnection(...) to define how the external system will confirm for iPaaS that a successful connection has been made. Validating a connection is typically performed by calling a non-authenticated end-point.


### MappingCollections

Please refer to the information gathered in the Research and Planning section in order to complete the following steps.

For each mapping collection that will be supported, you will need to build a data model consistent with the external API properties. Please note that after Data models are built, they will be associated with specific tasks later. See translation utilities for more.

1.  Add properties for each external field
2.  Implement the correct Abstract Inheritance Model (See Template.cs)
     * DataModels.AbstractIntegrationData (see AbstractIntegrationData.cs)
     * DataModels.AbstractIntegrationDataWithCustomFields (see AbstractIntegrationDataWithCustomFields.cs)
3.  Implement Methods:
     * GetPrimaryId(...)
     * SetPrimaryId(...)
     * Get(...)
     * Create(...)
     * Update(...)
     * Delete(...)
4.  Register the Data Model with iPaaS Metadata using GetTables() in Interface\Metadata.cs
5.  Register the Data Model with GetDestinationObject() in Interface\TranslationUtilities.cs. This enables iPaaS.com to work with an empty datamodel prior to transforming data from mappings.

`Important: While it is iPaaS.com best practice to include all fields from the external system’s object, not all properties in your Data Models will be mappable in either direction. If you did not already do so, consider tagging properties like this in your Data Models with [iPaaSIgnore]. This will exclude the registration of these fields with iPaaS Metadata. Subscribers will NOT be able to create mappings for these fields.`

### Create Presets and Settings

Whenever an integration is subscribed to, information that is unique to the customer may be needed by the integration to perform certain tasks.

Integrators can define Setting properties in Settings.cs which will receive the subscriber’s saved inputs at Runtime.
1.  Register any Preset fields with iPaaS Metadata using GetPresets() in Interface\Metadata.cs. These fields will be presented to the subscribers upon subscribing to the integration and save their unique inputs. They are used for authentication information and can be used to define global settings for the integration.
2.  Create and associate the local Setting properties in Interface\Settings.cs with the Preset names so iPaaS can deliver the unique customer values at runtime, which can be referenced by any dynamic formulas and conversion functions.

### Custom Field Handling

Build handling for custom fields since they can be implemented differently in each external system.

Modify the methods in CustomFieldHandler.cs (This is required if AbstractIntegrationDataWithCustomFields was inherited by any Data Model in your integration)
1.  GetValueCustomField(...)
2.  SetValueCustomField(...)
3.  GetCustomFieldKVPs(...)

### Translation Utilities

Whenever a transfer runtime event occurs, the methods in TranslationUtilties.cs will be executed.
1.  Some of the methods in this Template project have been abstracted into the Data Models, so that the method in TranslationUtilities will call a corresponding method in the Data Model directly such as GetPrimaryId(...), SetPrimaryId(...), ModelGetAsync(...), ModelCreateAsync(...), ModelUpdateAsync(...), and ModelDeleteAsync(...). 
Since you should have already created your Data Models by now, you will be familiar with these corresponding methods.
2.  In this Template project, the Method ValidateConnection() in Interface\TranslationUtilities.cs utilizes the ValidateConnection(...) defined in the CallWrapper. If using this preferred method, no update is required.

The remaining steps are optional if they apply to the external system. (Read more about each method within CallWrapper.cs):

3.  Define the Method HandlePrerequisite(...) in Interface\TranslationUtilities.cs (If applicable)
4.  Define the Method HandlePostActions(...) in Interface\TranslationUtilities.cs (If applicable)
5.  Define the Method GetChildMappings(...) in Interface\TranslationUtilities.cs (If applicable)
6.  Define the Method CollectAdditionalExternalIds(...) in Interface\TranslationUtilities.cs (If applicable)
7.  Define the Method EstimateTotalAPICallsMade(...) in Interface\TranslationUtilities.cs (If applicable)
8.  Define the Method UpdateWebhookSubscriptionAsync(...) in Interface\TranslationUtilities.cs (If applicable)
9.  Define the Method InitializeData(...) in Interface\TranslationUtilities.cs (If applicable)

## Logging Activity with iPaaS.com
iPaaS.com provides an easy way to log technical activity from within your integration at runtime for diagnosing issues, both during testing and once the integration is being used in a production environment.

During runtime, iPaaS.com will provide your integration a Connection object that contains a helper class named Logger that contains this method: * Log_Technical(string severity, string location, string details)

1.  Severity will indicate under what conditions iPaaS.com will record the entry:
     * E = Error (Always record the entry)
     * W = Warning (Always record the entry without Error)
     * D = Debug (Only record the entry when running in Debug mode)
     * V = Verbose (Verbose logging is turned off in all environments, so this will never be seen)
2.  Location is a personalized text entry that will indicate to you and your subscribers where in the integration the log entry is originating from. This is typically the Class and Method, but you can choose whatever is most manageable for you.
3.  Details is a personalized text entry that describes what is occurring.

These are some examples of how to Log an entry:
> **Example: Warning**
_connection.Logger.Log_Technical("W", string.Format("{0} CallWrapper.{1}", Identity.AppName, "MethodName"), "Calculating Custom Fields: Step 1");

> **Example: Debug**
_connection.Logger.Log_Technical("D", string.Format("{0} CallWrapper.{1}", Identity.AppName, "MethodName"), "Custom Fields Found: " & items.count());

> **Example: Error**
_connection.Logger.Log_Technical("E", string.Format("{0} CallWrapper.{1}", Identity.AppName, "MethodName"), resp.ErrorException.Message);

## Testing your Integration on iPaaS.com

When you are ready to test your integration with data in your staging environment account, you can use the iPaaS.com Integration Development Utility.

1.  For each Data Flow that you intend to provide support for, you must create a static method in DevelopmentTests.cs to demonstrate each feature. Here are some guidelines on naming conventions and objectives your tests should meet in order to pass certification.
     * Name should be the IntegrationName_MappingCollectionType_Direction_EventType
     * In this project, an example test was provided from the Philips MeetHue interface: MeetHue_Transaction_FromiPaaS_Update(Integration.Abstract.Connection connection)

## Uploading your Integration

When you are ready to upload your file to your staging environment, you can use the iPaaS.com Integration Development Utility.

## Building Mappings

After you successfully upload a new Integration:

1.  Return to your account portal in the subscriber experience
2.  Subscribe to your new integration from the marketplace
3.  Configure your subscription by entering credentials from your external system sandbox
4.  Build mappings in your subscriber account

## Building a Webhook Receiver

If the external application you are certifying an integration for provides webhooks, chances are that the webhook format is unique to that system. iPaaS.com provides a Dynamic Hook receiver, which you can configure to receive webhooks in the format provided by the external application.

Return to your account portal in the integrator experience to configure the Webhook Receiver.

## Certifying your Integration

Once you are complete and your integration is ready to be reviewed and published please email support@ipaas.com to start that process.

Once all testing is completed, we will generate Template mappings in your integration from the subscriber account that we tested on.

## Sample New Integration Project Plan

As noted above, this is a rough timeline for an experienced Integrator. Please leave yourself plenty of time if this is your first integration. 

====================================================================
### Prerequisites (.5 Days)

Establish an external system Sandbox
-  Verify login credentials for the UI
-  Verify login credentials for the API
 
====================================================================
### Phase 1 (1-2 Days)

Initial Discovery Steps:
-  Identify Flows
-  Review APIs
-  Verify Rough Field Mappings
-  Complete Discovery
-  Build Project Plan

Getting Started Steps:
-  Register Integration in iPaaS.com in the Integrator Experience
-  Setup a new Integration project
 
====================================================================
### Phase 2 (Less than 1 Day)

Build Steps:
*  Populate LoadMetaData(...) in Interface\Metadata.cs
*  Populate GetPresets(...) in Interface\Metadata.cs
*  Populate Settings.cs that will be used for Establishing a Connection
    * Populate EstablishConnection(...) in Interface\CallWrapper.cs
	- Apply end-point authorization formatting to CreateRestRequest(...) in Interface\APICall.cs
	- Populate ValidateConnection(...) in Interface\CallWrapper.cs
	- Build a method to test ValidateConnection in Interface\DevelopmentTest.cs
	- Perform your first upload using the Integration Development Utility
	- Using the iPaaS.com Integrator Experience on Staging, verify that the presets you defined were saved in the Integration.
	
Configure Steps:
-  Navigate to iPaaS.com Subscriber view > Subscriptions
-  Add a new subscription for your new integration.
-  Populate the Settings fields you defined as Presets earlier.
	
Test Steps:
	- Using the Integration Development Utility, execute the Development Test you built for Validating the Connection.
	
	* Please note that it may be necessary to modify Interface\APICall.cs, depending upon the external system.

When successful, Move to Phase 3

====================================================================
### Phase 3 (4-6 hours per flow, but complex data types with multiple children and dependencies may be additional hours)

Build Steps:

- Populate LoadMetaData(...) in Metadata.cs
- Build a DataModel for each data flow to be supported
	- Add properties to match each field from the corresponding external system object  
	- Populate GetPrimaryId(...)
	- Populate SetPrimaryId(...)
	- Populate Get(...)
	- Populate Create(...)
	- Populate Update(...)
	- Populate Delete(...)
		
- Add an entry for each DataModel in GetDestinationObject(...) in Interface\TranslationUtilities.cs
- If any of the DataModels were children of another, add an entry for each child DataModel in GetChildMappings(...) in Interface\TranslationUtilities.cs
- Build a method to test each DataModel Event in Inteface\DevelopmentTests.cs
	
Test Steps:

	- Using the Integration Development Utility, execute each Development Test you built for each DataModel event.
	
When successful, Move to Phase 4

====================================================================
### Phase 4 (2-8 hours per flow depending on the complexity of the data type and external API)

Configure Steps:
-  Perform another upload using the Integration Development Utility
-  Using the iPaaS.com Integrator Experience on Staging, verify that the tables and fields exist for the data models you defined in the integration.
-  Navigate to iPaaS.com Subscriber view > Subscriptions
-  Create new Mappings and individual field mappings for each DataFlow.  There should be one for each MappingCollection, Direction and Event Type
-  For Data Flows where data is moving From iPaaS.com, create a test record in the iPaaS Data Management module that corresponds to the MappingCollection you will be testing.
-  For Data Flows where data is moving To iPaaS.com, create a test record in the external system that corresponds to the end-point configured in your Data Models.
	
Test Steps:
-  Using the Integration Development Utility, execute a Hook request for each MappingCollection and Direction from your data flows.
-  Verify the data arrived successfully in the Destination System (Either the External System when the direction is From iPaaS.com, or in iPaaS.com when the direction is To iPaaS.com).

When successful, Move to Phase 5

====================================================================
### Phase 5 (4 Hours)

Build Integration
-  Populate GetScopes(...) in Interface\Metadata.cs
-  For each Scope that will receive a webhook from the external system, build handling for UpdateWebhookSubscriptionAsync(...) in Interface\TranslationUtilities.cs to enable the external system Webhook by API.
-  Using the iPaaS.com Integrator Experience on Staging, configure the Webhook Receiver to the external system specifications.
-  Using the iPaaS.com Integrator Experience on Staging: 
  -  Verify that the Scopes exist which you defined in the integration.
	
Test Integration
-  Navigate to iPaaS.com Subscriber view > Subscriptions
-  Edit the External Webhooks for the subscription you are testing
-  Enable the corresponding webhook for each Flow that will be moving "To iPaaS.com"
-  Verify that the external system webhooks were configured to send. If the external system does not have a way to verify this by API or within their UI, you can create a new record within the external system and verify it arrives in the corresponding iPaaS.com Data Module.
	
When successful, Move to Phase 6

====================================================================
### Optional Phase 6 (0-24 Hours)

Optional Additions
-  Define the Method HandlePrerequisite() in Interface\TranslationUtilities.cs (If applicable)
-  Define the Method HandlePostActions() in Interface\TranslationUtilities.cs (If applicable)
-  Define the Method CollectAdditionalExternalIds() in Interface\TranslationUtilities.cs (If applicable)
-  Define the Method EstimateTotalAPICallsMade() in Interface\TranslationUtilities.cs (If applicable)
-  Define the Methods in Interface\CustomFieldHandler.cs (If applicable)
-  Build Custom Functions in Interface\ConversionFunction.cs (If applicable)

When successful, Schedule for certification by emailing support@ipaas.com.

====================================================================

