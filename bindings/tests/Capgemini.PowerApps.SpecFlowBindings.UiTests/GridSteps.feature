Feature: Grid Steps
	In order to automate grid interaction
	As a developer
	I want to use pre-existing grid bindings

Background:
	Given I am logged in to the 'Mock App' app as 'an admin'

Scenario: Switch grid view
	When I open the 'Mock Records' sub area of the 'Primary Group' group
	And I switch to the 'Inactive Mock Records' view in the grid

Scenario: Open a grid record at a given position
	Given I have created 'a record with an alias'
	When I open the 'Mock Records' sub area of the 'Primary Group' group
	And I open the record at position 1st in the grid
	Then I am presented with a 'Information' form for the 'sb_mockrecord' entity

Scenario: Perform grid search
	When I open the 'Mock Records' sub area of the 'Primary Group' group
	And I search for 'Some text' in the grid

Scenario: Clear grid search
	When I open the 'Mock Records' sub area of the 'Primary Group' group
	And I search for 'Some text' in the grid
	And I clear the search in the grid

Scenario: Sort a grid
	When I open the 'Mock Records' sub area of the 'Primary Group' group
	And I sort the 'Name' column in the grid using the 'Sort A to Z' option

Scenario: Assert grid contains an aliased record
	Given I have created 'a record with an alias'
	When I open the 'Mock Records' sub area of the 'Primary Group' group
	Then the grid contains 'the referenced record'

Scenario: Highlight multiple aliased records from a grid
	Given I have created 'a record with an alias'
	And I have created 'data decorated with faker moustache syntax'
	When I open the 'Mock Records' sub area of the 'Primary Group' group
	And I select the following records from the grid
		| Alias                 |
		| the referenced record |
		| the faked record      |