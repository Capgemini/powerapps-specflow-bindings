Feature: Data Steps
	In order to automate data creation interaction
	As a developer
	I want to use pre-existing data creation steps

Scenario: Create a record with an alias then create another record referencing the alias with @alias.bind
	Given I am logged in to the 'Sales Team Member' app as 'an admin'
	And I have created 'an aliased contact'
	And I have created 'an account with aliased contact'
	And I have opened 'a sample account'

Scenario: Use faker.js syntax to generate data values at run-time
	Given I am logged in to the 'Customer Service Hub' app as 'an admin'
	And I have created 'data decorated with faker moustache syntax'
	And I have opened 'the faked record'