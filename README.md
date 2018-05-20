![Capgemini Logo](https://capgemini.github.io/images/logo.svg)

# Capgemini.Test.Xrm
> Automated UI testing for Dynamics 365

This repository contains a number of packages to make automated UI testing for Dynamics 365 easier. This includes:
- Extensions to [EasyRepro][easyrepro] 
- Step definitions and base classes for projects using [SpecFlow][specflow]

## Installing / Getting started

Install the correct NuGet package for your project.

### Capgemini.Test.Xrm.Bindings&#46;Web

SpecFlow step definitions and bindings for automated UI testing on the Dynamics 365 Web Client.

```shell
PM> Install-Package Capgemini.Test.Xrm.Bindings.Web
```

And your SpecFlow feature files will show additional bound steps:
```gherkin
Given I am logged in to the "{appName}" app as "{user}"
```

If you are using any of your own step definitions, you must ensure that the class defining your steps inherits from the `XrmWebStepDefiner` base class and interacts with via the browser using the inherited `Browser` property.

```csharp
[Binding]
public class MyCustomSteps : XrmWebStepDefiner
{
    [Given(@"I have a custom step")]
    public void GivenIHaveACustomStep()
    {
      // Interact with the inherited Browser property
    }
}
```

### Capgemini.Test.Xrm.EasyRepro&#46;Web

Extensions for EasyRepro specific to testing the Dynamics 365 web client.

```shell
PM> Install-Package Capgemini.Test.Xrm.EasyRepro.Web
```

This will make extension methods available. For example, there is an extension method to handle ADFS logins.

```csharp
Browser.LoginPage.Login(dynamicsUrl, username, password, isAdfs);
```

### Capgemini.Test.Xrm.Bindings.UUI

*Coming soon!*

### Capgemini.Test.Xrm.EasyRepro.UUI

*Coming soon!*

## Developing

To develop the repository further, clone it:

```shell
git clone https://capgeminiuk.visualstudio.com/Capgemini%20Reusable%20IP/_git/Capgemini.Test.Xrm
``` 

Develop on a separate feature branch and create a pull request into master.

## Configuration

The root of your project should contain an **xrm.test.config** file. Ensure that this file is copied to the output directory. It contains details of the Dynamics 365 instance, the user credentials and the Dynamics 365 Apps that can be used for test:

```xml
<?xml version="1.0" encoding="utf-8" ?>
<XrmTestConfig>
  <Url>https://yourinstance.crm4.dynamics.com</Url>
  <Users>
    <User Alias="Salesperson" IsAdfs="false">
      <Username>salesperson@domain.onmicrosoft.com</Username>
      <Password>P@ssw0rd</Password>
    </User>
    <User Alias="Sales Manager" IsAdfs="true">
      <Username>salesperson@domain.com</Username>
      <Password>P@ssw0rd</Password>
    </User>
  </Users>
  <Apps>
    <App Name="Sales App">
      <Id>6848bb0b-ed5f-45bb-9a6f-f196b21d04ee</Id>
    </App>
  </Apps>
</XrmTestConfig>
```

[easyrepro]:https://github.com/Microsoft/EasyRepro
[specflow]:http://specflow.org/
[capgemini-logo]:https://capgemini.github.io/images/logo.svg