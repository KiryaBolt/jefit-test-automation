@ui @epic:UI @story:Exercise_Database
Feature: Exercise database
  As a Jefit user
  I want to browse and filter the exercise database
  So that I can find the right exercise and learn the correct technique

  @smoke @critical
  Scenario: Exercise database page displays exercises
    Given I open the exercise database page
    Then the main heading should be "Exercise Database"
    And the exercise counter should show more than 1000 exercises
    And at least 10 exercise cards should be displayed

  @regression @normal
  Scenario Outline: Filtering exercises by muscle group
    Given I open the exercise database page
    When I filter exercises by muscle "<muscle>"
    Then the number of found exercises should decrease
    And every exercise card should belong to muscle group "<muscle>"

    Examples:
      | muscle |
      | Chest  |
      | Back   |
      | Biceps |

  @regression @normal
  Scenario: Navigating to the second page of exercises
    Given I open the exercise database page
    When I go to the next page of exercises
    Then the current URL should contain "page=2"
    And the exercise list should differ from the first page

  @smoke @critical
  Scenario: Opening exercise details from the list
    Given I open the exercise database page
    When I open the exercise "Barbell Bench Press" from the list
    Then the current URL should contain "/exercises/2/barbell-bench-press"
    And the main heading should be "Barbell Bench Press"

  @regression @normal
  Scenario: Exercise details page contains all information blocks
    Given I open the exercise details page "/exercises/2/barbell-bench-press"
    Then the exercise page should contain the following sections:
      | section              |
      | Target Muscle Groups |
      | Equipment            |
      | Difficulty           |
      | Instructions         |
