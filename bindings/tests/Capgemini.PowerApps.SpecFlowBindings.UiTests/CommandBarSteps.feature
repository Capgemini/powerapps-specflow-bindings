Feature: Command Bar Steps
	In order to automate command bar interaction
	As a developer
	I want to use pre-existing command bar bindings

Background:
	Given I am logged in to the 'Mock App' app as 'an admin'
	When I open the 'Mock Records' sub area of the 'Primary Group' group

Scenario: Assert a command is visible
	Then I can see the 'New' command

Scenario: Assert a command not visible
	Then I can not see the 'Missing' command

Scenario: Select a command
	When I select the 'Refresh' command

Scenario: Select a command under a flyout
	When I select the 'No Options Available' command under the 'Run Report' flyout