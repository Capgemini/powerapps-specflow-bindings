Feature: Lookup Dialog Steps
	In order to automate lookup dialog interaction
	As a developer
	I want to use pre-existing lookup dialog bindings

Background:
	Given I am logged in to the 'Mock App' app as 'an admin'
	And I have created 'a record with a subgrid and related records'
	And I have created 'a secondary mock record'
	And I have opened 'the record'
	When I click the 'Add Existing Secondary Mock Record' command on the 'subgrid' subgrid

Scenario: Select the first result for a search term in a lookup dialog
	When I select 'A secondary mock record' in the lookup dialog

Scenario: Click add button in a lookup dialog
	When I select 'A secondary mock record' in the lookup dialog
	And I click Add in the lookup dialog