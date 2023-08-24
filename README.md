# Target Process Analyzer Console Application

The **Target Process Analyzer** is a console application designed to analyze Target Process user stories and bugs in correlation with Azure DevOps, identifying discrepancies and issues in the development process. The application covers 12 different scenarios to ensure the integrity and correctness of your development workflow.

## Features

- **Comprehensive Analysis:** Analyze user stories and bugs based on 12 different scenarios to catch discrepancies and potential issues.

- **Azure DevOps Integration:** Link Azure DevOps commit information to user stories and bugs, ensuring accurate tracking of development progress.

- **Customizable Configuration:** Configure the application through the `appsettings.json` file to match your organization's setup and workflow.

## Scenarios Covered

1. US or Bug is marked as "Done" but missing QA entity state (Testing step).
2. US or Bug where the current State is not set to "Done".
3. Unable to identify US or Bug based on Azure DevOps commit comments (Development Branch Commits).
4. Azure DevOps Commits are linked to a US or Bug not associated with the current Sprint.
5. Product was changed by a code check-in where US or Bug was previously marked as 'Done' or 'Rejected'.
6. UserStory has been set to 'Done', but not all associated tasks are 'Done' (**Disabled Functionality**).
7. UserStory is set to 'Development Complete', but not all associated Developer tasks are closed.
8. UserStory is set to 'Performance Testing Complete', but not all associated QA tasks are closed.
9. UserStory is assigned a Developer, but no Developer Tasks have been created.
10. UserStory is assigned a QA, but no QA tasks have been created.
11. Current Sprint Code Commits checked into the previous Sprint Release Branch.
12. User Story set to Dev Complete, but Developer has not added screenshots of successful test execution in Comments.

## Configuration

To use the application, follow these steps:

1. Update the `appsettings.json` file with your configuration:
   - Update the placeholders (`null`) with actual values:
     - `year`, `organization`, `project`, `sprint`, `previousSprint`, `repoName`, `signOffTagFrom`, `signOffTagTo`, `teamCityPAT`, `targetProcessPAT`, `azureDevOpsPAT`.
   - Add your Target Process team names to the `teamNames` array.
   - Add your Target Process sprint names to the `teamSprintNames` and `teamSprintNamesForAPI` arrays.
   - Adjust the URL placeholders in `TPApiItemSelector.cs` to match your Target Process instance.
   - Update the file location in `AsciiFileCreator.cs` and `CommonUtilities.cs` where the Excel data will be written.
   - Replace `"PlaceHolder"` in `program.cs` with your Azure DevOps Organization Name.

2. Build and run the console application.
---

**Note:** This application requires valid Azure DevOps, TeamCity, and Target Process credentials, along with accurate configuration in the `appsettings.json` file to function correctly.
