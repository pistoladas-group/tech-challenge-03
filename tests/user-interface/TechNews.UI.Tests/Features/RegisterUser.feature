Feature: Register User

Scenario: Registering user successfully
    Given the lector is at the landing page
    And the Register button is clicked
    When the form is populated correctly
    And the submit button is clicked
    Then the lector must be logged in
    And the lector must be redirected to the News Home page