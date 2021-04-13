Feature: Quick Create Steps
	In order to automate quick create interaction
	As a developer
	I want to use pre-existing quick create bindings

Background:
	Given I am logged in to the 'Mock App' app as 'an admin'
	When I open a quick create for the 'Secondary Mock Record' entity

Scenario Outline: Enter values on a quick create
	When I enter '<value>' into the '<column>' <type> field on the quick create
	Then I can see a value of '<value>' in the '<column>' <type> field on the quick create

Scenarios:
		| column         | type           | value              |
		| sb_text        | text           | Some text          |
		| sb_number      | numeric        | 10                 |
		| sb_yesno       | boolean        | true               |
		| sb_choice      | optionset      | Option A           |
		| sb_choices     | multioptionset | Option A, Option B |
		| sb_dateandtime | datetime       | 1/1/2021 13:00     |
		| sb_dateonly    | datetime       | 1/1/2021           |
		| sb_currency    | currency       | £10.00             |

Scenario: Open a quick create form
	Given I am viewing the ' quick create form' form for the 'sb_secondarymockrecord' entity
	Then I am presented with a 'Quick Create: Secondary Mock Record' form for the 'sb_secondarymockrecord' entity

Scenario: Enter lookup on a quick create
	Given I have created 'a record with an alias'
	When I enter 'The referenced record' into the 'sb_parent' lookup field on the quick create
	Then I can see a value of 'The referenced record' in the 'sb_parent' lookup field on the quick create

Scenario: Enter multiple values on a quick create
	When I enter the following into the quick create
		| Value     | Field     | Type    |
		| Some text | sb_text   | text    |
		| 10        | sb_number | numeric |
	Then I can see a value of 'Some text' in the 'sb_text' text field on the quick create
	And I can see a value of '10' in the 'sb_number' numeric field on the quick create

Scenario: Cancel a quick create
	When I cancel the quick create

Scenario Outline: Clear values on a quick create
	When I enter '<value>' into the '<column>' <type> field on the quick create
	And I clear the '<column>' <type> field on the quick create
	Then I can see a value of '<clearedValue>' in the '<column>' <type> field on the quick create

Scenarios:
		| column         | type      | value          | clearedValue |
		| sb_text        | text      | Some text      |              |
		| sb_number      | numeric   | 10             |              |
		| sb_choice      | optionset | Option A       | --Select--   |
		| sb_dateandtime | datetime  | 1/1/2021 13:00 |              |
		| sb_dateonly    | datetime  | 1/1/2021       |              |
		| sb_currency    | currency  | £10.00         |              |

Scenario: Clear lookup on a quick create
	Given I have created 'a record with an alias'
	When I enter 'The referenced record' into the 'sb_parent' lookup field on the quick create
	And I clear the 'sb_parent' lookup field on the quick create
	Then I can see a value of '' in the 'sb_parent' lookup field on the quick create

Scenario: Save a quick create
	When I save the quick create

Scenario: Assert a given value for a given field on a quick create
	When I enter 'Some text' into the 'sb_text' text field on the quick create
	Then I can see a value of 'Some text' in the 'sb_text' text field on the quick create