@ui @auth @epic:UI @story:API_prepared_test_data
Feature: Workout plan created via API
  Test data is created through the API (RestSharp), then checked in the browser
  and removed after the scenario. Requires a test account and configured account API.

  @regression @normal
  Scenario: Workout plan created via API is visible in UI
    Given I am logged in via API
    And I have created a workout plan with a generated name via API
    When I transfer the API session to the browser
    And I open the my plans page
    Then the created workout plan should be displayed
