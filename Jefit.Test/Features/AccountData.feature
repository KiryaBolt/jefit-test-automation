@ui @regression @auth @epic:UI @story:API_prepared_test_data
Feature: Workout plan added via API
  Test data is created through the API (RestSharp): a plan from the catalog is added
  to "My plans" of the test account, checked in the browser and removed after the scenario.
  Requires a test account (UserEmail / UserPassword).

  @normal
  Scenario: Workout plan added via API is visible in UI
    Given I am logged in via API
    And I have added the catalog workout plan 19113 "6-Weeks to Six-Pack Abs" via API
    When I transfer the API session to the browser
    And I open the my plans page
    Then the added workout plan should be displayed
