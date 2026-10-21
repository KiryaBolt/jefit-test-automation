@ui @regression @epic:UI @story:Navigation
Feature: Site navigation
  As a visitor of jefit.com
  I want to move between the main sections of the site
  So that I can quickly find exercises, workout plans and account pages

  @smoke @critical
  Scenario: Home page opens with the main heading
    Given I am on the Jefit home page
    Then the page title should contain "JEFIT"
    And the main heading should be "Track & Plan Workouts"

  @smoke @critical
  Scenario: Get Started button leads to the sign up page
    Given I am on the Jefit home page
    When I click the Get Started button
    Then the current URL should contain "/signup"

  @normal
  Scenario Outline: Footer links open the corresponding pages
    Given I am on the Jefit home page
    When I click the "<link>" link in the footer
    Then the current URL should contain "<url>"
    And the page title should contain "<title>"

    Examples:
      | link              | url          | title                    |
      | Exercise Database | /exercises   | Exercise Database        |
      | Workout Plans     | /routines    | Workout Routine Database |
      | Login             | /login       | Login                    |
      | Sign Up           | /signup      | JEFIT                    |
      | Blog              | /blog        | JEFIT                    |
      | FAQ               | /support/faq | JEFIT                    |

  @minor
  Scenario Outline: Footer contains links to Jefit social networks
    Given I am on the Jefit home page
    Then the footer link "<network>" should point to "<address>"

    Examples:
      | network   | address                 |
      | Facebook  | facebook.com/jefitapp   |
      | Instagram | instagram.com/jefitapp  |
      | X         | x.com/JefitInc          |
      | Reddit    | reddit.com/r/jefit      |
