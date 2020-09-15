Feature: Quick Create Steps
	In order to automate quick create interaction
	As a developer
	I want to use pre-existing quick create bindings

Scenario: User Creates a record for a given entity
	Given I am logged in to the 'Sales Team Member' app as 'an admin'
	When I open the sub area 'Accounts' under the 'Customers' area
	And I open a quick create for the 'account' entity