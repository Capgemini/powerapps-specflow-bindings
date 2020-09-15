Feature: Entity Steps
	In order to automate entity interaction
	As a developer
	I want to use pre-existing entity bindings

Scenario: User Creates a record for a given entity
	Given I am logged in to the 'Sales Team Member' app as 'an admin'
	When I open the sub area 'Accounts' under the 'Customers' area
	When I select the 'New' command
	When I select the 'Summary' tab
	When I enter the following into the form
		| Value         | Field | Type | Location |
		| PowerAppsTest | name  | text | field    |
	When I save the record
	Then I can see a value of 'PowerAppsTest' in the 'name' text field
	And I can see the 'Details' tab