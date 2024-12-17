Feature: Filter launches by run name
  In order to verify that the launch filtering works correctly
  As a user of the system
  I want to filter launches by run name and see if they exist

  Background:
    Given open login page
    When login with valid credentials

  Scenario Outline: Filter page with run name
    Given I have opened the launches page
    When I filter launches by run name '<run_name>'
    Then the launch should be '<is_exist>'

    Examples:
      | run_name       | is_exist |
      | null           | false    |
      | Demo Api Tests | true     |
      | sios           | false    |