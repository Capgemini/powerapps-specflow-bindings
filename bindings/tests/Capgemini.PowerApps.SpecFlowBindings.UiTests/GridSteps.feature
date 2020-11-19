Feature: Grid Steps
	In order to automate Lookup interaction
	As a developer
	I want to use pre-existing lookup bindings

Scenario: Sort the All Accounts view by Account Name A to Z
	Given I am logged in to the 'Sales Team Member' app as 'an admin'
	When I open the 'Accounts' sub area of the 'Customers' group
	And I switch to the 'All Accounts' view in the grid
	And I sort the 'Account Name' column in the grid using the 'Sort A to Z' option