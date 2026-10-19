@ui @epic:UI @story:Workout_Plans
Feature: Workout plans database
  As a Jefit user
  I want to browse ready-made workout plans
  So that I can choose a training program for my goal

  @smoke @critical
  Scenario: Workout plans page displays plans
    Given I open the workout plans page
    Then the main heading should be "Routine Database"
    And the plans counter should show more than 1000 plans
    And at least 10 workout plan cards should be displayed

  @regression @normal
  Scenario Outline: Opening a workout plan category
    Given I open the workout plans page
    When I select the workout plan category "<category>"
    Then the current URL should contain "<url>"
    And at least 1 workout plan cards should be displayed

    Examples:
      | category        | url                       |
      | Beginner        | /routines/beginner        |
      | Home            | /routines/home            |
      | Muscle Building | /routines/muscle-building |

  @regression @normal
  Scenario: Opening workout plan details from the list
    Given I open the workout plans page
    When I open the first workout plan from the list
    Then the page heading should match the remembered name
