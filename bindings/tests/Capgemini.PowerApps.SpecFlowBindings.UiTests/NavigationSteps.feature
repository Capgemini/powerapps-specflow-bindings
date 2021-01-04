Feature: Navigation Steps
	In order to automate navigation interaction
	As a developer
	I want to use pre-existing navigation bindings

Background:
	Given I am logged in to the 'Mock App' app as 'an admin'

Scenario: Open global search
	When I open global search

Scenario: Open entity quick create
	When I open a quick create for the 'Secondary Mock Record' entity

Scenario: Open a subarea
	When I open the 'Mock Records' sub area of the 'Primary Group' group

Scenario: Open an area
	When I open the 'Secondary Area' area

Scenario: Sign out
	When I sign out

Scenario: Assert an area is visible
	Then I see the 'Primary Area' area
	
Scenario: Assert a group is visible
	Then I see the 'Primary Group' group	

Scenario: Assert a subarea is visible
	Then I see the 'Mock Records' subarea
	