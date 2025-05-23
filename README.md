# Power Apps SpecFlow Bindings

[![Build Status](https://capgeminiuk.visualstudio.com/GitHub%20Support/_apis/build/status/CI-Builds/NuGet%20Packages/Capgemini.PowerApps.SpecFlowBindings?branchName=master)](https://capgeminiuk.visualstudio.com/GitHub%20Support/_build/latest?definitionId=195&branchName=master) [![Quality Gate Status](https://sonarcloud.io/api/project_badges/measure?project=Capgemini_powerapps-specflow-bindings&metric=alert_status)](https://sonarcloud.io/dashboard?id=Capgemini_powerapps-specflow-bindings) [![Coverage](https://sonarcloud.io/api/project_badges/measure?project=Capgemini_powerapps-specflow-bindings&metric=coverage)](https://sonarcloud.io/dashboard?id=Capgemini_powerapps-specflow-bindings)

A Reqnroll bindings library for Power Apps.

The aim of this project is to make Power Apps test automation easier, faster and more accessible. It does this by providing a library of generic Reqnroll step bindings that adhere to test automation best practices. This allows effective automated tests to be written without the dependency on developers who are both proficient with Power Apps and test automation.

## Table of Contents

- [Installation](#Installation)
- [Usage](#Usage)
  - [Configuration](#Configuration)
  - [Writing feature files](#Writing-feature-files)
  - [Writing step bindings](#Writing-step-bindings)
  - [Test setup](#Test-setup)
    - [Bindings](#Bindings)
    - [Data file syntax](#Data-file-syntax)
    - [Dynamic data](#Dynamic-data)
- [Contributing](#Contributing)
- [Licence](#Licence)

## Installation

Follow the guidance in the **Installation and Setup** section in SpecFlow's [docs](https://specflow.org/getting-started/). After installing the IDE integration and setting up your project, install the NuGet package.

```shell
PM> Install-Package Capgemini.PowerApps.SpecFlowBindings
```

Once the NuGet package is installed, follow the Reqnroll [documentation](https://docs.reqnroll.net/stable/installation/configuration.html) on referencing an external binding library. At the time of writing, you should update a _reqnroll.json_ file in your project root as follows -

```json
{
    "bindingAssemblies": [
        { "assembly": "Capgemini.PowerApps.SpecFlowBindings" }
    ]
}
```

### WebDrivers

We do not have a dependency on any specific WebDrivers. You will need to ensure that the correct WebDrivers are available in your project based on the browser that you are targetting. For example - if your configuration file is targetting Chrome - you can install the Chrome WebDriver via NuGet -

```shell
PM> Install-Package Selenium.Chrome.WebDriver
```

## Usage

### Configuration

Installing the NuGet package creates a _power-apps-bindings.yml_ file in your project's root. This is used to configure the URL, browser, and users that will be used for your tests.

```yaml
url: SPECFLOW_POWERAPPS_URL # mandatory
useProfiles: false # optional - defaults to false if not set
deleteTestData: true # optional - defaults to true if not set
browserOptions: # optional - will use default EasyRepro options if not set
  browserType: Chrome
  headless: true
  width: 1920
  height: 1080
  startMaximized: false
  driversPath: ChromeWebDriver # optional - [Recommended when running tests from Azure DevOps Microsoft-hosted agent](https://docs.microsoft.com/en-us/azure/devops/pipelines/test/continuous-test-selenium?view=azure-devops#decide-how-you-will-deploy-and-test-your-app)
  additionalCapabilities: # optional - additional capabilities to pass to the WebDriver
    capabilityName: capabilityValue 
applicationUser: # optional - populate if creating test data for users other than the current user
  tenantId: SPECFLOW_POWERAPPS_TENANTID optional # mandatory
  clientId: SPECFLOW_POWERAPPS_CLIENTID # mandatory
  clientSecret: SPECFLOW_POWERAPPS_CLIENTSECRET # mandatory
users: # mandatory
  - username: SPECFLOW_POWERAPPS_USERNAME_SALESPERSON # mandatory
    password: SPECFLOW_POWERAPPS_PASSWORD_SALESPERSON # optional - populate if this user will be logging in for tests
    alias: a salesperson # mandatory
    otptoken: SPECFLOW_POWERAPPS_OTPTOKEN_SALESPERSON # optional - populate if this user has MFA enabled
```

The URL, driversPath, usernames, passwords, and application user details will be set from environment variable (if found). Otherwise, the value from the config file will be used. The browserOptions node supports anything in the EasyRepro `BrowserOptions` class.

Tests will be distributed evenly between the users if multiple users are configured with the same alias. This can be helpful when you run a large number of tests in parallel and are encountering errors due to user-level platform API limits.

#### User profiles

Setting the `useProfiles` property to true causes the solution to create and use a unique [profile](https://support.google.com/chrome/answer/2364824?co=GENIE.Platform%3DDesktop&hl=en) for each user listed in the config file. This currently only works in Chrome & Firefox and attempting to use it with Edge or IE will cause an exception to be thrown. By using profiles test runs for the same user will not be required to re-authenticate, this saves time during test runs. [To take full advantage of this you will need to have the "Stay Signed In" prompt enabled.](https://docs.microsoft.com/en-us/azure/active-directory/fundamentals/keep-me-signed-in)

### Writing feature files

You can use the predefined step bindings to write your tests.

```gherkin
Scenario: User can create a new account
 Given I am logged in to the 'Sales Team Member' app as 'an admin'
 When I open the 'Accounts' sub area of the 'Customers' group
 Then I can see the 'New' command
```

Alternatively, write your own step bindings (see below).

### Writing step bindings

You can write your own step bindings that have thread-safe access to EasyRepro and the Selenium WebDriver. This ensures that your tests can be ran safely in parallel. You can do this by creating a class decorated with the Reqnroll `BindingAttribute` and inheriting from the `PowerAppsStepDefiner` base class. You can then create your bindings by interacting with the `XrmApp` and `Driver` properties.

```csharp
[Binding]
public class MyCustomSteps : PowerAppsStepDefiner
{
    [Given(@"I have a custom step")]
    public void GivenIHaveACustomStep()
    {
      // Interact with the inherited EasyRepro 'Browser' object.
      // Interact with the inherited Selenium 'Driver' object.
    }
}
```

### Test setup

#### Bindings

We avoid performing test setup via the UI. This speeds up test execution and makes the tests more robust (as UI automation is more fragile than using supported APIs). _Given_ steps should therefore be carried out using the [Client API](client-api), [WebAPI](web-api) or [Organization Service](org-service).

You can create test data by using the following _Given_ steps -

```gherkin
Given I have created 'a record'
```

or

```gherkin
Given 'someone' has created 'a record with a difference'
```

These bindings look for a corresponding JSON file in a _data_ folder in the root of your project. The file is resolved using a combination of the directory structure and the file name. For example, the bindings above could resolve the following files:

```shell
└───data
    │   a record.json
    │
    └───a record
            with a difference.json
```

The deleteTestData property in the power-apps-bindings.yml file can be set to specify whether you want records created via these bindings to be deleted after a scenario has ran. You may wish to override the default value and retain these e.g. to aid in diagnosing failures.

If you are using the binding which creates data as someone other than the current user, you will need the following configuration to be present:

- a user with a matching alias in the `users` array that has the `username` set
- an application user with sufficient privileges to impersonate the above user configured in the `applicationUser` property.

Refer to the Microsoft documentation on creating an application user [here](https://docs.microsoft.com/en-us/power-platform/admin/create-users-assign-online-security-roles#create-an-application-user).

#### Data file syntax

The JSON is similar to that expected by Web API when creating records via a [deep insert](https://docs.microsoft.com/en-us/dynamics365/customer-engagement/developer/webapi/create-entity-web-api#create-related-entities-in-one-operation).

```json
{
  "@logicalName": "account",
  "@alias": "sample account",
  "name": "Sample Account",
  "primarycontactid": {
    "firstname": "John",
    "lastname": "Smith"
  },
  "opportunity_customer_accounts": [
    {
      "name": "Opportunity associated to Sample Account",
      "Opportunity_Tasks": [{ "subject": "Task associated to opportunity" }]
    }
  ]
}
```

The example above will create the following:

- An account
- An primary contact related to the account
- An opportunity related to the account
- A task related to the opportunity

In addition to the standard Web API syntax, we also have the following:

| Property     | Description                                                                                                                                                  | Requirement                                              |
|--------------|--------------------------------------------------------------------------------------------------------------------------------------------------------------|----------------------------------------------------------|
| @logicalName | the entity logical name of the root record                                                                                                                   | Mandatory (unless included using `@extends` - see below) |
| @alias       | a friendly alias that can be used to reference the created record in certain bindings. Can be set on nested records                                          | Optional                                                 |
| @extends     | a relative path to a data file to extend. Records in arrays are merged by index (you may need to include blank objects to insert new records into the array) | Optional                                                 |

#### Dynamic data

We support [faker.js](https://github.com/marak/Faker.js) moustache template syntax for generating dynamic test data at run-time. Please refer to the faker documentation for all of the functionality that is available. The below JSON will generate a contact with a random name, credit limit, email address, and date of birth in the past 90 years:

```json
{
  "@logicalName": "contact",
  "@alias": "a dynamically generated contact",
  "lastname": "{{name.firstName}}",
  "firstname": "{{name.lastName}}",
  "creditlimit@faker.number": "{{finance.amount}}",
  "emailaddress1": "{{internet.email}}",
  "birthdate@faker.date": "{{date.past(90)}}",
  "accountid@alias.bind": "sample account"
}
```

When using faker syntax, you must also annotate number or date fields using `@faker.number`, `@faker.date` or `@faker.dateonly` to ensure that the JSON is formatted correctly.

You can also dynamically set lookups by alias using `<lookup>@alias.bind` (this is limited to aliased records in other files - not the current file).

## Contributing

Please refer to the [Contributing](./CONTRIBUTING.md) guide.

## Licence

Power Apps SpecFlow Bindings is released under the [MIT licence](./LICENCE).
