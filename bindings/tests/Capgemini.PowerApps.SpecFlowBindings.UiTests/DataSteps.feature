Feature: Data Steps
	In order to automate data creation
	As a developer
	I want to use pre-existing data creation steps

Background:
	Given I am logged in to the 'Mock App' app as 'an admin'

Scenario: Set a lookup with an alias
	And I have created 'a record with an alias'
	And I have created 'a record referencing the record with an alias using @alias.bind'
	And I have opened 'the referencing record'
	Then I can see a value of 'The referenced record' in the 'sb_parent' lookup field

Scenario: Generate data at run-time with faker
	And I have created 'data decorated with faker moustache syntax'
	And I have opened 'the faked record'