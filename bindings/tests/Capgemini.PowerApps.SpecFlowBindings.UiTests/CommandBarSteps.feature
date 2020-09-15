Feature: CommandBar Steps
	In order to automate commandbar interaction
	As a developer
	I want to use pre-existing commandbar bindings

Scenario: User checks to ensure a command is available
	Given I am logged in to the 'Sales Team Member' app as 'an admin'
	When I open the 'Accounts' sub area of the 'Customers' group
	Then I can see the 'New' command

Scenario: User can execute a command on the command bar
	Given I am logged in to the 'Sales Team Member' app as 'an admin'
	When I open the 'Accounts' sub area of the 'Customers' group
	When I select the 'New' command
	Then I am presented with a 'Account' form for the 'account' entity