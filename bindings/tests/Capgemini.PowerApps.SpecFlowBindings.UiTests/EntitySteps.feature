Feature: Entity Steps
	In order to automate entity interaction
	As a developer
	I want to use pre-existing entity bindings

Background:
	Given I am logged in to the 'Mock App' app as 'an admin'
	When I open the sub area 'Mock Records' under the 'Primary Group' area
	When I select the 'New' command
	And I select 'Information' form

Scenario: Select a tab
	When I select the 'Secondary' tab

Scenario: Assert tab is visible
	Then I can see the 'Primary' tab

Scenario: Assert tab is hidden
	Then I cannot see the 'Hidden' tab

Scenario Outline: Enter values on a form
	When I enter '<value>' into the '<column>' <type> field on the form
	Then I can see a value of '<value>' in the '<column>' <type> field

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

Scenario: Enter lookup on a form
	Given I have created 'a secondary mock record'
	When I enter 'A secondary mock record' into the 'sb_lookup' lookup field on the form
	Then I can see a value of 'A secondary mock record' in the 'sb_lookup' lookup field

Scenario Outline: Enter values on a form header
	When I enter '<value>' into the '<column>' <type> header field on the form

Scenarios:
		| column               | type      | value          |
		| sb_headertext        | text      | Some text      |
		| sb_headernumber      | numeric   | 10             |
		| sb_headeryesno       | boolean   | false          |
		| sb_headerchoice      | optionset | Option A       |
		| sb_headerdateandtime | datetime  | 1/1/2021 13:00 |
		| sb_headerdateonly    | datetime  | 1/1/2021       |
		| sb_headercurrency    | currency  | £10.00         |

Scenario: Enter lookup on a form header
	Given I have created 'a secondary mock record'
	When I enter 'A secondary mock record' into the 'sb_headerlookup' lookup header field on the form

Scenario: Enter multiple values on a form
	When I enter the following into the form
		| Value     | Field           | Type    | Location     |
		| Some text | sb_text         | text    | field        |
		| 10        | sb_headernumber | numeric | header field |

Scenario Outline: Clear field values
	When I enter '<value>' into the '<column>' <type> field on the form
	And I clear the '<column>' <type> field
	Then I can see a value of '<clearedValue>' in the '<column>' <type> field

Scenarios:
		| column         | type           | value              | clearedValue |
		| sb_text        | text           | Some text          |              |
		| sb_number      | numeric        | 10                 |              |
		| sb_choice      | optionset      | Option A           | --Select--   |
		| sb_choices     | multioptionset | Option A, Option B |              |
		| sb_dateandtime | datetime       | 1/1/2021 13:00     |              |
		| sb_dateonly    | datetime       | 1/1/2021           |              |
		| sb_currency    | currency       | £10.00             |              |

Scenario: Clear lookup value
	Given I have created 'a secondary mock record'
	When I enter 'A secondary mock record' into the 'sb_lookup' lookup field on the form
	And I clear the 'sb_lookup' lookup field
	Then I can see a value of '' in the 'sb_lookup' lookup field

Scenario: Delete a record
	Given I have created 'a secondary mock record'
	And I have opened 'the secondary mock record'
	When I delete the record

Scenario: Open and close the record set navigator
	Given I have created 'data decorated with faker moustache syntax'
	And I have created 'a record with an alias'
	When I open the sub area 'Mock Records' under the 'Primary Group' area
	And I open the record at position '0' in the grid
	And I open the record set navigator
	And I close the record set navigator

Scenario: Open a form
	When I select 'Additional information' form
	Then I am presented with a 'Additional information' form for the 'sb_mockrecord' entity

Scenario: Select a lookup
	When I select 'sb_lookup' lookup

Scenario: Save a record
	When I enter 'Some text' into the 'sb_name' text field on the form
	And I save the record

Scenario: Assign to a user or team
	Given I have created 'a team'
	And I have created 'a secondary mock record'
	And I have opened 'the secondary mock record'
	When I assign the record to a team named 'A team'

Scenario: Switch process
	Given I have created 'a record with a business process flow'
	And I have opened 'the record'
	When I switch process to the 'Secondary Business Process Flow' process

Scenario: Assert a field is optional
	Then the 'sb_text' field is optional

Scenario: Assert a field is required
	Then the 'sb_name' field is mandatory

Scenario: Assert a set of fields are optional
	Then the following fields are optional
	| fields    |
	| sb_text   |
	| sb_number |
	| sb_yesno  |

Scenario: Assert a set of fields are mandatory
	Then the following fields are mandatory
	| fields  |
	| sb_name |

Scenario Outline: Assert fom notification message
	Then I can see <type> form notification stating '<text>'

Scenarios:
		| type      | text                             |
		| an info   | A mock info form notification    |
		| a warning | A mock warning form notification |
		| an error  | A mock error form notification   |

Scenario: Assert header title
	Then I can see a value of 'New Mock Record' as the header title

Scenario Outline: Assert field editable
	Then I can edit the '<column>' field

Scenarios:
		| column         |
		| sb_text        |
		| sb_number      |
		| sb_yesno       |
		| sb_choice      |
		| sb_choices     |
		| sb_dateandtime |
		| sb_dateonly    |
		| sb_currency    |

Scenario: Assert fields editable
	Then I can edit the following fields
		| Field          |
		| sb_text        |
		| sb_number      |
		| sb_yesno       |
		| sb_choice      |
		| sb_choices     |
		| sb_dateandtime |
		| sb_dateonly    |
		| sb_currency    |

Scenario: Assert option set options
	Then I can see the following options in the 'sb_choice' option set field
		| Option   |
		| Option A |
		| Option B |
		| Option C |

Scenario: Assert field visible
	Then I can see the 'sb_name' field

Scenario: Assert field not visible
	Then I can not see the 'ownerid' field

Scenario: Assert record status
	Then the status of the record is active

Scenario: Assert business process error message
	When I enter 'Some text' into the 'sb_name' text field on the form
	And I enter 'true' into the 'sb_triggerbusinessprocesserror' boolean field on the form
	And I save the record
	Then I can see a business process error stating 'Mock business process error'

Scenario: Assert field not editable
	Then I can not edit the 'createdonbehalfby' field

Scenario: Assert fields not editable
	Then I can not edit the following fields
		| Field             |
		| createdonbehalfby |