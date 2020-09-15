Feature: Lookup Steps
	In order to automate Lookup interaction
	As a developer
	I want to use pre-existing lookup bindings

Scenario: User switches lookup views
	Given I am logged in to the 'Sales Team Member' app as 'an admin'
	When I open the sub area 'Accounts' under the 'Customers' area
	And I select the 'New' command
	And I select 'primarycontactid' lookup
	#KNOWN BUG (TestCategory - Bug - Fail) https://github.com/microsoft/EasyRepro/blob/aadad319f713e169ce080524f533f20d86b23c97/Microsoft.Dynamics365.UIAutomation.Sample/UCI/Read/OpenContact.cs
	And I select the 'All Contacts' view in the lookup