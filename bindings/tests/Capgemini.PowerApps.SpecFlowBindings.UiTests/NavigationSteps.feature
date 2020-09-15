Feature: Navigation Steps
	In order to automate navigation interaction
	As a developer
	I want to use pre-existing navigation bindings

Scenario: I want to navigate to global search
	Given I am logged in to the 'Sales Team Member' app as 'an admin'
	When I open global search

Scenario: I want to utilise the quick create shortcut
	Given I am logged in to the 'Sales Team Member' app as 'an admin'
	When I open a quick create for the 'account' entity

Scenario: I want to sign in and open a subarea
	Given I am logged in to the 'Sales Team Member' app as 'an admin'
	When I open the 'Accounts' sub area of the 'Customers' group

Scenario: I want to sign in and open an area
	Given I am logged in to the 'Customer Service Hub' app as 'an admin'
	When I open the 'Scheduling' area