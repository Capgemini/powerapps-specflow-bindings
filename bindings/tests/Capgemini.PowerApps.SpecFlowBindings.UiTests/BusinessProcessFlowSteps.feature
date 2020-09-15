Feature: Business Process Flow Steps
	In order to automate business process flow interaction
	As a developer
	I want to use pre-existing business process flow bindings

Scenario: User closes business process flow
	Given I am logged in to the 'Sales Team Member' app as 'an admin'
	And I have created 'a record with a business process flow'
	And I have opened 'the record'
	When I select the 'Qualify' stage of the business process flow
	And I close the 'Qualify' stage of the business process flow