Feature: Dialog Steps
	In order to automate dialog interaction
	As a developer
	I want to use pre-existing dialog bindings

Background:
	Given I am logged in to the 'Mock App' app as 'an admin'
	And  I have created 'a record with an alias'
	And I have opened 'the referenced record'

Scenario: Confirm a confirmation dialog
	When I select the 'Show Confirmation Dialog' command
	And I confirm when presented with the confirmation dialog

Scenario: Cancel a confirmation dialog
	When I select the 'Show Confirmation Dialog' command
	And I cancel when presented with the confirmation dialog

@ignore
#Currently failing due to https://github.com/microsoft/EasyRepro/issues/1120
Scenario: Assign to me on assign dialog
	When I select the 'Assign' command
	And I assign to me on the assign dialog

@ignore
#Currently failing due to https://github.com/microsoft/EasyRepro/issues/1120
Scenario: Assign to user on assign dialog
	When I select the 'Assign' command
	And I assign to a user named 'Power Apps Checker Application' on the assign dialog

@ignore
#Currently failing due to https://github.com/microsoft/EasyRepro/issues/1120
Scenario: Assign to team on assign dialog
	Given I have created 'a different team'
	When I select the 'Assign' command
	And I assign to a team named 'A different team' on the assign dialog

Scenario: Close warning dialog
	When I select the 'Show Error Dialog' command
	And I close the warning dialog

Scenario: Click OK on set state dialog
	When I select the 'Deactivate' command
	And I click ok on the set state dialog

Scenario: Click cancel on set state dialog
	When I select the 'Deactivate' command
	And I click cancel on the set state dialog
#TODO:
#Scenario: Cancel publish dialog
#Scenario: Confirm publish dialog
#Scenario: Close opportunity as won
#Scenario: Close opportunity as lost