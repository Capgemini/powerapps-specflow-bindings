﻿Feature: Timeline Steps
	In order to automate timeline interaction
	As a developer
	I want to use pre-existing timeline bindings

Background:
	Given I am logged in to the 'Mock App' app as 'an admin'
	And I have created 'a record with a timeline on the main form'
	And I have opened 'the record'

Scenario: Add appointment to timeline
	When I add an appointment to the timeline with the subject 'A subject', the description 'A description', the duration '1', and the location 'A location'

Scenario: Add note to timeline
	When I add a note to the timeline with the title 'A note' and the body 'A note's body'

Scenario: Add phone call to timeline
	When I add a phone call to the timeline with the subject 'A subject', the description 'A description', the duration '1', and the number '07123456789'

Scenario: Add post to timeline
	When I post 'A post' to the timeline

Scenario: Add task to timeline
	When I add a task to the timeline with the subject 'A subject', the description 'A description' and the duration '1'

Scenario: Add email to timeline
	When I add an email to the timeline with the subject 'A subject', the duration '1', and the following contacts
		| Type | Name       |
		| To   | John Smith |
		| CC   | Jane Doe   |
		| BCC  | Joe Bloggs |