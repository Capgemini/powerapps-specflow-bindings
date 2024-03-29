﻿Feature: Global Search Steps
	In order to automate global search interaction
	As a developer
	I want to use pre-existing global search bindings

Background:
	Given I am logged in to the 'Mock App' app as 'an admin'
	And I have created 'a record configured for global search'
	And the 'sb_mockrecord' entity is enabled for categorized search

Scenario: Perform a global search
	When I search globally using the filter 'Record'

Scenario: Apply a search filter
	When I search globally using the filter 'Record'
	And I apply a search filter using the filter 'Mock Record'

Scenario: Open a record from global search results
	When I search globally using the filter 'A record configured for global search'
	And I open a record from global search on the entity 'Mock Records' in the position of '0'
	Then I am presented with a 'Information' form for the 'sb_mockrecord' entity