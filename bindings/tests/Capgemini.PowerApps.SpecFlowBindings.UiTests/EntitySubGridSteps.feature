Feature: Entity Sub Grid Steps
	In order to automate entity sub grid interaction
	As a developer
	I want to use pre-existing entity sub grid bindings

Background:
	Given I am logged in to the 'Mock App' app as 'an admin'
	And I have created 'a record with a subgrid and related records'
	And I have opened 'the record'

Scenario: Click subgrid command
	When I click the 'New Secondary Mock Record' command on the 'subgrid' subgrid

Scenario: Select aliased record in subgrid
	When I select 'the related record' from the 'subgrid' subgrid

Scenario: Select all subgrid rows
	When I select all in the 'subgrid' subgrid

Scenario: Open record at given position in subgrid
	When I open the record at position '0' in the 'subgrid' subgrid
	Then I am presented with a 'Information' form for the 'sb_secondarymockrecord' entity

Scenario: Open record at given position in subgrid with new binding
	When I open 1st record in the 'subgrid' subgrid
	Then I am presented with a 'Information' form for the 'sb_secondarymockrecord' entity

Scenario: Search in subgrid
	When I search for 'This should return no results' in the 'subgrid' subgrid
	Then I can see exactly 0 records in the 'subgrid' subgrid

Scenario: Switch subgrid view
	When I switch to the 'Inactive Secondary Mock Records' view in the 'subgrid' subgrid
	Then I can see exactly 0 records in the 'subgrid' subgrid

Scenario: Assert aliased record not in subgrid
	When I switch to the 'Inactive Secondary Mock Records' view in the 'subgrid' subgrid
	Then I can not see 'the related record' in the 'subgrid' subgrid

Scenario: Assert count of subgrid records
	Then I can see exactly 2 records in the 'subgrid' subgrid

Scenario: Assert subgrid contains aliased record
	Then the 'subgrid' subgrid contains 'the related record'

Scenario: Assert subgrid contains mulitiple aliased records
	Then the 'subgrid' subgrid contains the following records
		| Alias                         |
		| the related record            |
		| the additional related record |

Scenario Outline: Assert subgrid contains a record with a given value in a given column
	Then the 'subgrid' subgrid contains a record with '<value>' in the '<column>' <type> field

Scenarios:
		| column         | type           | value              |
		| sb_text        | text           | Some text          |
		| sb_number      | numeric        | 10                 |
		| sb_yesno       | boolean        | Yes                |
		| sb_choice      | optionset      | Option A           |
		| sb_choices     | multioptionset | Option A; Option B |
		| sb_dateandtime | datetime       | 01/01/2021 13:00   |
		| sb_dateonly    | datetime       | 31/01/2021         |
		| sb_currency    | currency       | £10.00             |

Scenario: Assert subgrid contains a record with a lookup to an aliased record
	Then the 'subgrid' subgrid contains a record with a reference to 'the record' in the 'sb_parent' lookup field

Scenario: Assert subgrid contains records with lookup to multiple aliased records
	Then the 'subgrid' subgrid contains records with the following in the 'sb_parent' lookup
		| Alias      |
		| the record |

Scenario: Assert a command is visible on a subgrid
	Then I can see the 'New Secondary Mock Record' command on the 'subgrid' subgrid

Scenario: Click a flyout command on a subgrid
	When I click the 'Run Report' flyout on the 'subgrid' subgrid

Scenario: Assert a command is not visible on a subgrid
	Then I can not see the 'Missing' command on the 'subgrid' subgrid

Scenario: Assert that a command is visible on the flyout of a subgrid
	When I click the 'Open Flyout' flyout on the 'subgrid' subgrid
	Then I can see the 'Flyout Button' command on the flyout of the subgrid

Scenario: Assert that a command is not visible on the flyout of a subgrid
	When I click the 'Run Report' flyout on the 'subgrid' subgrid
	Then I can not see the 'Missing' command on the flyout of the subgrid

Scenario: Click a command under a flyout on a subgrid
	When I click the 'Flyout Button' command under the 'Open Flyout' flyout on the 'subgrid' subgrid

Scenario: Select a record with a given value in a given column in the subgrid
	When I select a record with 'A related record' in the 'sb_name' field in the 'subgrid' subgrid