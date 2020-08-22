Feature: Global Search Steps
	In order to automate global search interaction
	As a developer
	I want to use pre-existing global search bindings

Scenario: User performs a global search For Accounts
	Given I am logged in to the 'Sales Team Member' app as 'an admin'
	When I search globally using the filter 'Power'
	When I apply a search filter using the filter 'Account'
	When I open a record from global search on the entity 'account' in the position of '0'
	Then I am presented with a 'Account' form for the 'account' entity