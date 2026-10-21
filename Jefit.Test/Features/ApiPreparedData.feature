@ui @regression @api-data @epic:UI @story:API_prepared_test_data
Feature: UI checks with test data prepared via API
  Test data is not hardcoded: before the UI check it is received
  from the site over HTTP (RestSharp) and a random item is selected.

  @normal
  Scenario: Random exercise received via API is displayed correctly in UI
    Given I pick a random exercise from the exercise database via API
    When I open the selected item in the browser
    Then the page heading should match the remembered name
    And the page title should contain the remembered name

  @normal
  Scenario: Random workout plan received via API is displayed correctly in UI
    Given I pick a random workout plan from the routine database via API
    When I open the selected item in the browser
    Then the page heading should match the remembered name
