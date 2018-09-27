# SpecXRM

SpecXRM aims to make automated UI testing for Dynamics 365 more accessible by enabling best practice test automation and removing the dependency on developers who are proficient in both the Dynamics 365 APIs and browser automation tools.

## Installing / Getting started

Install the correct NuGet package for your project - depending on whether you're using the web client or UCI.

### Web

```shell
PM> Install-Package Capgemini.Test.Xrm.Web
```

### UCI 

```shell
PM> Install-Package Capgemini.Test.Xrm.Uci
```

### Configuration

Installing the NuGet package creates a *spec-xrm.yaml* file in your project's root directory. This is used to configure the instance, users and apps that will be used for your tests. You also have the ability to configure a remoteServerUrl if you using a remote WebDriver (e.g. Selenium Grid). 

```yaml
url: https://instance.region.dynamics.com
remoteServerUrl: http://localhost:4444/wd/hub
users: 
  - username: user@domain.com
    password: password
    isAdfs: false
    alias: admin
apps:
  - id: 7d4981cd-ec11-43b3-bdcc-3cb67b092b29
    name: Some App
```

### Writing feature files

SpecFlow allows the use of steps which have been defined in an external library. This is what allows SpecXRM steps to be used in your projects feature files. When the SpecXRM NuGet package is installed, your projects *app.config* is transformed to include the following -

```xml
<specFlow>
  <stepAssemblies>
    <stepAssembly assembly="Capgemini.Test.Xrm.Web" />
  </stepAssemblies>
</specFlow>
```

This tells SpecFlow to include the steps defined in SpecXRM - giving you access to all the steps when writing your feature files.

```gherkin
Scenario: Save a record with no name
	Given I am logged in as admin
	And I have created a demo record with no name
	When I save the record
	Then I should see an error on the cap_name field which reads "You must provide a value for Name."
```

### Writing step defintions

SpecXRM provides a way for you to write your own step definitions with access to EasyRepro and the WebDriver in a thread-safe manner. This ensures that your tests can be ran safely in parallel. You can do this by creating a class decorated with the SpecFlow `BindingAttribute` and inheriting from the SpecXRM `XrmWebStepDefiner` base class. You can then create your SpecFlow step bindings as normal, interacting with the `Browser` and `Driver` properties.

```csharp
[Binding]
public class MyCustomSteps : XrmWebStepDefiner
{
    [Given(@"I have a custom step")]
    public void GivenIHaveACustomStep()
    {
      // Interact with the inherited EasyRepro Browser property
      // Browser.

      // Interact with the inherited WebDriver property
      // Driver.
    }
}
```

### Test Setup

One of the principles of SpecXRM is to avoid performing test setup via the UI. This helps speed up test execution, as well as making the tests more robust (as the UI is more fragile than the SDK/API). For that reason, `Given` steps should be performed using the [Client API](client-api), [WebAPI](web-api) or [Organization Service](org-service).

You can create test data by using the following `Given` step -

```gherkin
Given I have created a (file name)
```

It will look for a JSON file in the *Data* folder (ensure that the files are set to copy to the output directory). You do not need to include the .json extension in the step. The JSON is the same stucture as though using the WebAPI with a couple of key differences. For example, this will create an account with an associated primary contact and an opportunity with an associated task.

```json
{
  "@logicalName": "account",
  "@alias": "sample account",
  "name": "Sample Account",
  "primarycontactid":
  {
    "firstname": "John",
    "lastname": "Smith"
  },
  "opportunity_customer_accounts":
  [
    {
      "name": "Opportunity associated to Sample Account",
      "Opportunity_Tasks":
      [
        { "subject": "Task associated to opportunity" }
      ]
    }
  ]
}
```

The `"@logicalName"` property tells SpecXRM which entity to create. The `"@alias"` is used to give friendly names to the records so that we can refer to them in our scenarios. 
## Contributing

To develop the repository further, fork it and develop on a separate feature branch before creating a pull request.

[easyrepro]:https://github.com/Microsoft/EasyRepro
[specflow]:http://specflow.org/
[client-api]:https://docs.microsoft.com/en-us/dynamics365/customer-engagement/developer/clientapi/reference
[web-api]:https://docs.microsoft.com/en-us/dynamics365/customer-engagement/developer/use-microsoft-dynamics-365-web-api
[org-service]:https://docs.microsoft.com/en-us/dynamics365/customer-engagement/developer/use-microsoft-dynamics-365-organization-service