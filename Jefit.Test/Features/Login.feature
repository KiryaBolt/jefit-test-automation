@ui @epic:UI @story:Authorization
Feature: Login
  As a registered Jefit user
  I want to log in to my account
  So that I can access my workout plans and progress

  @smoke @critical
  Scenario: Login form is displayed
    Given I open the login page
    Then the login form should be displayed

  @regression @critical
  Scenario: Login with invalid credentials is rejected
    Given I open the login page
    When I log in with randomly generated invalid credentials
    Then I should remain on the login page

  @regression @normal
  Scenario: Login with empty fields is not possible
    Given I open the login page
    When I submit the login form with empty fields
    Then I should remain on the login page

  @auth @regression @blocker
  Scenario: Successful login with valid credentials
    Given I open the login page
    When I log in with valid credentials
    Then I should be redirected away from the login page
