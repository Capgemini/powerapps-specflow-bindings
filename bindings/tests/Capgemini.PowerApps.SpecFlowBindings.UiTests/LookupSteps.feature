Feature: Lookup Steps
	In order to automate Lookup interaction
	As a developer
	I want to use pre-existing lookup bindings

Background:
	Given I am logged in to the 'Mock App' app as 'an admin'
	When I open the sub area 'Mock Records' under the 'Primary Group' area
	And I select the 'New' command

Scenario: Click new button in lookup
	When I search for 'Some text' in the 'sb_lookup' lookup
	And I click the new button in the lookup

Scenario: Open a record at a given position in a lookup
	Given I have created 'a secondary mock record'
	When I search for 'A secondary mock record' in the 'sb_lookup' lookup
	And I open the record at position '0' in the lookup

Scenario: Perform a search in a lookup
	When I search for 'Some text' in the 'sb_lookup' lookup

Scenario: Switch view in a lookup
	When I search for 'Some text' in the 'sb_lookup' lookup
	And I switch to the 'Inactive Secondary Mock Records' view in the lookup

Scenario: Select a related entity in a lookup
	When I search for '*' in the 'sb_customer' lookup
	And I select the related 'Contacts' entity in the lookup

Scenario: Assert lookup search results only contain records the given names
	Given I have created 'a secondary mock record'
	When I search for 'A secondary mock record' in the 'sb_lookup' lookup
	Then I can see only the following records in the 'sb_lookup' lookup
		| Name                    |
		| A secondary mock record |

Scenario: Open a record set in a lookup
	Given I have created 'data decorated with faker moustache syntax'
	And I have opened 'the faked record'
	When I select a related 'sb_lookup' lookup field
	Then I am presented with a 'Information' form for the 'sb_secondarymockrecord' entity