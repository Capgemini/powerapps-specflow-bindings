Feature: Related Grid Steps
	In order to automate related grid interaction
	As a developer
	I want to use pre-existing related grid bindings

Background:
	Given I am logged in to the 'Mock App' app as 'an admin'
	And I have created 'a record with a subgrid and related records'
	And I have opened 'the record'

Scenario: Open record at a given position in a related grid
	When I open the related 'Secondary Mock Records' tab
	And I open the record at position 1st in the related grid

Scenario: Open related tab
	When I open the related 'Activities' tab

Scenario: Click command in a related grid
	When I open the related 'Activities' tab
	And I click the 'Add Existing Activity' button on the related grid

Scenario: Assert a button is visible on a flyout in a related grid
	When I open the related 'Activities' tab
	And I click the 'New Activity' button on the related grid
	Then I should not see a 'Missing' button in the flyout on the related grid

Scenario: Assert a button is not visible on a flyout in a related grid
	When I open the related 'Activities' tab
	And I click the 'New Activity' button on the related grid
	Then I should see a 'Task' button in the flyout on the related grid