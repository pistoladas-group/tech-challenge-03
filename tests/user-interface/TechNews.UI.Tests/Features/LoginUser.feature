Feature: Login User

Scenario: Login user successfully
    Given the lector is at the landing page
    And the login button is clicked
    When the login form is populated with a registered lector
    And the login submit button is clicked
    Then the lector must be logged in
    And the lector must be redirected to the News Home page