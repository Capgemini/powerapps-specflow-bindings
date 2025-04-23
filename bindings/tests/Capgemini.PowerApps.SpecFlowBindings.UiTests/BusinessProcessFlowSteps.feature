Feature: Business Process Flow Steps
	In order to automate business process flow interaction
	As a developer
	I want to use pre-existing business process flow bindings

Background:
	Given I am logged in to the 'Mock App' app as 'an admin'
	And I have created 'a record with a business process flow'
	And I have opened 'the record'

Scenario: Open and close business process flow
	When I select the 'First Stage' stage of the business process flow
	And I close the 'First Stage' stage of the business process flow

Scenario: Set business process flow stage as active
	When I click next stage on the the 'First Stage' stage of the business process flow
	And I set the 'First Stage' stage of the business process flow as active

Scenario: Click next stage on business process flow
	When I click next stage on the the 'First Stage' stage of the business process flow

Scenario: Pin stage on business process flow
	When I pin the 'First Stage' stage of the business process flow

Scenario Outline:  Enter values on business process flow
	When I select the 'First Stage' stage of the business process flow
	When I enter '<value>' into the '<column>' <type> field on the business process flow
	Then I can see a value of '<value>' in the '<column>' <type> field on the business process flow

Scenarios:
		| column         | type      | value          |
		| sb_text        | text      | Some text      |
		| sb_number      | numeric   | 10             |
		# | sb_yesno       | boolean   | false          | Currently failing due to https://github.com/microsoft/EasyRepro/issues/1140
		#| sb_choice      | optionset | Option A       |
		# | sb_dateandtime | datetime  | 1/1/2021 13:00 | Currently failing due to https://github.com/microsoft/EasyRepro/issues/1139
		#| sb_dateonly    | datetime  | 1/1/2021       |
		| sb_currency    | currency  | £10.00         |

Scenario: Enter lookup value on business process flow
	Given I have created 'a secondary mock record'
	When I select the 'First Stage' stage of the business process flow
	And I enter 'A secondary mock record' into the 'sb_lookup' lookup field on the business process flow
	Then I can see a value of 'A secondary mock record' in the 'sb_lookup' lookup field on the business process flow