# Power Apps SpecFlow Bindings

[![Build Status](https://capgeminiuk.visualstudio.com/GitHub%20Support/_apis/build/status/CI-Builds/NuGet%20Packages/Capgemini.PowerApps.SpecFlowBindings?branchName=master)](https://capgeminiuk.visualstudio.com/GitHub%20Support/_build/latest?definitionId=195&branchName=master) [![Quality Gate Status](https://sonarcloud.io/api/project_badges/measure?project=Capgemini_powerapps-specflow-bindings&metric=alert_status)](https://sonarcloud.io/dashboard?id=Capgemini_powerapps-specflow-bindings) [![Coverage](https://sonarcloud.io/api/project_badges/measure?project=Capgemini_powerapps-specflow-bindings&metric=coverage)](https://sonarcloud.io/dashboard?id=Capgemini_powerapps-specflow-bindings)

A SpecFlow bindings library for Power Apps.

The aim of this project is to make Power Apps test automation easier, faster and more accessible. It does this by providing a library of generic SpecFlow step bindings that adhere to test automation best practices. This allows effective automated tests to be written without the dependency on developers who are both proficient with Power Apps and test automation.

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

Follow the guidance in the **Installation and Setup** section in https://specflow.org/getting-started/. After installing the IDE integration and setting up your project, install the NuGet package.

```shell
PM> Install-Package Capgemini.PowerApps.SpecFlowBindings
```

Once the NuGet package is installed, follow the SpecFlow [documentation](https://specflow.org/documentation/Configuration/) on referencing an external binding library. At the time of writing - for SpecFlow 3.x - you should update a _specflow.json_ file in your project root as follows -

```json
{
    "stepAssemblies": [
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
browserOptions: # optional - will use default EasyRepro options if not set
  browserType: Chrome
  headless: true
  width: 1920
  height: 1080
  startMaximized: false
applicationUser: # optional - populate if creating test data for users other than the current user
  tenantId: SPECFLOW_POWERAPPS_TENANTID optional # mandatory
  clientId: SPECFLOW_POWERAPPS_CLIENTID # mandatory
  clientSecret: SPECFLOW_POWERAPPS_CLIENTSECRET # mandatory
users: # mandatory
  - username: SPECFLOW_POWERAPPS_USERNAME_SALESPERSON # mandatory
    password: SPECFLOW_POWERAPPS_PASSWORD_SALESPERSON # optional - populate if this user will be logging in for tests
    alias: a salesperson # mandatory
```

The URL, usernames, passwords, and application user details will be set from environment variable (if found). Otherwise, the value from the config file will be used. The browserOptions node supports anything in the EasyRepro `BrowserOptions` class.

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

You can write your own step bindings that have thread-safe access to EasyRepro and the Selenium WebDriver. This ensures that your tests can be ran safely in parallel. You can do this by creating a class decorated with the SpecFlow `BindingAttribute` and inheriting from the `PowerAppsStepDefiner` base class. You can then create your SpecFlow step bindings by interacting with the `XrmApp` and `Driver` properties.

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
Given 'someone' has created 'a record'
```

The examples above will both look for a JSON file named _a record.json_ in a _data_ folder in the root of your project. The difference is that the latter requires the following in the configuration file: 

- a user with an alias of _someone_ in the `users` array 
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

- `@logicalName` - the entity logical name of the root record. **Mandatory (unless included using `@extends` - see below)**. 
- `@alias` - a friendly alias that can be used to reference the created record in certain bindings. Can be set on nested records. **Optional**.
- '@extends` - a relative path to a data file to extend. Records in arrays are merged by index (you may need to include blank objects to insert new records into the array). **Optional**.

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

> ⚠ When using faker syntax, you must also annotate number or date fields using `@faker.number`, `@faker.date` or `@faker.dateonly` to ensure that the JSON is formatted correctly.

You can dynamically set lookups by alias using `<lookup>@alias.bind` as shown in the example above. This is currently limited to records that have already been created as part of a different file.

## Contributing

Please refer to the [Contributing](./CONTRIBUTING.md) guide.

## Licence

Power Apps SpecFlow Bindings is released under the [MIT licence](./LICENCE).
