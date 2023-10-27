Feature: Login User

    Scenario: Login user successfully
        Given the lector is at the landing page
        And the login button is clicked
        And the login form is populated with a registered lector
        When the login submit button is clicked
        Then the lector must be logged in
        And the lector must be redirected to the News Home page

    Scenario: User does not exists
        Given the lector is at the landing page
        And the login button is clicked
        And the login form is populated with an unexistent lector
        When the login submit button is clicked
        Then a generic error message appears

    Scenario: User provides wrong password
        Given the lector is at the landing page
        And the login button is clicked
        And the login form is populated with a registered lector
        And the password is incorrect
        When the login submit button is clicked
        Then a generic error message appears

    Scenario: User provides wrong email
        Given the lector is at the landing page
        And the login button is clicked
        And the login form is populated with a registered lector
        And the email is incorrect
        When the login submit button is clicked
        Then a generic error message appears

    Scenario: User locks account
        Given the lector is at the landing page
        And the login button is clicked
        And the login form is populated with a registered lector
        And the password is incorrect
        When the login submit button is clicked more than three times
        Then a lockout message appears

    Scenario: User does not provide email or password
        Given the lector is at the landing page
        And the login button is clicked
        And the login form is not populated
        When the login submit button is clicked
        Then the user remains in the login page

    Scenario: Access login from register page
        Given the lector is at the landing page
        And the register button is clicked
        When the login link is clicked
        Then the lector must be redirected to the login page