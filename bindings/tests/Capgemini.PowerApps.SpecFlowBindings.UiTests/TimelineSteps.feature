Feature: Timeline Steps
	In order to automate timeline interaction
	As a developer
	I want to use pre-existing timeline bindings

Scenario: User posts to timeline
	Given I am logged in to the 'Sales Team Member' app as 'an admin'
	And I have created 'a record with a timeline on the main form'
	And I have opened 'the record'
	#BUG - cannot find + button
	When I add a note to the timeline with the title 'Test' and the body 'Test'
	And I add a task to the timeline with the subject 'Test', the description 'Test' and the duration '1'
	And I add an appointment to the timeline with the subject 'Test', the description 'Test', the duration '1', and the location 'Test'
	And I add an email to the timeline with the subject 'Test', the duration '1', and the following contacts
		| Type | Name       |
		| To   | John Smith |
		| CC   | Jane Doe   |
		| BCC  | Joe Bloggs |
	And I add a phone call to the timeline with the subject 'Test', the description 'Test', the duration '1', and the number '1'