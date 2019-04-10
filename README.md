# SpecFlow for Dynamics 365

Web ![Build Status](https://capgeminiuk.visualstudio.com/Capgemini Reusable IP/_apis/build/status/NUGET CI Builds/Capgemini%20Test%20Xrm%20Web%20CI%20NUGET?branchName=master)

UCI ![Build Status](https://capgeminiuk.visualstudio.com/Capgemini Reusable IP/_apis/build/status/NUGET CI Builds/Capgemini%20Test%20Xrm%20UCI%20CI%20NUGET?branchName=master)

## Description

A SpecFlow bindings library for Dynamics 365.
The aim of this project is to make Dynamics 365 test automation easier, faster and more accessible. It does this by providing a library of generic SpecFlow step bindings that adhere to test automation best practices. This allows effective automated tests to be written without the dependency on developers who are both proficient with Dynamics 365 and test automation.

## Table of Contents

1. [Installation](#Installation)
1. [Usage](#Usage)
1. [Contributing](#Contributing)
1. [Credits](#Credits)
1. [License](#License)

## Installation

Follow the guidance in the **Installation and Setup** section in https://specflow.org/getting-started/. After installing the IDE integration and setting up your project, install the corresponding NuGet package for your implementation - depending on whether you're using the web client or UCI.

### Web

```shell
PM> Install-Package Capgemini.Test.Xrm.Web
```

### UCI

```shell
PM> Install-Package Capgemini.Test.Xrm.Uci
```

_Note: The UCI package does not yet contain any predefined step bindings. It currently provides `XrmUciStepDefiner` base class that makes writing your own bindings easier as well as ensuring that they are thread-safe - meaning your tests can be ran in parallel across all supported test runners._

### WebDrivers

We do not have a dependency on any specific WebDrivers. You will need to ensure that the correct WebDrivers are available in your project based on the browser that you are targetting. For example - if your configuration file is targetting Chrome - you can install the Chrome WebDriver via NuGet - 

```shell
PM> Install-Package Selenium.Chrome.WebDriver
```

## Usage

This section applies primarily to the Capgemini.Test.Xrm&#46;Web package as do not have pre-defined step bindings available for UCI. If you are using UCI, please skip ahead to the **writing step definitions** section.

### Configuration

Installing the NuGet package creates a _spec-xrm.yaml_ file in your project's root. This is used to configure the URL, browser, users and apps that will be used for your tests. You also have the option to configure a remoteServerUrl if you using a remote WebDriver (e.g. Selenium Grid).

```yaml
url: "https://instance.region.dynamics.com"
browser: Chrome
remoteServerUrl: "http://localhost:4444/wd/hub"
users:
  - username: "salesperson@organisation.onmicrosoft.com"
    password: "Password123"
    isAdfs: false
    alias: "a salesperson"
apps:
  - id: "7d4981cd-ec11-43b3-bdcc-3cb67b092b29"
    name: "Sales App"
```

### Writing feature files

After the NuGet package is installed, your projects _app.config_ is transformed to include the following -

```xml
<specFlow>
  <stepAssemblies>
    <stepAssembly assembly="Capgemini.Test.Xrm.Web" />
  </stepAssemblies>
</specFlow>
```

This tells SpecFlow to include the step bindings defined in the Capgemini.Test.Xrm.Web.dll. You can then use the predefined step bindings to write your tests -

```gherkin
Scenario: Save a record with no name
	Given I am logged in as "a salesperson"
	And I have created "a demo record with no name"
	When I save the record
	Then I should see an error on the "cap_name" field which reads "You must provide a value for Name."
```

### Writing step defintions

Both the web and UCI packages provide a way for you to write your own step bindings that have thread-safe access to EasyRepro and the WebDriver. This ensures that your tests can be ran safely in parallel. You can do this by creating a class decorated with the SpecFlow `BindingAttribute` and inheriting from the either the `XrmWebStepDefiner` or `XrmUciStepDefiner` base classes. You can then create your SpecFlow step bindings as normal while interacting with the `Browser` and `Driver` properties.

```csharp
[Binding]
public class MyCustomSteps : XrmWebStepDefiner
{
    [Given(@"I have a custom step")]
    public void GivenIHaveACustomStep()
    {
      // Interact with the inherited EasyRepro 'Browser' object.
      // Interact with the inherited Selenium 'WebDriver' object.
    }
}
```

### Test setup

We are avoiding performing test setup via the UI. This speeds up test execution and makes the tests more robust (as UI automation is more fragile than using supported APIs). _Given_ steps should therefore be carried out using the [Client API](client-api), [WebAPI](web-api) or [Organization Service](org-service).

You can create test data by using the following _Given_ step -

```gherkin
Given I have created "a record"
```

It will look for a JSON file in the _Data_ folder. You must ensure that these files are copying to the build output directory. You do not need to include the .json extension when writing the step (the example above would look for _'a record.json'_).

The JSON is the same as expected by WebAPI when creating records via a [deep insert](https://docs.microsoft.com/en-us/dynamics365/customer-engagement/developer/webapi/create-entity-web-api#create-related-entities-in-one-operation). The example below will create the following -

- An account
- An primary contact related to the account
- An opportunity related to the account
- A task related to the opportunity

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

## Contributing

The source code for will be made available soon. All contributions will be appreciated.

To contact us, please email nuget.uk@capgemini.com.

## Credits

Capgemini UK Microsoft Team

And the creators of the following:

- EasyRepro
- Newtonsoft.Json
- NUnit 
- Selenium
- SpecFlow
- YamlDotNet

## License

SpecFlow for Dynamics 365 is released under the [MIT license](./License).
