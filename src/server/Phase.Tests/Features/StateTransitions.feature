Feature: StateTransitions

@CatchException
Scenario: execute command without result against a vacant phase
	Given the phase client is vacant
	When executing a command without result
	Then an exception should be thrown with message "Phase must be occupied before executing commands and queries"

@CatchException
Scenario: execute query against a vacant phase
	Given the phase client is vacant
	When executing a query
	Then an exception should be thrown with message "Phase must be occupied before executing commands and queries"

@CatchException
Scenario: vacate a vacant phase
	Given the phase client is vacant
	When executing vacate
	Then an exception should be thrown with message "Phase is already vacant"