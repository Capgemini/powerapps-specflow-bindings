Feature: Grid Steps
	In order to automate Lookup interaction
	As a developer
	I want to use pre-existing lookup bindings

Scenario: Switch views for App loading grid
	Given I am logged in to the 'Sales Team Member' app as 'an admin'
	When I open the 'Accounts' sub area of the 'Customers' group
	When I switch to the 'All Accounts' view in the grid