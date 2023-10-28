Feature: Register User

    Scenario: Registering user successfully
        Given the lector is at the landing page
        And the register button is clicked
        When the register form is populated correctly
        And the register submit button is clicked
        Then the lector must be logged in
        And the lector must be redirected to the News Home page

    Scenario: Registering user with invalid e-mail
        Given the lector is at the landing page
        And the register button is clicked
        When the register form is populated with an invalid e-mail
        And the e-mail input loses focus
        Then an invalid e-mail error message appears

    Scenario: Registering user with invalid user name
        Given the lector is at the landing page
        And the register button is clicked
        When the register form is populated with an invalid user name
        And the register submit button is clicked
        Then an invalid user name error message appears
        
    Scenario: Registering user with weak password
        Given the lector is at the landing page
        And the register button is clicked
        When the register form is populated with a weak password
        And the register submit button is clicked
        Then a week password error message appears
        
    Scenario: Registering user with passwords that doesn't match
        Given the lector is at the landing page
        And the register button is clicked
        When the register form is populated with passwords that doesn't match
        And the password or confirm password input loses focus
        Then a password doesnt match error message appears