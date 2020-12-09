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


    ##NOT COVERED
    ##    [When("I click next stage on the the '(.*)' stage of the business process flow")]
    ##    [When("I pin the '(.*)' stage of the business process flow")]
    ##    [When("I set the '(.*|current)' stage of the business process flow as active")]
    ##    [When("I enter '(.*)' into the '(.*)' (text|optionset|boolean|numeric|currency|datetime|lookup) field on the business process flow")]
    ##    [Then("I can see a value of '(.*)' in the '(.*)' (?:currency|numeric|text) field on the business process flow")]
    ##    [Then("I can see a value of '(.*)' in the '(.*)' optionset field on the business process flow")]
    ##    [Then("I can see a value of '(.*)' in the '(.*)' lookup field on the business process flow")]
  
