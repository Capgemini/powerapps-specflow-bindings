Feature: Data Steps
	In order to automate data creation interaction
	As a developer
	I want to use pre-existing data creation steps

Scenario: Create a contact with an alias then create an account referencing the  alias as the primary contact id
	Given I am logged in to the 'Sales Team Member' app as 'an admin'
	And I have created 'an aliased contact'
	And I have created 'an account with aliased contact'
	And I have opened 'a sample account'

Scenario: Create a contact with tokenised values
	Given I am logged in to the 'Defra Trade' app as 'an admin'
	And I have created 'an account with replacement tokens'