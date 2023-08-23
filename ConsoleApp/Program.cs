using Microsoft.VisualStudio.Services.WebApi;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Windows;
using Microsoft.TeamFoundation.SourceControl.WebApi;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Common.Models;
using TPApiClient.Services;
using Common.Utilities;
using Common.Services;
using Common.Enumerators;
using Common.AzureModels;
using AzureDevOpsApiClient.Services;
using Microsoft.Identity.Client;
using System;
using System.Windows;
using OneDriveAPIClient;
using System.Threading;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net.Http;
using Common.TeamCityModels;
using System.Globalization;

namespace ConsoleApp
{
    class Program
    {
        #region Constants
        private const string PAT_DEFAULT_AZURE_DEVOPS = "";
        private const string PAT_DEFAULT_TP = "";
        private const string US_REGEX_POUND_5_DIGITS = @"#[\d]{5}";
        private const string US_REGEX_POUND_6_DIGITS = @"#[\d]{6}";
        private const string US_REGEX_US_5_DIGITS = @"US[\d]{5}";
        private const string US_REGEX_US_6_DIGITS = @"US[\d]{6}";
        private const string US_REGEX_POUND_US_6_DIGITS = @"#US[\d]{6}";
        private const string US_REGEX_POUND_US_5_DIGITS = @"#US[\d]{5}";
        private const string US_REGEX_POUND_SPACE_US_SINGLESPACE_6_DIGITS = @"#US\s[\d]{6}";
        private const string US_REGEX_POUND_SPACE_US_SINGLESPACE_5_DIGITS = @"#US\s[\d]{5}";
        private const string US_REGEX_POUND_SPACE_US_TAB_6_DIGITS = @"#US\t[\d]{6}";
        private const string US_REGEX_POUND_SPACE_US_TAB_5_DIGITS = @"#US\t[\d]{5}";
        private const string US_REGEX_POUND_SPACE_US_DOUBLESPACE_6_DIGITS = @"#\sUS\s[\d]{6}"; 
        private const string US_REGEX_POUND_SPACE_US_DOUBLESPACE_5_DIGITS = @"#\sUS\s[\d]{5}"; 
        private const string US_REGEX_POUND_BUG_6_DIGITS = @"#Bug[\d]{6}";
        private const string US_REGEX_POUND_BUG_5_DIGITS = @"#Bug[\d]{5}";
        private const string US_REGEX_POUND_B_6_DIGITS = @"#B[\d]{6}";
        private const string US_REGEX_POUND_B_5_DIGITS = @"#B[\d]{5}";
        private const string US_REGEX_POUND_SPACE_BUG_SINGLESPACE_6_DIGITS = @"#Bug\s[\d]{6}";
        private const string US_REGEX_POUND_SPACE_BUG_SINGLESPACE_5_DIGITS = @"#Bug\s[\d]{5}";
        #endregion

        #region Fields
        private static string _year;
        private static string _organization;
        private static string _project;
        private static string _repoName;
        private static string _patAzureDevops;
        private static string _patTP;
        private static string _patTeamCity;
        private static string _signOffTagFrom;
        private static string _signOffTagTo;
        private static string _sprint;
        private static string _previousSprint;
        private static TargetProcessItemList _targetProcessItemList = new TargetProcessItemList();
        private static TargetProcessItemTaskList _targetProcessItemTaskList = new TargetProcessItemTaskList();
        private static TargetProcessItemAssignmentList _targetProcessAssignmentsTaskList = new TargetProcessItemAssignmentList();
        private static TargetProcessTeamMemberList _targetProcessDeveloperListPerTeam = new TargetProcessTeamMemberList();
        private static TargetProcessItemList _targetProcessUserStoryByNameItemList = new TargetProcessItemList();
        private static TargetProcessItemList _targetProcessEpicItemList = new TargetProcessItemList();
        private static TargetProcessItemList _targetProcessFeatureItemList = new TargetProcessItemList();
        private static TargetProcessItemStateHistoryList _targetProcessItemStateHistoryList = new TargetProcessItemStateHistoryList();
        private static TargetProcessCommentListForUS _targetProcessCommentListForUS = new TargetProcessCommentListForUS();
        private static List<CommitTargetProcessItem> _commits;
        private static List<CommitTargetProcessItem> _commitsToCheck;
        private static string[]? _teamNames;
        private static string[]? _teamSprintNames;
        private static string[]? _teamSprintNamesForAPI;
        private static List<GitBranchStats> _currentSprintBranches = new List<GitBranchStats>();
        private static List<string> _listOfStatesBeforeDevelopementComplete = new List<string> {"01 - New","02 - Scorecard In Progress", "03 - Scorecard Complete","04 - Approved","06 - Refinement In Progress","06b - Ready for Tech Review","07 - Ready for Sprint Planning",
            "08 - Development In Progress","08b - Code Review" };
        private static List<string> _listOfStatesBeforeQAComplete = new List<string> {"01 - New","02 - Scorecard In Progress", "03 - Scorecard Complete","04 - Approved","06 - Refinement In Progress","06b - Ready for Tech Review","07 - Ready for Sprint Planning",
            "08 - Development In Progress","08b - Code Review", "09 - Development Complete", "10 - UAT In Progress","11 - UAT Complete", "12 - Data Verification In Progress","13 - Bug Reported", "14 - Data Verification Complete", 
            "15 - Functional Testing In Progress", "16 - Functional Testing Complete", "17 - Regression Testing Complete", "18 - Performance Testing In Progress"};

        #endregion

        #region Teams
        private static List<string> teamSprintList = new List<string>();
        private static List<string> teamNameList = new List<string>();
        private static List<List<string>> allDeveloperList = new List<List<string>>();
        private static List<string> teamOneDeveloperList = new List<string>();
        private static List<string> teamTwoDeveloperList = new List<string>();
        private static List<string> teamThreeDeveloperList = new List<string>();
        private static List<string> teamFourDeveloperList = new List<string>();
        private static List<string> teamFiveDeveloperList = new List<string>();
        private static List<string> teamSixDeveloperList = new List<string>();
        private static List<string> teamSevenDeveloperList = new List<string>();
        private static List<string> teamEightDeveloperList = new List<string>();
        private static List<string> teamNineDeveloperList = new List<string>();
        #endregion

        #region TeamCommits 
        private static List<CommitTargetProcessItem> _commitsTeamOne = new List<CommitTargetProcessItem>();
        private static List<CommitTargetProcessItem> _commitsTeamTwo = new List<CommitTargetProcessItem>();
        private static List<CommitTargetProcessItem> _commitsTeamThree = new List<CommitTargetProcessItem>();
        private static List<CommitTargetProcessItem> _commitsTeamFour = new List<CommitTargetProcessItem>();
        private static List<CommitTargetProcessItem> _commitsTeamFive = new List<CommitTargetProcessItem>();
        private static List<CommitTargetProcessItem> _commitsTeamSix = new List<CommitTargetProcessItem>();
        private static List<CommitTargetProcessItem> _commitsTeamSeven = new List<CommitTargetProcessItem>();
        private static List<CommitTargetProcessItem> _commitsTeamEight = new List<CommitTargetProcessItem>();
        private static List<CommitTargetProcessItem> _commitsTeamNine = new List<CommitTargetProcessItem>();
        #endregion

        #region GraphAPI (No Longer in Use)
        private static string ClientId = "11f4ef9b-666d-4252-ad67-4fb7b74de7a9";
        private static string Tenant = "common";
        //Set the API Endpoint to Graph 'me' endpoint
        string graphAPIEndpoint = "https://graph.microsoft.com/v1.0/me";

        //Set the scope for API call to user.read
        static string[] scopes = new string[] { "user.read" };
        static string accessToken = "";
        #endregion

        static void Main(string[] args)
        {
            initializeSettings(); // Initialize AppSettings.Json           
            populateScenarios(); //Scenario Descriptions         
            initializeTeams(); // Get list of all Developers per Team and assigned them to a List

            getTPEntitiesDoneButNotTested();
            Thread.Sleep(5000);
            getTPEntitiesMarkedAsNotDone();
            Thread.Sleep(5000);
            cannotIdentifyUSfromCommitComments();
            Thread.Sleep(5000);
            getCommitsWithoutAssociatedSprint();
            Thread.Sleep(5000);
            getTPEntitiesMarkedDoneWithCommits();
            Thread.Sleep(5000);
            getTPEntitiesMarkedAsDevCompleteWithDevTasksNotClosed();
            Thread.Sleep(5000);
            getTPEntitiesMarkedAsQACompleteWithQATasksNotClosed();
            Thread.Sleep(5000);
            getTPEntitiesMarkedAsDoneWithNoDevTasks();
            Thread.Sleep(5000);
            getTPEntitiesMarkedAsDoneWithNoQATasks();
            Thread.Sleep(5000);
            getCurrentSprintCommitsPlacedIntoPreviousReleaseCandidate();
            Thread.Sleep(5000);
            getUSWithoutDeveloperScreenshots();
            Thread.Sleep(5000);
            getListOfAllTeamCityBuildsForThisSprint();
            Thread.Sleep(5000);

            Console.WriteLine("Deleting all extra Excel Tabs");
            for (int i = 0; i < 12; i++)
            {
                CommonUtilities.deleteTabFromExcelFile();
                Console.WriteLine($"Tab number deleted: {i}");
                GC.Collect();
            }      
        }
      
        public static string linkDeveloperToTeam(CommitTargetProcessItem commit)
        {
            string developerName = (commit.Commit.Author.Email.Substring(0, commit.Commit.Author.Email.IndexOf("@"))).Replace(".", " ").ToLower();
            
            if (teamOneDeveloperList.ConvertAll(d => d.ToLower()).Contains(developerName))
            {
                return "Captains";
            }
            else if (teamTwoDeveloperList.ConvertAll(d => d.ToLower()).Contains(developerName))
            {
                return "Chargers";
            }
            else if (teamFourDeveloperList.ConvertAll(d => d.ToLower()).Contains(developerName))
            {
                return "Cardinals";
            }
            else if (teamThreeDeveloperList.ConvertAll(d => d.ToLower()).Contains(developerName))
            {
                return "Condors";
            }
            else if (teamFiveDeveloperList.ConvertAll(d => d.ToLower()).Contains(developerName))
            {
                return "Phoenix";
            }
            else if (teamSixDeveloperList.ConvertAll(d => d.ToLower()).Contains(developerName))
            {
                return "PathFinders";
            }
            else if (teamSevenDeveloperList.ConvertAll(d => d.ToLower()).Contains(developerName))
            {
                return "Pioneers";
            }
            else if (teamEightDeveloperList.ConvertAll(d => d.ToLower()).Contains(developerName))
            {
                return "Spartans";
            }
            else if (teamNineDeveloperList.ConvertAll(d => d.ToLower()).Contains(developerName))
            {
                return "Solutions Architecture";
            }
            return "--";
        }

        public static bool developerExistsInTeam(string developerName)
        {         
            if (teamOneDeveloperList.ConvertAll(d => d.ToLower()).Contains(developerName))
            {
                return true;
            }
            else if (teamTwoDeveloperList.ConvertAll(d => d.ToLower()).Contains(developerName))
            {
                return true;
            }
            else if (teamFourDeveloperList.ConvertAll(d => d.ToLower()).Contains(developerName))
            {
                return true;
            }
            else if (teamThreeDeveloperList.ConvertAll(d => d.ToLower()).Contains(developerName))
            {
                return true;
            }
            else if (teamFiveDeveloperList.ConvertAll(d => d.ToLower()).Contains(developerName))
            {
                return true;
            }
            else if (teamSixDeveloperList.ConvertAll(d => d.ToLower()).Contains(developerName))
            {
                return true;
            }
            else if (teamSevenDeveloperList.ConvertAll(d => d.ToLower()).Contains(developerName))
            {
                return true;
            }
            else if (teamEightDeveloperList.ConvertAll(d => d.ToLower()).Contains(developerName))
            {
                return true;
            }
            else if (teamNineDeveloperList.ConvertAll(d => d.ToLower()).Contains(developerName))
            {
                return true;
            }
            return false;
        }

        private static void populateScenarios()
        {
            Console.WriteLine("Scenarios covered for Target Process:");
            Console.WriteLine("1 - US or Bug is marked as \"Done\" but missing QA entity state (Testing step).");
            Console.WriteLine("2 - US or Bug where current State is not set to \"Done\".");
            Console.WriteLine("3 - Could not identify US or Bug based on Azure Devops Commit comments. (Development Branch Commits)");
            Console.WriteLine("4 - Azure DevOps Commits are linked to a US or Bug that is not associated with the current Sprint.");
            Console.WriteLine("5 - Product was changed by a code check in where US or Bug was previously marked as 'Done' or 'Rejected'");
            Console.WriteLine("6 - UserStory has been set to 'Done', but the list of associated tasks have not all been set to 'Done' (**Disabled Functionality**)");
            Console.WriteLine("7 - UserStory has been set to 'Development Complete', but the list of associated Developer tasks have not all been closed out");
            Console.WriteLine("8 - UserStory has been set to 'Performance Testing Complete', but the list of associated QA tasks have not all been closed out");
            Console.WriteLine("9 - UserStory has been assigned a Developer, but no Developer Tasks has been created");
            Console.WriteLine("10 - UserStory has been assigned a QA, but no QA tasks has been created");
            Console.WriteLine("11 - Current Sprint Code Commits have been checked into previous Sprint Release Branch");
            Console.WriteLine("12 - User Story set to Dev Complete, but Developer has not put in any screenshots showing succesful test execution in Comments");
            Console.WriteLine("\r\n");
        }

        private static void getTPEntitiesMarkedAsNotDone()
        {
            Console.WriteLine("Starting - getTPEntitiesMarkedAsNotDone");
            ProcessResponse response = targetProcessEntitiesByEpicName($"Product Documentation {_year}");
            if (response != null && !response.IsSuccess)
            {
                Console.WriteLine($"Error getting list of state histories: {response.SystemError}\r\n{response.ErrorDescription}");
                return;
            }           
                response = targetProcessSprintEntities();
                if (response != null && !response.IsSuccess)
                {
                    Console.WriteLine($"Error: {response.SystemError}\r\n{response.ErrorDescription}");
                    return;
                }

                if (_targetProcessItemList.Items.Any())
                {
                    List<TargetProcessItem> entityList = _targetProcessItemList.Items.Where(entity => entity.Id != null).ToList(); //Create new entity list and put all items from _TargetProcessItemList
                    List<TargetProcessItem> notDoneEntities = new List<TargetProcessItem>();
                    List<TargetProcessItem> notDoneEntitiesFinalList = new List<TargetProcessItem>();

                    foreach (TargetProcessItem entity in entityList)
                    {
                        if (entity.State.Name.ToLower().IndexOf("done") == -1 && entity.State.Name.ToLower().IndexOf("by design") == -1
                            && entity.State.Name.ToLower().IndexOf("rejected") == -1 && entity.State.Name.ToLower().IndexOf("cannot reproduce") == -1)
                        {
                            List<TargetProcessItem> entitiesPartOfEpic =
                        _targetProcessEpicItemList.Items.Where(epicEntity => epicEntity.Id == entity.Id).ToList();
                            if (!entitiesPartOfEpic.Any())
                            {
                                notDoneEntitiesFinalList.Add(entity);
                            }
                        }
                    }
                        printEntities($"Found {{0}} Target Process Entities not in \"Done\" state for sprint {_sprint}", notDoneEntitiesFinalList);
                        AsciiFileCreator.UpdateExcelFile(notDoneEntitiesFinalList, 5 , Int32.Parse(_sprint));
                        Console.WriteLine("All done - getTPEntitiesMarkedAsNotDone");
                        GC.Collect();
            }            
        }

        private static void getTPEntitiesDoneButNotTested()
        {
            Console.WriteLine("Starting - getTPEntitiesDoneButNotTested");
            ProcessResponse response = targetProcessEntitiesByEpicName($"Product Documentation {_year}");
            if (response != null && !response.IsSuccess)
            {
                Console.WriteLine($"Error getting list of state histories: {response.SystemError}\r\n{response.ErrorDescription}");
                return;
            }
            response = targetProcessEntitiesByFeatureId(144575);
            if (response != null && !response.IsSuccess)
            {
                Console.WriteLine($"Error getting list of state histories: {response.SystemError}\r\n{response.ErrorDescription}");
                return;
            }                       
                 response = targetProcessSprintEntities();
                if (response != null && !response.IsSuccess)
                {
                    Console.WriteLine($"Error getting list of sprint entities: {response.SystemError}\r\n{response.ErrorDescription}");
                    return;
                }
                response = targetProcessSprintEntitiesFullHistory();
                if (response != null && !response.IsSuccess)
                {
                    Console.WriteLine($"Error getting list of state histories: {response.SystemError}\r\n{response.ErrorDescription}");
                    return;
                }
                
                List<TargetProcessItemStateHistory> entitiesStateHistory = new List<TargetProcessItemStateHistory>();
                List<TargetProcessItem> entitiesStateDone =
                        _targetProcessItemList.Items.Where(entity => entity.State.Name.ToLower().IndexOf("done") > -1).ToList();               
                foreach (TargetProcessItem entitityStateDone in entitiesStateDone)
                {
                    List<TargetProcessItemStateHistory> entitiesInTestState =
                        _targetProcessItemStateHistoryList.Items.Where(entity => entity.UserStory.Id == entitityStateDone.Id
                            && (entity.EntityState.Name.ToLower().IndexOf("by design") > -1
                            || entity.EntityState.Name.ToLower().IndexOf("cannot reproduce") > -1
                            || entity.EntityState.Name.ToLower().IndexOf("rejected") > -1
                            || entity.EntityState.Name.ToLower().IndexOf("qa - test execution pending") > -1
                            || entity.EntityState.Name.ToLower().IndexOf("bug reported") > -1
                            || entity.EntityState.Name.ToLower().IndexOf("data verification complete") > -1
                            || entity.EntityState.Name.ToLower().IndexOf("functional testing in progress") > -1
                            || entity.EntityState.Name.ToLower().IndexOf("functional testing complete") > -1
                            || entity.EntityState.Name.ToLower().IndexOf("regression testing complete") > -1
                            || entity.EntityState.Name.ToLower().IndexOf("performance testing in progress") > -1
                            || entity.EntityState.Name.ToLower().IndexOf("performance testing complete") > -1)).ToList();

                    List<TargetProcessItem> entitiesNotPartOfEpic =
                        _targetProcessEpicItemList.Items.Where(entity => entity.Id == entitityStateDone.Id).ToList();

                    List<TargetProcessItem> entitiesNotPartOfFeature =
                        _targetProcessFeatureItemList.Items.Where(entity => entity.Id == entitityStateDone.Id).ToList();

                    if (!entitiesInTestState.Any() && !entitiesNotPartOfEpic.Any() && !entitiesNotPartOfFeature.Any())
                    {
                        entitiesStateHistory.AddRange(_targetProcessItemStateHistoryList.Items.Where(entity => entity.UserStory.Id == entitityStateDone.Id
                            && !entity.EntityState.Name.ToLower().Contains("testing")).ToList());
                    }
                }               
                    printEntities($"Found {{0}} Target Process Entities without testing state in sprint {_sprint}:", entitiesStateHistory);
                    AsciiFileCreator.UpdateExcelFile(entitiesStateHistory, 4 , Int32.Parse(_sprint));
                    Console.WriteLine("All done - getTPEntitiesDoneButNotTested");
                    GC.Collect();
        }
        
        private static void cannotIdentifyUSfromCommitComments()
        {
            Console.WriteLine("Starting - cannotIdentifyUSfromCommitComments");
            if (!linkCommitsUserStoriesCurrentTeamSprintDevelopementBranch())
            {
                Console.WriteLine("Error exit.");
                return;
            }
            _commitsToCheck = _commits.Where(commit => !commit.IsMerge && commit.CommitUSId == null).
                OrderBy(commit => commit.Commit.Author.Name).
                OrderByDescending(commit => commit.IsInDevelopment).ToList();
            printCommitInfo("US Id wasn't found for {0} commits:", _commitsToCheck);
            foreach(CommitTargetProcessItem commit in _commitsToCheck)
            {
                commit.DeveloperTeam = linkDeveloperToTeam(commit);
            }
            AsciiFileCreator.UpdateExcelFile(_commitsToCheck, EnumOutputFileType.MissingUS, 6 , Int32.Parse(_sprint));                     
            Console.WriteLine("All done - cannotIdentifyUSfromCommitComments");
            GC.Collect();
        }       

        private static void getCommitsWithoutAssociatedSprint()
        {
            Console.WriteLine("Starting - getCommitsWithoutAssociatedSprint");
            if (!linkCommitsUserStoriesCurrentTeamSprintDevelopementBranch())
            {
                Console.WriteLine("Error exit.");
                return;
            }

            ProcessResponse response = targetProcessEntitiesByUserStoryName($"ADMIN: Sprint {_sprint}");
            if (response != null && !response.IsSuccess)
            {
                Console.WriteLine($"Error: {response.SystemError}\r\n{response.ErrorDescription}");
                return;
            }
          
            _commitsToCheck = _commits.Where(commit => !commit.IsMerge && commit.UserStory == null && commit.CommitUSId != null).
                OrderBy(commit => commit.CommitUSId).
                OrderByDescending(commit => commit.IsInDevelopment).ToList();
            if (!findUSbyUSId(_commitsToCheck))
            {
                Console.WriteLine("Error exit.");
                return;
            }

            List<TargetProcessItem> entityList = _targetProcessUserStoryByNameItemList.Items.Where(entity => entity.Id != null).ToList();
            List<CommitTargetProcessItem> _commitsToPrint = new List<CommitTargetProcessItem>();

            foreach(CommitTargetProcessItem item in _commitsToCheck)
            {
                foreach(int tpItem in entityList.Select(x => x.Id).Distinct().ToList()) // Make sure no 2 users stories have the same exact name
                {
                    if (item.CommitUSId != tpItem)
                    {
                        if (item.UserStory != null && !item.UserStory.ResourceType.Equals("Request"))
                        _commitsToPrint.Add(item);
                    }                   
                }
            }
            printCommitInfo("Wrong Team Sprint for {0} commits:", _commitsToPrint);
            AsciiFileCreator.UpdateExcelFile(_commitsToPrint, EnumOutputFileType.WrongTeamSprint, 7, Int32.Parse(_sprint));
            Thread.Sleep(5000);          
            Console.WriteLine("All done - getCommitsWithoutAssociatedSprint");
            GC.Collect();
        }

        private static void getTPEntitiesMarkedDoneWithCommits()
        {
            Console.WriteLine("Starting - getTPEntitiesMarkedDoneWithCommits");
            if (!linkCommitsUserStoriesHistoryCurrentTeamSprint())
            {
                Console.WriteLine("Error exit.");
                return;
            }
            
            _commitsToCheck = _commits.Where(commit => !commit.IsMerge && commit.UserStoryHistory != null && commit.CommitUSId != null).
                OrderBy(commit => commit.CommitUSId).
                OrderByDescending(commit => commit.IsInDevelopment).ToList();
            if (!findUSbyUSId(_commitsToCheck))
            {
                Console.WriteLine("Error exit.");
                return;
            }
          
            printCommitInfo("Commits on Done User Story: {0} commits:", _commitsToCheck);
            AsciiFileCreator.UpdateExcelFile(_commitsToCheck, EnumOutputFileType.CommitOnDoneTpEntity, 8 , Int32.Parse(_sprint));      
            Console.WriteLine("All done - getTPEntitiesMarkedDoneWithCommits");
            GC.Collect();
        }

        private static void getTPEntitiesMarkedAsDoneAndTasksNotComplete()
        {
            Console.WriteLine("Starting - getTPEntitiesMarkedAsDoneAndTasksNotComplete");
            ProcessResponse response = targetProcessEntitiesByEpicName($"Product Documentation {_year}");
            if (response != null && !response.IsSuccess)
            {
                Console.WriteLine($"Error getting list of state histories: {response.SystemError}\r\n{response.ErrorDescription}");
                return;
            }
            response = targetProcessSprintEntities();
            if (response != null && !response.IsSuccess)
            {
                Console.WriteLine($"Error: {response.SystemError}\r\n{response.ErrorDescription}");
                return;
            }

            if (_targetProcessItemList.Items.Any())
            {
                List<TargetProcessItem> entityList = _targetProcessItemList.Items.Where(entity => entity.Id != null).ToList(); //Create new entity list and put all items from _TargetProcessItemList
                List<TargetProcessItem> doneEntitiesCalibratedList = new List<TargetProcessItem>();
                List<TargetProcessItem> userStoryHasNotDoneTasksList = new List<TargetProcessItem>();

                foreach (TargetProcessItem entity in entityList)
                {
                    if (entity.State.Name.ToLower().IndexOf("done") > -1)
                    {
                        List<TargetProcessItem> entitiesPartOfEpic =
                    _targetProcessEpicItemList.Items.Where(epicEntity => epicEntity.Id == entity.Id).ToList();
                        if (!entitiesPartOfEpic.Any())
                        {
                            doneEntitiesCalibratedList.Add(entity);
                        }
                    }
                }
                bool hasUnfinishedTasks = false;
                foreach (TargetProcessItem entity in doneEntitiesCalibratedList)
                {
                    targetProcessTaskEntities(entity.Id);
                    List<TargetProcessTaskItem> taskListForUserStory = _targetProcessItemTaskList.Items.Where(entity => entity.Id != null).ToList();
                    foreach (TargetProcessTaskItem item in taskListForUserStory)
                    {
                        if (item.EntityState.Name.ToLower() != "done")
                        {
                            hasUnfinishedTasks = true;
                        }
                    }
                    if (hasUnfinishedTasks)
                    {
                        userStoryHasNotDoneTasksList.Add(entity);
                    }
                }
                printEntities($"Found {{0}} Target Process Entities where Done State and Tasks not closed out", userStoryHasNotDoneTasksList);
                AsciiFileCreator.UpdateExcelFile(userStoryHasNotDoneTasksList, 9, Int32.Parse(_sprint));
                Console.WriteLine("All done - getTPEntitiesMarkedAsDoneAndTasksNotComplete");
                GC.Collect();
            }
        }

        private static void getTPEntitiesMarkedAsDevCompleteWithDevTasksNotClosed()
        {
            Console.WriteLine("getTPEntitiesMarkedAsDevCompleteWithDevTasksNotClosed");
           
            ProcessResponse response = targetProcessSprintEntitiesFullHistoryWithoutBugs();
            if (response != null && !response.IsSuccess)
            {
                Console.WriteLine($"Error: {response.SystemError}\r\n{response.ErrorDescription}");
                return;
            }

            response = targetProcessSprintEntities();
            if (response != null && !response.IsSuccess)
            {
                Console.WriteLine($"Error: {response.SystemError}\r\n{response.ErrorDescription}");
                return;
            }

            if (_targetProcessItemStateHistoryList.Items.Any())
            {
                List<TargetProcessItemStateHistory> entityWithHistoryList = _targetProcessItemStateHistoryList.Items.Where(entity => entity.EntityState.Name.IndexOf("Development Complete") > -1).ToList(); //Create new entity list and put all items from _TargetProcessItemList
                List<TargetProcessItem> entityList = _targetProcessItemList.Items.Where(entity => entity.Id != null).ToList();
                List<TargetProcessItemStateHistory> targetProcessHistoryListDistinct = new List<TargetProcessItemStateHistory>();
                bool uniqueId = true;
                int currentId = 0;
                foreach (TargetProcessItemStateHistory targetProcessItem in entityWithHistoryList)
                {
                    if (currentId == targetProcessItem.UserStory.Id)
                    {
                        uniqueId = false;
                    }
                    else
                    {
                        uniqueId = true;
                    }

                    if (uniqueId == true)
                    {
                        foreach(TargetProcessItem item in entityList)
                        {
                            if (item.Id.Equals(targetProcessItem.UserStory.Id))
                            {
                                if (!_listOfStatesBeforeDevelopementComplete.Contains(item.State.Name))
                                {
                                    targetProcessHistoryListDistinct.Add(targetProcessItem);
                                    currentId = targetProcessItem.UserStory.Id;
                                }                               
                            }
                        }                        
                    }
                }
                List<TargetProcessItemStateHistory> userStoryHasNotDoneDeveloperTasksList = new List<TargetProcessItemStateHistory>();               
                bool hasUnfinishedTasks = false;
                bool unfinishedTaskLinkedToDeveloper = false;
                bool devTaskHasNoEffort = false;
                foreach (TargetProcessItemStateHistory entity in targetProcessHistoryListDistinct)
                
                {
                    targetProcessTaskEntities(entity.UserStory.Id);
                    targetProcessAssignmentsforEntity(entity.UserStory.Id);
                    List<TargetProcessTaskItem> taskListForUserStory = _targetProcessItemTaskList.Items.Where(entity => entity.Id != null).ToList();
                    List<TargetProcessAssignmentItem> assignmentListForUserStory = _targetProcessAssignmentsTaskList.Assignments.Items.Where(assignment => assignment.Role.Id.Equals(1)).ToList();
                    foreach (TargetProcessTaskItem item in taskListForUserStory)
                    {
                        if (item.EntityState.Name.ToLower() != "done")
                        {
                            hasUnfinishedTasks = true;
                        }
                        if (hasUnfinishedTasks)
                        {
                            foreach(TargetProcessAssignmentItem assignedDev in assignmentListForUserStory)
                            {
                                if (item.AssignedUsers.Items.Where(assignedToTask => assignedToTask.FullName.Equals(assignedDev.GeneralUser.FullName)).Any())
                                {
                                    unfinishedTaskLinkedToDeveloper = true;
                                    if (item.Effort.Equals(0))
                                    {
                                        devTaskHasNoEffort = true;
                                    }
                                }
                            }                             
                        }
                        hasUnfinishedTasks = false;
                    }
                    if (unfinishedTaskLinkedToDeveloper || devTaskHasNoEffort)
                    {
                        userStoryHasNotDoneDeveloperTasksList.Add(entity);
                        unfinishedTaskLinkedToDeveloper = false;
                        devTaskHasNoEffort = false;
                    }
                }
                printEntities($"Found {{0}} Target Process Entities Where Dev Complete State and not closed Dev Tasks", userStoryHasNotDoneDeveloperTasksList);
                AsciiFileCreator.UpdateExcelFileForTasks(userStoryHasNotDoneDeveloperTasksList, 9, Int32.Parse(_sprint));
                Console.WriteLine("All done - getTPEntitiesMarkedAsDevCompleteWithDevTasksNotClosed");
                GC.Collect();
            }
        }

        private static void getTPEntitiesMarkedAsQACompleteWithQATasksNotClosed()
        {
            Console.WriteLine("getTPEntitiesMarkedAsQACompleteWithQATasksNotClosed");

            ProcessResponse response = targetProcessSprintEntitiesFullHistoryWithoutBugs();
            if (response != null && !response.IsSuccess)
            {
                Console.WriteLine($"Error: {response.SystemError}\r\n{response.ErrorDescription}");
                return;
            }

            response = targetProcessSprintEntities();
            if (response != null && !response.IsSuccess)
            {
                Console.WriteLine($"Error: {response.SystemError}\r\n{response.ErrorDescription}");
                return;
            }

            if (_targetProcessItemStateHistoryList.Items.Any())
            {
                List<TargetProcessItemStateHistory> entityWithHistoryList = _targetProcessItemStateHistoryList.Items.Where(entity => entity.EntityState.Name.IndexOf("Performance Testing Complete") > -1).ToList(); //Create new entity list and put all items from _TargetProcessItemList
                List<TargetProcessItem> entityList = _targetProcessItemList.Items.Where(entity => entity.Id != null).ToList(); 
                List<TargetProcessItemStateHistory> targetProcessHistoryListDistinct = new List<TargetProcessItemStateHistory>();
                bool uniqueId = true;
                int currentId = 0;
                foreach (TargetProcessItemStateHistory targetProcessItem in entityWithHistoryList)
                {
                    if (currentId == targetProcessItem.UserStory.Id)
                    {
                        uniqueId = false;
                    }
                    else
                    {
                        uniqueId = true;
                    }

                    if (uniqueId == true)
                    {
                        foreach (TargetProcessItem item in entityList)
                        {
                            if (item.Id.Equals(targetProcessItem.UserStory.Id))
                            {
                                if (!_listOfStatesBeforeQAComplete.Contains(item.State.Name))
                                {
                                    targetProcessHistoryListDistinct.Add(targetProcessItem);
                                    currentId = targetProcessItem.UserStory.Id;
                                }                               
                            }
                        }
                    }
                }
                List<TargetProcessItemStateHistory> userStoryHasNotDoneQATasksList = new List<TargetProcessItemStateHistory>();
                bool hasUnfinishedTasks = false;
                bool unfinishedTaskLinkedToQA = false;
                bool qaTaskHasNoEffort = false;
                foreach (TargetProcessItemStateHistory entity in targetProcessHistoryListDistinct)

                {
                    targetProcessTaskEntities(entity.UserStory.Id);
                    targetProcessAssignmentsforEntity(entity.UserStory.Id);
                    List<TargetProcessTaskItem> taskListForUserStory = _targetProcessItemTaskList.Items.Where(entity => entity.Id != null).ToList();
                    List<TargetProcessAssignmentItem> assignmentListForUserStory = _targetProcessAssignmentsTaskList.Assignments.Items.Where(assignment => assignment.Role.Id.Equals(9)).ToList();
                    foreach (TargetProcessTaskItem item in taskListForUserStory)
                    {
                        if (item.EntityState.Name.ToLower() != "done")
                        {
                            hasUnfinishedTasks = true;
                        }
                        if (hasUnfinishedTasks)
                        {
                            foreach (TargetProcessAssignmentItem assignedDev in assignmentListForUserStory)
                            {
                                if (item.AssignedUsers.Items.Where(assignedToTask => assignedToTask.FullName.Equals(assignedDev.GeneralUser.FullName)).Any())
                                {
                                    unfinishedTaskLinkedToQA = true;
                                    if (item.Effort.Equals(0))
                                    {
                                        qaTaskHasNoEffort = true;
                                    }
                                }
                            }
                        }
                        hasUnfinishedTasks = false;
                    }
                    if (unfinishedTaskLinkedToQA || qaTaskHasNoEffort)
                    {
                        userStoryHasNotDoneQATasksList.Add(entity);
                        unfinishedTaskLinkedToQA = false;
                        qaTaskHasNoEffort = false;
                    }
                }
                printEntities($"Found {{0}} Target Process Entities Where Performance Testing Complete State and not closed QA Tasks", userStoryHasNotDoneQATasksList);
                AsciiFileCreator.UpdateExcelFileForTasks(userStoryHasNotDoneQATasksList, 10, Int32.Parse(_sprint));
                Console.WriteLine("All done - getTPEntitiesMarkedAsQACompleteWithQATasksNotClosed");
                GC.Collect();
            }
        }

        private static void getTPEntitiesMarkedAsDoneWithNoDevTasks()
        {
            Console.WriteLine("getTPEntitiesMarkedAsDoneWithNoDevTasks");

            ProcessResponse response = targetProcessSprintEntitiesFullHistoryWithoutBugs();
            if (response != null && !response.IsSuccess)
            {
                Console.WriteLine($"Error: {response.SystemError}\r\n{response.ErrorDescription}");
                return;
            }

            if (_targetProcessItemStateHistoryList.Items.Any())
            {
                List<TargetProcessItemStateHistory> entityList = _targetProcessItemStateHistoryList.Items.Where(entity => entity.EntityState.Name.IndexOf("Done") > -1).ToList(); //Create new entity list and put all items from _TargetProcessItemList
                List<TargetProcessItemStateHistory> targetProcessHistoryListDistinct = new List<TargetProcessItemStateHistory>();
                bool uniqueId = true;
                int currentId = 0;
                foreach (TargetProcessItemStateHistory targetProcessItem in entityList)
                {
                    if (currentId == targetProcessItem.UserStory.Id)
                    {
                        uniqueId = false;
                    }
                    else
                    {
                        uniqueId = true;
                    }

                    if (uniqueId == true)
                    {
                        targetProcessHistoryListDistinct.Add(targetProcessItem);
                        currentId = targetProcessItem.UserStory.Id;
                    }
                }
                List<TargetProcessItemStateHistory> userStoryHasNoDevTasks = new List<TargetProcessItemStateHistory>();
                bool hasDevTasks = false;
                foreach (TargetProcessItemStateHistory entity in targetProcessHistoryListDistinct)
                {
                    targetProcessTaskEntities(entity.UserStory.Id);
                    targetProcessAssignmentsforEntity(entity.UserStory.Id);
                    List<TargetProcessTaskItem> taskListForUserStory = _targetProcessItemTaskList.Items.Where(entity => entity.Id != null).ToList();
                    List<TargetProcessAssignmentItem> assignmentListForUserStory = _targetProcessAssignmentsTaskList.Assignments.Items.Where(assignment => assignment.Role.Id.Equals(1)).ToList();
                    if (assignmentListForUserStory.Any())
                    {
                        foreach (TargetProcessTaskItem item in taskListForUserStory)
                        {
                            foreach (TargetProcessAssignmentItem assignedDev in assignmentListForUserStory)
                            {
                                if (item.AssignedUsers.Items.Where(assignedToTask => assignedToTask.FullName.Equals(assignedDev.GeneralUser.FullName)).Any())
                                {
                                    hasDevTasks = true;                                    
                                }
                            }
                        }
                    }                   
                    if (!hasDevTasks && assignmentListForUserStory.Any())
                    {
                        userStoryHasNoDevTasks.Add(entity);
                    }
                }
                printEntities($"Found {{0}} Target Process Entities Where Dev Assigned, but no Dev Tasks created", userStoryHasNoDevTasks);
                AsciiFileCreator.UpdateExcelFileForTasks(userStoryHasNoDevTasks, 11, Int32.Parse(_sprint));
                Console.WriteLine("All done - getTPEntitiesMarkedAsDoneWithNoDevTasks");
                GC.Collect();
            }
        }

        private static void getTPEntitiesMarkedAsDoneWithNoQATasks()
        {
            Console.WriteLine("getTPEntitiesMarkedAsDoneWithNoQATasks");

            ProcessResponse response = targetProcessSprintEntitiesFullHistoryWithoutBugs();
            if (response != null && !response.IsSuccess)
            {
                Console.WriteLine($"Error: {response.SystemError}\r\n{response.ErrorDescription}");
                return;
            }

            if (_targetProcessItemStateHistoryList.Items.Any())
            {
                List<TargetProcessItemStateHistory> entityList = _targetProcessItemStateHistoryList.Items.Where(entity => entity.EntityState.Name.IndexOf("Done") > -1).ToList(); //Create new entity list and put all items from _TargetProcessItemList
                List<TargetProcessItemStateHistory> targetProcessHistoryListDistinct = new List<TargetProcessItemStateHistory>();
                bool uniqueId = true;
                int currentId = 0;
                foreach (TargetProcessItemStateHistory targetProcessItem in entityList)
                {
                    if (currentId == targetProcessItem.UserStory.Id)
                    {
                        uniqueId = false;
                    }
                    else
                    {
                        uniqueId = true;
                    }

                    if (uniqueId == true)
                    {
                        targetProcessHistoryListDistinct.Add(targetProcessItem);
                        currentId = targetProcessItem.UserStory.Id;
                    }
                }
                List<TargetProcessItemStateHistory> userStoryHasNoQATasks = new List<TargetProcessItemStateHistory>();
                bool hasQATasks = false;
                foreach (TargetProcessItemStateHistory entity in targetProcessHistoryListDistinct)
                {
                    targetProcessTaskEntities(entity.UserStory.Id);
                    targetProcessAssignmentsforEntity(entity.UserStory.Id);
                    List<TargetProcessTaskItem> taskListForUserStory = _targetProcessItemTaskList.Items.Where(entity => entity.Id != null).ToList();
                    List<TargetProcessAssignmentItem> assignmentListForUserStory = _targetProcessAssignmentsTaskList.Assignments.Items.Where(assignment => assignment.Role.Id.Equals(9)).ToList();
                    if (assignmentListForUserStory.Any())
                    {
                        foreach (TargetProcessTaskItem item in taskListForUserStory)
                        {
                            foreach (TargetProcessAssignmentItem assignedDev in assignmentListForUserStory)
                            {
                                if (item.AssignedUsers.Items.Where(assignedToTask => assignedToTask.FullName.Equals(assignedDev.GeneralUser.FullName)).Any())
                                {
                                    hasQATasks = true;
                                }
                            }
                        }
                    }
                    if (!hasQATasks && assignmentListForUserStory.Any())
                    {
                        userStoryHasNoQATasks.Add(entity);
                    }
                }
                printEntities($"Found {{0}} Target Process Entities Where QA Assigned, but no QA Tasks created", userStoryHasNoQATasks);
                AsciiFileCreator.UpdateExcelFileForTasks(userStoryHasNoQATasks, 12, Int32.Parse(_sprint));
                Console.WriteLine("All done - getTPEntitiesMarkedAsDoneWithNoQATasks");
                GC.Collect();
            }
        }

        private static void getCurrentSprintCommitsPlacedIntoPreviousReleaseCandidate()
        {
            int currentSprint = Int32.Parse(_sprint);
            string futureSprint = (currentSprint + 1).ToString();
            Console.WriteLine("Starting - getCurrentSprintCommitsPlacedIntoPreviousReleaseCandidate");
            if (!linkCommitsUserStoriesCurrentTeamSprintReleaseCandidateBranch())
            {
                Console.WriteLine("Error exit.");
                return;
            }

            ProcessResponse response = targetProcessEntitiesByUserStoryName($"ADMIN: Sprint {_sprint}");
            if (response != null && !response.IsSuccess)
            {
                Console.WriteLine($"Error: {response.SystemError}\r\n{response.ErrorDescription}");
                return;
            }

            _commitsToCheck = _commits.Where(commit => !commit.IsMerge && commit.CommitUSId != null).
                OrderBy(commit => commit.CommitUSId).
                OrderByDescending(commit => commit.IsInDevelopment).ToList();
            if (!findUSbyUSId(_commitsToCheck))
            {
                Console.WriteLine("Error exit.");
                return;
            }

            List<TargetProcessItem> entityList = _targetProcessUserStoryByNameItemList.Items.Where(entity => entity.Id != null).ToList();
            List<CommitTargetProcessItem> _commitsToPrint = new List<CommitTargetProcessItem>();

            foreach (CommitTargetProcessItem item in _commitsToCheck)
            {
                foreach (int tpItem in entityList.Select(x => x.Id).Distinct().ToList()) // Make sure no 2 users stories have the same exact name
                {
                    if (item.CommitUSId != tpItem)
                    {
                        if (item.UserStory != null && !item.UserStory.ResourceType.Equals("Request") && item.UserStory.TeamIteration.Name != null && item.UserStory.TeamIteration.Name.Contains(_sprint))
                            _commitsToPrint.Add(item);
                    }
                }
            }
            printCommitInfo("Number of current sprint commits in previous Release Candidate {0}", _commitsToPrint);
            AsciiFileCreator.UpdateExcelFile(_commitsToPrint, EnumOutputFileType.WrongTeamSprint, 13, Int32.Parse(_sprint));
            Thread.Sleep(5000);
            Console.WriteLine("All done - getCurrentSprintCommitsPlacedIntoPreviousReleaseCandidate");
            GC.Collect();
        }

        private static void getUSWithoutDeveloperScreenshots()
        {
            Console.WriteLine("getTPEntitiesInDevCompleteWithoutDevTestingScreenshots");

            ProcessResponse response = targetProcessSprintEntitiesFullHistoryWithoutBugs();
            if (response != null && !response.IsSuccess)
            {
                Console.WriteLine($"Error: {response.SystemError}\r\n{response.ErrorDescription}");
                return;
            }

            if (_targetProcessItemStateHistoryList.Items.Any())
            {
                List<TargetProcessItemStateHistory> entityList = _targetProcessItemStateHistoryList.Items.Where(entity => entity.EntityState.Name.IndexOf("Development Complete") > -1).ToList(); //Create new entity list and put all items from _TargetProcessItemList
                List<TargetProcessItemStateHistory> targetProcessHistoryListDistinct = new List<TargetProcessItemStateHistory>();
                bool uniqueId = true;
                int currentId = 0;
                foreach (TargetProcessItemStateHistory targetProcessItem in entityList)
                {
                    if (currentId == targetProcessItem.UserStory.Id)
                    {
                        uniqueId = false;
                    }
                    else
                    {
                        uniqueId = true;
                    }

                    if (uniqueId == true)
                    {
                        targetProcessHistoryListDistinct.Add(targetProcessItem);
                        currentId = targetProcessItem.UserStory.Id;
                    }
                }
                List<TargetProcessItemStateHistory> entitiesWithoutDeveloperScreenshot = new List<TargetProcessItemStateHistory>();
                bool containsDevScreenshot = false;
                foreach (TargetProcessItemStateHistory userStory in targetProcessHistoryListDistinct)
                {
                    targetProcessCommentEntriesPerUS(userStory.UserStory.Id);                    
                    List<TargetProcessCommentItemsForUS> commentList = _targetProcessCommentListForUS.Items;
                    if (commentList.Count > 0)
                    {
                        List<TargetProcessCommentsDetails> commentsDetails = commentList[0].commentsDetails.ToList();
                        targetProcessAssignmentsforEntity(userStory.UserStory.Id);
                        List<TargetProcessAssignmentItem> assignmentListForUserStory = _targetProcessAssignmentsTaskList.Assignments.Items.Where(assignment => assignment.Role.Id.Equals(1)).ToList();
                        foreach (TargetProcessCommentsDetails commentDetails in commentsDetails)
                        {
                            foreach (TargetProcessAssignmentItem assignedDev in assignmentListForUserStory)
                            {
                                if (commentDetails.commentOwnerDetails.fullName.Equals(assignedDev.GeneralUser.FullName) && commentDetails.Description.Contains("<img src="))
                                {
                                    containsDevScreenshot = true;
                                }
                            }
                        }                       
                    }
                    if (!containsDevScreenshot)
                    {
                        entitiesWithoutDeveloperScreenshot.Add(userStory);
                    }
                }
                printEntities($"Found {{0}} Target Process Entities State is Dev Complete and no Dev Screenshots have been added", entitiesWithoutDeveloperScreenshot);
                AsciiFileCreator.UpdateExcelFileForTasks(entitiesWithoutDeveloperScreenshot, 14, Int32.Parse(_sprint));
                Console.WriteLine("All Done - getTPEntitiesInDevCompleteWithoutDevTestingScreenshots");
                GC.Collect();
            }
        }

        private static void getListOfAllTeamCityBuildsForThisSprint()
        {
            // Set the base URL for the TeamCity REST API
            string baseUrl = "https://bg-teamcity.placeholder.com/app/rest/";            
          
            // Create the URL for the API call
            string url = $"{baseUrl}builds?fields=build(buildType,startDate)&count=2000";

            // Create a new HttpClient object
            using (var client = new HttpClient())
            {
                // Add the basic authentication headers
                client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", $"{_patTeamCity}");
                client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

                // Send the GET request to the API
                var response = client.GetAsync(url).Result;

                // Read the response content as a string
                var responseContent = response.Content.ReadAsStringAsync().Result;

                // Deserialize the JSON string into a JObject
                TeamCityRoot builds = CommonUtilities.DeserializeJsontoObject<TeamCityRoot>((string)responseContent);

                DateTime cutoffDate = new DateTime(2023, 1, 20);
                 
                List<TeamCityBuilds> filteredBuilds = builds.Builds.Where(b => DateTime.ParseExact(b.startDate.Replace(" ", ""), "yyyyMMdd'T'HHmmsszzz", CultureInfo.InvariantCulture) > cutoffDate).ToList();
                List<TeamCityBuilds> finalBuilds = filteredBuilds.GroupBy(o => new { o.buildType.WebUrl, o.buildType.Href, 
                    o.buildType.ProjectId, o.buildType.ProjectName,o.buildType.Description,o.buildType.Name}).Select(o => o.FirstOrDefault()).ToList();

                 AsciiFileCreator.UpdateExcelFileForTasks(finalBuilds, 15, Int32.Parse(_sprint));
                 Console.WriteLine("All Done - getListOfAllTeamCityBuildsForThisSprint");
                 GC.Collect();
            }
        }

        private static void getListOfActiveBranchesForCurrentSprint()
        {
            GitRef tagReference = null;
            VssBasicCredential credentials = new VssBasicCredential(string.Empty, _patAzureDevops);
            VssConnection connection = new VssConnection(new Uri(string.Format("https://dev.azure.com/{0}", "PlaceHolder")), credentials);
            using (GitHttpClient gitClient = connection.GetClient<GitHttpClient>())
            {
                GitRepository repo = gitClient.GetRepositoryAsync("PlaceHolder", _repoName).Result;
                var branches = gitClient.GetBranchesAsync(repo.Id).Result;
                var refTags = gitClient.GetRefsAsync(repo.Id, filterContains: _signOffTagFrom, peelTags: true).Result;
                foreach (GitRef reference in refTags){
                    if (reference.Name.Equals($"refs/tags/{_signOffTagFrom}"))
                    {
                        tagReference = reference;
                    }
                }
                string commitId = tagReference.PeeledObjectId ?? tagReference.ObjectId;
                GitCommit commit = gitClient.GetCommitAsync(commitId, repo.Id).Result;
                if (commit == null)
                { throw new Exception($"ERROR_COMMIT_TAG_WAS_NOT_FOUND + { _signOffTagFrom }"); }                
                foreach (GitBranchStats branch in branches)
                {
                   if (DateTime.Compare(branch.Commit.Author.Date, commit.Push.Date) > 0)
                    {
                        _currentSprintBranches.Add(branch);
                    }
                }
            }            
        }

        private static bool linkCommitsUserStoriesCurrentTeamSprint()
        {
            ProcessResponse response =  targetProcessSprintEntities();
            if (response != null && !response.IsSuccess)
            {
                Console.WriteLine($"Error: {response.SystemError}\r\n{response.ErrorDescription}");
                return false;
            }            
            if (!talkToAzureDevOps())
                { return false; }
            Console.WriteLine($"All commits: {_commits.Count}");
            Console.WriteLine($"Merge commits: {_commits.Where(commit => commit.IsMerge).Count()}");
            Console.WriteLine($"Not Merge commits: {_commits.Where(commit => !commit.IsMerge).Count()}");
            linkUserStoriestoCommits();
            return true;
        }

        private static bool linkCommitsUserStoriesCurrentTeamSprintDevelopementBranch()
        {
            ProcessResponse response = targetProcessSprintEntities();
            if (response != null && !response.IsSuccess)
            {
                Console.WriteLine($"Error: {response.SystemError}\r\n{response.ErrorDescription}");
                return false;
            }
            if (!talkToAzureDevOpsForDevelopementBranch())
            { return false; }
            Console.WriteLine($"All commits: {_commits.Count}");
            Console.WriteLine($"Merge commits: {_commits.Where(commit => commit.IsMerge).Count()}");
            Console.WriteLine($"Not Merge commits: {_commits.Where(commit => !commit.IsMerge).Count()}");
            linkUserStoriestoCommits();
            return true;
        }

        private static bool linkCommitsUserStoriesCurrentTeamSprintReleaseCandidateBranch()
        {
            ProcessResponse response = targetProcessSprintEntities();
            if (response != null && !response.IsSuccess)
            {
                Console.WriteLine($"Error: {response.SystemError}\r\n{response.ErrorDescription}");
                return false;
            }
            if (!talkToAzureDevOpsForReleaseBranch())
            { return false; }
            Console.WriteLine($"All commits: {_commits.Count}");
            Console.WriteLine($"Merge commits: {_commits.Where(commit => commit.IsMerge).Count()}");
            Console.WriteLine($"Not Merge commits: {_commits.Where(commit => !commit.IsMerge).Count()}");
            linkUserStoriestoCommits();
            return true;
        }

        private static bool linkCommitsUserStoriesHistoryCurrentTeamSprint()
        {
            ProcessResponse response = targetProcessSprintEntitiesFullHistory();
            if (response != null && !response.IsSuccess)
            {
                Console.WriteLine($"Error getting list of state histories: {response.SystemError}\r\n{response.ErrorDescription}");
                return false;
            }

            List <TargetProcessItemStateHistory> targetProcessHistoryList = new List<TargetProcessItemStateHistory> ();
            targetProcessHistoryList = _targetProcessItemStateHistoryList.Items.Where(entity => entity.EntityState.Name.ToLower().IndexOf("done") > -1 || entity.EntityState.Name.ToLower().IndexOf("rejected") > -1).Distinct().ToList();

            List<TargetProcessItemStateHistory> targetProcessHistoryListDistinct = new List<TargetProcessItemStateHistory>();

            bool uniqueId = true;
            int currentId = 0;
            foreach(TargetProcessItemStateHistory targetProcessItem in targetProcessHistoryList)
            {
                if (currentId == targetProcessItem.UserStory.Id)
                {
                    uniqueId = false;
                }
                else
                {
                    uniqueId = true;
                }

                if (uniqueId == true)
                {
                    targetProcessHistoryListDistinct.Add(targetProcessItem);                                     
                    currentId = targetProcessItem.UserStory.Id;
                }
            }
            if (!talkToAzureDevOps())
            { return false; }
            Console.WriteLine($"All commits: {_commits.Count}");
            Console.WriteLine($"Merge commits: {_commits.Where(commit => commit.IsMerge).Count()}");
            Console.WriteLine($"Not Merge commits: {_commits.Where(commit => !commit.IsMerge).Count()}");
            linkUserStoriesWithHistorytoCommits(targetProcessHistoryListDistinct);
            return true;
        }      

        static void initializeTeams()
        {
            allDeveloperList.Add(teamOneDeveloperList);
            allDeveloperList.Add(teamFourDeveloperList);
            allDeveloperList.Add(teamTwoDeveloperList);            
            allDeveloperList.Add(teamThreeDeveloperList);
            allDeveloperList.Add(teamFiveDeveloperList);
            allDeveloperList.Add(teamSixDeveloperList);
            allDeveloperList.Add(teamSevenDeveloperList);
            allDeveloperList.Add(teamEightDeveloperList);
            allDeveloperList.Add(teamNineDeveloperList);
       
            teamSprintList.AddRange(_teamSprintNames);
            teamNameList.AddRange(_teamNames);

            int x = 0;
            foreach (string teamName in teamNameList)
            {               
                ProcessResponse response = targetProcessDeveloperListPerTeam(teamName);
                if (response != null && !response.IsSuccess)
                {
                    Console.WriteLine($"Error getting list of sprint entities: {response.SystemError}\r\n{response.ErrorDescription}");
                    return;
                }
                List<TargetProcessTeamMemberItem> teamNameList = _targetProcessDeveloperListPerTeam.Items.ToList();                       
                foreach (TargetProcessTeamMemberItem item in teamNameList)
                {
                    allDeveloperList[x].Add($"{item.User.firstName} {item.User.lastName}");
                }
                x++;
            }            
            string[] solutionsArchitectDevelopers = new string[] { "Erik Frantz", "Niraj Patel", "Alex Akulich", "Oybek Khakimjanov" };
            teamNineDeveloperList.AddRange(solutionsArchitectDevelopers);           
        }
       
        private static bool talkToAzureDevOps()
        {
            CommitHelper commitHelper = new CommitHelper(_patAzureDevops, _organization, _project, _repoName);
            commitHelper.GetCommits(_signOffTagFrom, _signOffTagTo);
            if (!commitHelper.Response.IsSuccess)
            {
                Console.Write($"Error: {commitHelper.Response.SystemError}{commitHelper.Response.ErrorDescription}");
                Console.Write($"Log: {commitHelper.Response.Log}");
                Console.ReadKey();
                return false;
            }
            List<GitCommitRef> commitsCurrentSprint = (List<GitCommitRef>)commitHelper.Response.ReturnedResponseObject;

            commitHelper.GetCommits(tagSelectFrom: _signOffTagFrom, versionType: GitVersionType.Branch, version: "Development", tagSelectTo: _signOffTagTo);
            if (!commitHelper.Response.IsSuccess)
            {
                Console.Write($"Error: {commitHelper.Response.SystemError}{commitHelper.Response.ErrorDescription}");
                Console.Write($"Log: {commitHelper.Response.Log}");
                Console.ReadKey();
                return false;
            }
            List<GitCommitRef> commitsFilteredByDateandBranch = (List<GitCommitRef>)commitHelper.Response.ReturnedResponseObject;
            _commits = (
                from commitCurrentSprint in commitsCurrentSprint
                join commitFilteredByDateandBranch in commitsFilteredByDateandBranch
                    on commitCurrentSprint.CommitId equals commitFilteredByDateandBranch.CommitId into commits_joined
                from commitFilteredByDateandBranch in commits_joined.DefaultIfEmpty()
                select new CommitTargetProcessItem
                {
                    CommitId = commitCurrentSprint.CommitId,
                    IsMerge = (commitCurrentSprint.Comment.Length > 4 && (commitCurrentSprint.Comment.Substring(0, 5).ToLower() == "merge" ||
                       commitCurrentSprint.Comment.ToLower().IndexOf("merge conflict") > -1 ||
                       commitCurrentSprint.Comment.ToLower().IndexOf("merge issue") > -1)) ? true : false,
                    Commit = commitCurrentSprint,
                    IsInDevelopment = commitFilteredByDateandBranch != null ? true : false
                }).ToList();       
             
            return true;
        }

        private static bool talkToAzureDevOpsForDevelopementBranch()
        {
            CommitHelper commitHelper = new CommitHelper(_patAzureDevops, _organization, _project, _repoName);
            commitHelper.GetCommits(_signOffTagFrom, _signOffTagTo);
            if (!commitHelper.Response.IsSuccess)
            {
                Console.Write($"Error: {commitHelper.Response.SystemError}{commitHelper.Response.ErrorDescription}");
                Console.Write($"Log: {commitHelper.Response.Log}");
                Console.ReadKey();
                return false;
            }
            List<GitCommitRef> commitsCurrentSprint = (List<GitCommitRef>)commitHelper.Response.ReturnedResponseObject;

            commitHelper.GetCommits(tagSelectFrom: _signOffTagFrom, versionType: GitVersionType.Branch, version: "Development", tagSelectTo: _signOffTagTo);
            if (!commitHelper.Response.IsSuccess)
            {
                Console.Write($"Error: {commitHelper.Response.SystemError}{commitHelper.Response.ErrorDescription}");
                Console.Write($"Log: {commitHelper.Response.Log}");
                Console.ReadKey();
                return false;
            }
            List<GitCommitRef> commitsFilteredByDateandBranch = (List<GitCommitRef>)commitHelper.Response.ReturnedResponseObject;

            _commits = (from commitFilteredByDateandBranch in commitsFilteredByDateandBranch.DefaultIfEmpty()
                        select new CommitTargetProcessItem
                        {
                            CommitId = commitFilteredByDateandBranch.CommitId,
                            IsMerge = (commitFilteredByDateandBranch.Comment.Length > 4 && (commitFilteredByDateandBranch.Comment.Substring(0, 5).ToLower() == "merge" ||
                               commitFilteredByDateandBranch.Comment.ToLower().IndexOf("merge conflict") > -1 ||
                               commitFilteredByDateandBranch.Comment.ToLower().IndexOf("merge issue") > -1)) ? true : false,
                            Commit = commitFilteredByDateandBranch,
                            IsInDevelopment = true
                        }).ToList();
            return true;
        }

        private static bool talkToAzureDevOpsForReleaseBranch()
        {
            CommitHelper commitHelper = new CommitHelper(_patAzureDevops, _organization, _project, _repoName);
            commitHelper.GetCommits(_signOffTagFrom, _signOffTagTo);
            if (!commitHelper.Response.IsSuccess)
            {
                Console.Write($"Error: {commitHelper.Response.SystemError}{commitHelper.Response.ErrorDescription}");
                Console.Write($"Log: {commitHelper.Response.Log}");
                Console.ReadKey();
                return false;
            }
            List<GitCommitRef> commitsCurrentSprint = (List<GitCommitRef>)commitHelper.Response.ReturnedResponseObject;

            commitHelper.GetCommits(tagSelectFrom: _signOffTagFrom, versionType: GitVersionType.Branch, version: $"ReleaseCandidate-{_previousSprint}", tagSelectTo: _signOffTagTo);
            if (!commitHelper.Response.IsSuccess)
            {
                Console.Write($"Error: {commitHelper.Response.SystemError}{commitHelper.Response.ErrorDescription}");
                Console.Write($"Log: {commitHelper.Response.Log}");
                Console.ReadKey();
                return false;
            }
            List<GitCommitRef> commitsFilteredByDateandBranch = (List<GitCommitRef>)commitHelper.Response.ReturnedResponseObject;

            if (commitsFilteredByDateandBranch.Count() > 0)
            {
                _commits = (from commitFilteredByDateandBranch in commitsFilteredByDateandBranch.DefaultIfEmpty()
                            select new CommitTargetProcessItem
                            {
                                CommitId = commitFilteredByDateandBranch.CommitId,
                                IsMerge = (commitFilteredByDateandBranch.Comment.Length > 4 && (commitFilteredByDateandBranch.Comment.Substring(0, 5).ToLower() == "merge" ||
                                   commitFilteredByDateandBranch.Comment.ToLower().IndexOf("merge conflict") > -1 ||
                                   commitFilteredByDateandBranch.Comment.ToLower().IndexOf("merge issue") > -1)) ? true : false,
                                Commit = commitFilteredByDateandBranch,
                                IsInDevelopment = true
                            }).ToList();
            }          
            return true;
        }

        private static bool findUSbyUSId(List<CommitTargetProcessItem> commits)
        {
            List<int> usIds = commits.Select(commit => (int)commit.CommitUSId).Distinct().ToList();

            ProcessResponse response = targetProcessGetEntityDetails(usIds, EnumEntityType.UserStoryPlural);
            if (!response.IsSuccess)
            {
                Console.WriteLine($"Error: {response.SystemError}\r\n{response.ErrorDescription}");
                return false; 
            }
            TargetProcessItemList userStories = CommonUtilities.DeserializeJsontoObject<TargetProcessItemList>((string)response.ReturnedResponseObject);
          
            if (userStories.Items.Any())
            {
                commits.ForEach(commit => 
                {
                    TargetProcessItem targetProcessItem = userStories.Items.Where(item => item.Id == commit.CommitUSId).FirstOrDefault();
                    if (targetProcessItem != null)
                    {
                        commit.UserStory = targetProcessItem;
                        usIds.Remove(targetProcessItem.Id);
                    }
                });
            }
            
            if(usIds.Any())
            {
                response = targetProcessGetEntityDetails(usIds, EnumEntityType.BugPlural);
                if (!response.IsSuccess)
                {
                    Console.WriteLine($"Error: {response.SystemError}\r\n{response.ErrorDescription}");
                    return false;
                }
                TargetProcessItemList bugs = CommonUtilities.DeserializeJsontoObject<TargetProcessItemList>((string)response.ReturnedResponseObject);
                if (bugs.Items.Any())
                {
                    commits.Where(commit => commit.UserStory == null).ForEach(commit =>
                    {
                        TargetProcessItem targetProcessItem = bugs.Items.Where(item => item.Id == commit.CommitUSId).FirstOrDefault();
                        if (targetProcessItem != null)
                        {
                            commit.UserStory = targetProcessItem;
                            usIds.Remove(targetProcessItem.Id);
                        }
                    });
                }
            }

            if (usIds.Any())
            {
                response = targetProcessGetEntityDetails(usIds, EnumEntityType.TaskPlural);
                if (!response.IsSuccess)
                {
                    Console.WriteLine($"Error: {response.SystemError}\r\n{response.ErrorDescription}");
                    return false;
                }
                TargetProcessItemList tasks = CommonUtilities.DeserializeJsontoObject<TargetProcessItemList>((string)response.ReturnedResponseObject);
                if (tasks.Items.Any())
                {
                    commits.Where(commit => commit.UserStory == null).ForEach(commit =>
                    {
                        TargetProcessItem targetProcessItem = tasks.Items.Where(item => item.Id == commit.CommitUSId).FirstOrDefault();
                        if (targetProcessItem != null)
                        {
                            commit.UserStory = targetProcessItem;
                            usIds.Remove(targetProcessItem.Id);
                        }
                    });
                }
            }

            if (usIds.Any())
            {
                response = targetProcessGetEntityDetails(usIds, EnumEntityType.RequestPlural);
                if (!response.IsSuccess)
                {
                    Console.WriteLine($"Error: {response.SystemError}\r\n{response.ErrorDescription}");
                    return false;
                }
                TargetProcessItemList requests = CommonUtilities.DeserializeJsontoObject<TargetProcessItemList>((string)response.ReturnedResponseObject);
                if (requests.Items.Any())
                {
                    commits.Where(commit => commit.UserStory == null).ForEach(commit =>
                    {
                        TargetProcessItem targetProcessItem = requests.Items.Where(item => item.Id == commit.CommitUSId).FirstOrDefault();
                        if (targetProcessItem != null)
                        {
                            commit.UserStory = targetProcessItem;
                            usIds.Remove(targetProcessItem.Id);
                        }
                    });
                }
            }

            return true;
        }

        private static void printCommitInfo(string header, List<CommitTargetProcessItem> commits)
        {
            int index = 0;
            Console.WriteLine("\r\n\r\n\r\n");
            Console.WriteLine(string.Format(header, commits.Count));
            string notFound = "--";
            foreach (CommitTargetProcessItem commit in commits)
            {
                index++;
                Console.WriteLine($"{index}. Commit comment: {commit.Commit.Comment}\r\n  " +
                    $"Exists in Development branch: {commit.IsInDevelopment}\r\n  " +
                    $"Author: {commit.Commit.Author.Email}\r\n  " +
                    $"Target Process Sprint: {(commit.UserStory == null ? notFound : commit.UserStory.TeamIteration.Name)}\r\n  " +
                    $"Target Process State: {(commit.UserStory == null ? notFound : commit.UserStory.State.Name)}\r\n  " +
                    $"Commit Id: {commit.CommitId}\r\n  " +
                    $"US Id: {(commit.CommitUSId == null ? notFound : commit.CommitUSId)}\r\n  " +
                    $"Target Process Type: {(commit.UserStory == null ? notFound : commit.UserStory.ResourceType)}\r\n  " +
                    $"Url: {commit.Commit.RemoteUrl}\r\n");

                string developerName = (commit.Commit.Author.Email.Substring(0, commit.Commit.Author.Email.IndexOf("@"))).Replace(".", " ").ToLower();

                if (teamOneDeveloperList.ConvertAll(d => d.ToLower()).Contains(developerName))
                {
                    _commitsTeamOne.Add(commit);
                }
                else if (teamTwoDeveloperList.ConvertAll(d => d.ToLower()).Contains(developerName))
                {
                    _commitsTeamTwo.Add(commit);
                }
                else if (teamFourDeveloperList.ConvertAll(d => d.ToLower()).Contains(developerName))
                {
                    _commitsTeamFour.Add(commit);
                }
                else if (teamThreeDeveloperList.ConvertAll(d => d.ToLower()).Contains(developerName))
                {
                    _commitsTeamThree.Add(commit);
                }
                else if (teamFiveDeveloperList.ConvertAll(d => d.ToLower()).Contains(developerName))
                {
                    _commitsTeamFive.Add(commit);
                }
                else if (teamSixDeveloperList.ConvertAll(d => d.ToLower()).Contains(developerName))
                {
                    _commitsTeamSix.Add(commit);
                }
                else if (teamSevenDeveloperList.ConvertAll(d => d.ToLower()).Contains(developerName))
                {
                    _commitsTeamSeven.Add(commit);
                }
                else if (teamEightDeveloperList.ConvertAll(d => d.ToLower()).Contains(developerName))
                {
                    _commitsTeamEight.Add(commit);
                }
                else if (teamNineDeveloperList.ConvertAll(d => d.ToLower()).Contains(developerName))
                {
                    _commitsTeamNine.Add(commit);
                }
            }
        }

        private static void printEntities(string header, List<TargetProcessItem> entities)
        {
            int index = 0;
            Console.WriteLine("\r\n\r\n\r\n");
            Console.WriteLine(string.Format(header, entities.Count));
            foreach (TargetProcessItem entity in entities)
            {
                index++;
                Console.WriteLine($"{index}. Target Process Id: {entity.Id}\r\n  " +
                    $"Entity Type: {entity.ResourceType}\r\n  " +
                    $"Team Sprint: {entity.TeamIteration.Name}\r\n  " +
                    $"Entity State: {entity.State.Name}\r\n  ");
            }   
        }

        private static void printEntities(string header, List<TargetProcessItemStateHistory> entities)
        {
            int index = 0;
            Console.WriteLine("\r\n\r\n\r\n");
            Console.WriteLine(string.Format(header, entities.Select(entity=>entity.UserStory.Id).Distinct().ToList().Count));
            foreach (TargetProcessItemStateHistory entity in entities)
            {
                index++;
                Console.WriteLine($"{index}. Target Process Id: {entity.UserStory.Id}\r\n  " +
                    $"Entity Type: {entity.ResourceType}\r\n  " +
                    $"Date: {entity.Date.ToString("MM/dd/yyyy hh:mm:ss tt")}\r\n  " +
                    $"Entity State: {entity.EntityState.Name}\r\n  " + 
                    $"Team Sprint: {entity.TeamIteration.Name}");
            }
        }

        private static void linkUserStoriestoCommits()
        {
            foreach (CommitTargetProcessItem commitTP in _commits.Where(commit => !commit.IsMerge).ToList())
            {
                Regex pattern = new Regex(US_REGEX_POUND_6_DIGITS);
                Match match = pattern.Match(commitTP.Commit.Comment);
                if (!string.IsNullOrEmpty(match.Groups[0].Value))
                {
                    commitTP.CommitUSId = Convert.ToInt32(match.Groups[0].Value.Replace("#", ""));
                    commitTP.UserStory = _targetProcessItemList.Items.Where(item => item.Id == commitTP.CommitUSId).FirstOrDefault();
                    continue;
                }

                pattern = new Regex(US_REGEX_POUND_5_DIGITS);
                match = pattern.Match(commitTP.Commit.Comment);
                if (!string.IsNullOrEmpty(match.Groups[0].Value))
                {
                    commitTP.CommitUSId = Convert.ToInt32(match.Groups[0].Value.Replace("#", ""));
                    commitTP.UserStory = _targetProcessItemList.Items.Where(item => item.Id == commitTP.CommitUSId).FirstOrDefault();
                    continue;
                }

                pattern = new Regex(US_REGEX_US_6_DIGITS);
                match = pattern.Match(commitTP.Commit.Comment);
                if (!string.IsNullOrEmpty(match.Groups[0].Value))
                {
                    commitTP.CommitUSId = Convert.ToInt32(match.Groups[0].Value.Replace("US", ""));
                    commitTP.UserStory = _targetProcessItemList.Items.Where(item => item.Id == commitTP.CommitUSId).FirstOrDefault();
                    continue;
                }

                pattern = new Regex(US_REGEX_US_5_DIGITS);
                match = pattern.Match(commitTP.Commit.Comment);
                if (!string.IsNullOrEmpty(match.Groups[0].Value))
                {
                    commitTP.CommitUSId = Convert.ToInt32(match.Groups[0].Value.Replace("US", ""));
                    commitTP.UserStory = _targetProcessItemList.Items.Where(item => item.Id == commitTP.CommitUSId).FirstOrDefault();
                    continue;
                }

                pattern = new Regex(US_REGEX_POUND_US_6_DIGITS);
                match = pattern.Match(commitTP.Commit.Comment);
                if (!string.IsNullOrEmpty(match.Groups[0].Value))
                {
                    commitTP.CommitUSId = Convert.ToInt32(match.Groups[0].Value.Replace("#US", ""));
                    commitTP.UserStory = _targetProcessItemList.Items.Where(item => item.Id == commitTP.CommitUSId).FirstOrDefault();
                    continue;
                }

                pattern = new Regex(US_REGEX_POUND_US_5_DIGITS);
                match = pattern.Match(commitTP.Commit.Comment);
                if (!string.IsNullOrEmpty(match.Groups[0].Value))
                {
                    commitTP.CommitUSId = Convert.ToInt32(match.Groups[0].Value.Replace("#US", ""));
                    commitTP.UserStory = _targetProcessItemList.Items.Where(item => item.Id == commitTP.CommitUSId).FirstOrDefault();
                    continue;
                }

                pattern = new Regex(US_REGEX_POUND_SPACE_US_TAB_6_DIGITS);
                match = pattern.Match(commitTP.Commit.Comment);
                if (!string.IsNullOrEmpty(match.Groups[0].Value))
                {
                    commitTP.CommitUSId = Convert.ToInt32(match.Groups[0].Value.Replace("#US", ""));
                    commitTP.UserStory = _targetProcessItemList.Items.Where(item => item.Id == commitTP.CommitUSId).FirstOrDefault();
                    continue;
                }

                pattern = new Regex(US_REGEX_POUND_SPACE_US_TAB_5_DIGITS);
                match = pattern.Match(commitTP.Commit.Comment);
                if (!string.IsNullOrEmpty(match.Groups[0].Value))
                {
                    commitTP.CommitUSId = Convert.ToInt32(match.Groups[0].Value.Replace("#US", ""));
                    commitTP.UserStory = _targetProcessItemList.Items.Where(item => item.Id == commitTP.CommitUSId).FirstOrDefault();
                    continue;
                }

                pattern = new Regex(US_REGEX_POUND_SPACE_US_SINGLESPACE_6_DIGITS);
                match = pattern.Match(commitTP.Commit.Comment);
                if (!string.IsNullOrEmpty(match.Groups[0].Value))
                {
                    commitTP.CommitUSId = Convert.ToInt32(match.Groups[0].Value.Replace("#US ", ""));
                    commitTP.UserStory = _targetProcessItemList.Items.Where(item => item.Id == commitTP.CommitUSId).FirstOrDefault();
                    continue;
                }

                pattern = new Regex(US_REGEX_POUND_SPACE_US_SINGLESPACE_5_DIGITS);
                match = pattern.Match(commitTP.Commit.Comment);
                if (!string.IsNullOrEmpty(match.Groups[0].Value))
                {
                    commitTP.CommitUSId = Convert.ToInt32(match.Groups[0].Value.Replace("#US ", ""));
                    commitTP.UserStory = _targetProcessItemList.Items.Where(item => item.Id == commitTP.CommitUSId).FirstOrDefault();
                    continue;
                }

                  pattern = new Regex(US_REGEX_POUND_SPACE_US_DOUBLESPACE_6_DIGITS);
                  match = pattern.Match(commitTP.Commit.Comment);
                  if (!string.IsNullOrEmpty(match.Groups[0].Value))
                  {
                      commitTP.CommitUSId = Convert.ToInt32(match.Groups[0].Value.Replace("# US ", ""));
                      commitTP.UserStory = _targetProcessItemList.Items.Where(item => item.Id == commitTP.CommitUSId).FirstOrDefault();
                      continue;
                  }

                  pattern = new Regex(US_REGEX_POUND_SPACE_US_DOUBLESPACE_5_DIGITS);
                  match = pattern.Match(commitTP.Commit.Comment);
                  if (!string.IsNullOrEmpty(match.Groups[0].Value))
                  {
                      commitTP.CommitUSId = Convert.ToInt32(match.Groups[0].Value.Replace("# US ", ""));
                      commitTP.UserStory = _targetProcessItemList.Items.Where(item => item.Id == commitTP.CommitUSId).FirstOrDefault();
                      continue;
                  }

                pattern = new Regex(US_REGEX_POUND_BUG_6_DIGITS);
                match = pattern.Match(commitTP.Commit.Comment);
                if (!string.IsNullOrEmpty(match.Groups[0].Value))
                {
                    commitTP.CommitUSId = Convert.ToInt32(match.Groups[0].Value.Replace("#Bug", ""));
                    commitTP.UserStory = _targetProcessItemList.Items.Where(item => item.Id == commitTP.CommitUSId).FirstOrDefault();
                    continue;
                }

                pattern = new Regex(US_REGEX_POUND_BUG_5_DIGITS);
                match = pattern.Match(commitTP.Commit.Comment);
                if (!string.IsNullOrEmpty(match.Groups[0].Value))
                {
                    commitTP.CommitUSId = Convert.ToInt32(match.Groups[0].Value.Replace("#Bug", ""));
                    commitTP.UserStory = _targetProcessItemList.Items.Where(item => item.Id == commitTP.CommitUSId).FirstOrDefault();
                    continue;
                }

                pattern = new Regex(US_REGEX_POUND_B_6_DIGITS);
                match = pattern.Match(commitTP.Commit.Comment);
                if (!string.IsNullOrEmpty(match.Groups[0].Value))
                {
                    commitTP.CommitUSId = Convert.ToInt32(match.Groups[0].Value.Replace("#B", ""));
                    commitTP.UserStory = _targetProcessItemList.Items.Where(item => item.Id == commitTP.CommitUSId).FirstOrDefault();
                    continue;
                }

                pattern = new Regex(US_REGEX_POUND_B_5_DIGITS);
                match = pattern.Match(commitTP.Commit.Comment);
                if (!string.IsNullOrEmpty(match.Groups[0].Value))
                {
                    commitTP.CommitUSId = Convert.ToInt32(match.Groups[0].Value.Replace("#B", ""));
                    commitTP.UserStory = _targetProcessItemList.Items.Where(item => item.Id == commitTP.CommitUSId).FirstOrDefault();
                    continue;
                }

                pattern = new Regex(US_REGEX_POUND_SPACE_BUG_SINGLESPACE_6_DIGITS);
                match = pattern.Match(commitTP.Commit.Comment);
                if (!string.IsNullOrEmpty(match.Groups[0].Value))
                {
                    commitTP.CommitUSId = Convert.ToInt32(match.Groups[0].Value.Replace("#Bug ", ""));
                    commitTP.UserStory = _targetProcessItemList.Items.Where(item => item.Id == commitTP.CommitUSId).FirstOrDefault();
                    continue;
                }

                pattern = new Regex(US_REGEX_POUND_SPACE_BUG_SINGLESPACE_5_DIGITS);
                match = pattern.Match(commitTP.Commit.Comment);
                if (!string.IsNullOrEmpty(match.Groups[0].Value))
                {
                    commitTP.CommitUSId = Convert.ToInt32(match.Groups[0].Value.Replace("#Bug ", ""));
                    commitTP.UserStory = _targetProcessItemList.Items.Where(item => item.Id == commitTP.CommitUSId).FirstOrDefault();
                    continue;
                }
            }
        }

        private static void linkUserStoriesWithHistorytoCommits(List<TargetProcessItemStateHistory> targetProcessItemStateHistoryList)
        {
            foreach (CommitTargetProcessItem commitTP in _commits.Where(commit => !commit.IsMerge).ToList())
            {
                Regex pattern = new Regex(US_REGEX_POUND_6_DIGITS);
                Match match = pattern.Match(commitTP.Commit.Comment);
                if (!string.IsNullOrEmpty(match.Groups[0].Value))
                {
                    commitTP.CommitUSId = Convert.ToInt32(match.Groups[0].Value.Replace("#", ""));
                    foreach(TargetProcessItemStateHistory item in targetProcessItemStateHistoryList)
                    {
                        if (item.UserStory.Id == commitTP.CommitUSId && item.Date < commitTP.Commit.Committer.Date)
                            commitTP.UserStoryHistory = item;
                    }                                     
                    continue;
                }

                pattern = new Regex(US_REGEX_POUND_5_DIGITS);
                match = pattern.Match(commitTP.Commit.Comment);
                if (!string.IsNullOrEmpty(match.Groups[0].Value))
                {
                    commitTP.CommitUSId = Convert.ToInt32(match.Groups[0].Value.Replace("#", ""));
                    foreach (TargetProcessItemStateHistory item in targetProcessItemStateHistoryList)
                    {
                        if (item.UserStory.Id == commitTP.CommitUSId && item.Date < commitTP.Commit.Committer.Date)
                            commitTP.UserStoryHistory = item;
                    }
                    continue;
                }

                pattern = new Regex(US_REGEX_US_6_DIGITS);
                match = pattern.Match(commitTP.Commit.Comment);
                if (!string.IsNullOrEmpty(match.Groups[0].Value))
                {
                    commitTP.CommitUSId = Convert.ToInt32(match.Groups[0].Value.Replace("US", ""));
                    foreach (TargetProcessItemStateHistory item in targetProcessItemStateHistoryList)
                    {
                        if (item.UserStory.Id == commitTP.CommitUSId && item.Date < commitTP.Commit.Committer.Date)
                            commitTP.UserStoryHistory = item;
                    }
                    continue;
                }

                pattern = new Regex(US_REGEX_US_5_DIGITS);
                match = pattern.Match(commitTP.Commit.Comment);
                if (!string.IsNullOrEmpty(match.Groups[0].Value))
                {
                    commitTP.CommitUSId = Convert.ToInt32(match.Groups[0].Value.Replace("US", ""));
                    foreach (TargetProcessItemStateHistory item in targetProcessItemStateHistoryList)
                    {
                        if (item.UserStory.Id == commitTP.CommitUSId && item.Date < commitTP.Commit.Committer.Date)
                            commitTP.UserStoryHistory = item;
                    }
                    continue;
                }

                pattern = new Regex(US_REGEX_POUND_US_6_DIGITS);
                match = pattern.Match(commitTP.Commit.Comment);
                if (!string.IsNullOrEmpty(match.Groups[0].Value))
                {
                    commitTP.CommitUSId = Convert.ToInt32(match.Groups[0].Value.Replace("#US", ""));
                    foreach (TargetProcessItemStateHistory item in targetProcessItemStateHistoryList)
                    {
                        if (item.UserStory.Id == commitTP.CommitUSId && item.Date < commitTP.Commit.Committer.Date)
                            commitTP.UserStoryHistory = item;
                    }
                    continue;
                }

                pattern = new Regex(US_REGEX_POUND_US_5_DIGITS);
                match = pattern.Match(commitTP.Commit.Comment);
                if (!string.IsNullOrEmpty(match.Groups[0].Value))
                {
                    commitTP.CommitUSId = Convert.ToInt32(match.Groups[0].Value.Replace("#US", ""));
                    foreach (TargetProcessItemStateHistory item in targetProcessItemStateHistoryList)
                    {
                        if (item.UserStory.Id == commitTP.CommitUSId && item.Date < commitTP.Commit.Committer.Date)
                            commitTP.UserStoryHistory = item;
                    }
                    continue;
                }

                pattern = new Regex(US_REGEX_POUND_SPACE_US_TAB_6_DIGITS);
                match = pattern.Match(commitTP.Commit.Comment);
                if (!string.IsNullOrEmpty(match.Groups[0].Value))
                {
                    commitTP.CommitUSId = Convert.ToInt32(match.Groups[0].Value.Replace("#US", ""));
                    foreach (TargetProcessItemStateHistory item in targetProcessItemStateHistoryList)
                    {
                        if (item.UserStory.Id == commitTP.CommitUSId && item.Date < commitTP.Commit.Committer.Date)
                            commitTP.UserStoryHistory = item;
                    }
                    continue;
                }

                pattern = new Regex(US_REGEX_POUND_SPACE_US_TAB_5_DIGITS);
                match = pattern.Match(commitTP.Commit.Comment);
                if (!string.IsNullOrEmpty(match.Groups[0].Value))
                {
                    commitTP.CommitUSId = Convert.ToInt32(match.Groups[0].Value.Replace("#US", ""));
                    foreach (TargetProcessItemStateHistory item in targetProcessItemStateHistoryList)
                    {
                        if (item.UserStory.Id == commitTP.CommitUSId && item.Date < commitTP.Commit.Committer.Date)
                            commitTP.UserStoryHistory = item;
                    }
                    continue;
                }

                pattern = new Regex(US_REGEX_POUND_SPACE_US_SINGLESPACE_6_DIGITS);
                match = pattern.Match(commitTP.Commit.Comment);
                if (!string.IsNullOrEmpty(match.Groups[0].Value))
                {
                    commitTP.CommitUSId = Convert.ToInt32(match.Groups[0].Value.Replace("#US ", ""));
                    foreach (TargetProcessItemStateHistory item in targetProcessItemStateHistoryList)
                    {
                        if (item.UserStory.Id == commitTP.CommitUSId && item.Date < commitTP.Commit.Committer.Date)
                            commitTP.UserStoryHistory = item;
                    }
                    continue;
                }

                pattern = new Regex(US_REGEX_POUND_SPACE_US_SINGLESPACE_5_DIGITS);
                match = pattern.Match(commitTP.Commit.Comment);
                if (!string.IsNullOrEmpty(match.Groups[0].Value))
                {
                    commitTP.CommitUSId = Convert.ToInt32(match.Groups[0].Value.Replace("#US ", ""));
                    foreach (TargetProcessItemStateHistory item in targetProcessItemStateHistoryList)
                    {
                        if (item.UserStory.Id == commitTP.CommitUSId && item.Date < commitTP.Commit.Committer.Date)
                            commitTP.UserStoryHistory = item;
                    }
                    continue;
                }

                pattern = new Regex(US_REGEX_POUND_SPACE_US_DOUBLESPACE_6_DIGITS);
                match = pattern.Match(commitTP.Commit.Comment);
                if (!string.IsNullOrEmpty(match.Groups[0].Value))
                {
                    commitTP.CommitUSId = Convert.ToInt32(match.Groups[0].Value.Replace("# US ", ""));
                    foreach (TargetProcessItemStateHistory item in targetProcessItemStateHistoryList)
                    {
                        if (item.UserStory.Id == commitTP.CommitUSId && item.Date < commitTP.Commit.Committer.Date)
                            commitTP.UserStoryHistory = item;
                    }
                    continue;
                }

                pattern = new Regex(US_REGEX_POUND_SPACE_US_DOUBLESPACE_5_DIGITS);
                match = pattern.Match(commitTP.Commit.Comment);
                if (!string.IsNullOrEmpty(match.Groups[0].Value))
                {
                    commitTP.CommitUSId = Convert.ToInt32(match.Groups[0].Value.Replace("# US ", ""));
                    foreach (TargetProcessItemStateHistory item in targetProcessItemStateHistoryList)
                    {
                        if (item.UserStory.Id == commitTP.CommitUSId && item.Date < commitTP.Commit.Committer.Date)
                            commitTP.UserStoryHistory = item;
                    }
                    continue;
                }

                pattern = new Regex(US_REGEX_POUND_BUG_6_DIGITS);
                match = pattern.Match(commitTP.Commit.Comment);
                if (!string.IsNullOrEmpty(match.Groups[0].Value))
                {
                    commitTP.CommitUSId = Convert.ToInt32(match.Groups[0].Value.Replace("#Bug", ""));
                    foreach (TargetProcessItemStateHistory item in targetProcessItemStateHistoryList)
                    {
                        if (item.UserStory.Id == commitTP.CommitUSId && item.Date < commitTP.Commit.Committer.Date)
                            commitTP.UserStoryHistory = item;
                    }
                    continue;
                }

                pattern = new Regex(US_REGEX_POUND_BUG_5_DIGITS);
                match = pattern.Match(commitTP.Commit.Comment);
                if (!string.IsNullOrEmpty(match.Groups[0].Value))
                {
                    commitTP.CommitUSId = Convert.ToInt32(match.Groups[0].Value.Replace("#Bug", ""));
                    foreach (TargetProcessItemStateHistory item in targetProcessItemStateHistoryList)
                    {
                        if (item.UserStory.Id == commitTP.CommitUSId && item.Date < commitTP.Commit.Committer.Date)
                            commitTP.UserStoryHistory = item;
                    }
                    continue;
                }

                pattern = new Regex(US_REGEX_POUND_B_6_DIGITS);
                match = pattern.Match(commitTP.Commit.Comment);
                if (!string.IsNullOrEmpty(match.Groups[0].Value))
                {
                    commitTP.CommitUSId = Convert.ToInt32(match.Groups[0].Value.Replace("#B", ""));
                    foreach (TargetProcessItemStateHistory item in targetProcessItemStateHistoryList)
                    {
                        if (item.UserStory.Id == commitTP.CommitUSId && item.Date < commitTP.Commit.Committer.Date)
                            commitTP.UserStoryHistory = item;
                    }
                    continue;
                }

                pattern = new Regex(US_REGEX_POUND_B_5_DIGITS);
                match = pattern.Match(commitTP.Commit.Comment);
                if (!string.IsNullOrEmpty(match.Groups[0].Value))
                {
                    commitTP.CommitUSId = Convert.ToInt32(match.Groups[0].Value.Replace("#B", ""));
                    foreach (TargetProcessItemStateHistory item in targetProcessItemStateHistoryList)
                    {
                        if (item.UserStory.Id == commitTP.CommitUSId && item.Date < commitTP.Commit.Committer.Date)
                            commitTP.UserStoryHistory = item;
                    }
                    continue;
                }

                pattern = new Regex(US_REGEX_POUND_SPACE_BUG_SINGLESPACE_6_DIGITS);
                match = pattern.Match(commitTP.Commit.Comment);
                if (!string.IsNullOrEmpty(match.Groups[0].Value))
                {
                    commitTP.CommitUSId = Convert.ToInt32(match.Groups[0].Value.Replace("#Bug ", ""));
                    foreach (TargetProcessItemStateHistory item in targetProcessItemStateHistoryList)
                    {
                        if (item.UserStory.Id == commitTP.CommitUSId && item.Date < commitTP.Commit.Committer.Date)
                            commitTP.UserStoryHistory = item;
                    }
                    continue;
                }

                pattern = new Regex(US_REGEX_POUND_SPACE_BUG_SINGLESPACE_5_DIGITS);
                match = pattern.Match(commitTP.Commit.Comment);
                if (!string.IsNullOrEmpty(match.Groups[0].Value))
                {
                    commitTP.CommitUSId = Convert.ToInt32(match.Groups[0].Value.Replace("#Bug ", ""));
                    foreach (TargetProcessItemStateHistory item in targetProcessItemStateHistoryList)
                    {
                        if (item.UserStory.Id == commitTP.CommitUSId && item.Date < commitTP.Commit.Committer.Date)
                            commitTP.UserStoryHistory = item;
                    }
                    continue;
                }
            }
        }

        private static ProcessResponse targetProcessDeveloperListPerTeam(string teamName)
        {
            TPApiItemSelector tpClient = new TPApiItemSelector(_patTP);
            tpClient.GetDevelopersByTeam($"{teamName}", "TeamMembers");
            if (tpClient.Response.IsSuccess)
            {
                _targetProcessDeveloperListPerTeam = CommonUtilities.DeserializeJsontoObject<TargetProcessTeamMemberList>((string)tpClient.Response.ReturnedResponseObject);
            }
            else
            { return tpClient.Response; }
            
            return null;
        }

        private static ProcessResponse targetProcessEntitiesByUserStoryName(string userStoryName)
        {
            TPApiItemSelector tpClient = new TPApiItemSelector(_patTP);
            tpClient.GetEntitiesbyUserStoryName($"{userStoryName}", "UserStories");
            if (tpClient.Response.IsSuccess)
            {
                _targetProcessUserStoryByNameItemList = CommonUtilities.DeserializeJsontoObject<TargetProcessItemList>((string)tpClient.Response.ReturnedResponseObject);
            }
            else
            { return tpClient.Response; }          
            return null;
        }

        private static ProcessResponse targetProcessEntitiesByEpicName(string epicName)
        {
            TPApiItemSelector tpClient = new TPApiItemSelector(_patTP);
            tpClient.GetEntitiesbyEpicName($"{epicName}","UserStories");
            if (tpClient.Response.IsSuccess)
            {
                _targetProcessEpicItemList = CommonUtilities.DeserializeJsontoObject<TargetProcessItemList>((string)tpClient.Response.ReturnedResponseObject);
            }
            else
            { return tpClient.Response; }
            tpClient.GetEntitiesbyEpicName($"{epicName}","Bugs");
            if (tpClient.Response.IsSuccess)
            {
                _targetProcessEpicItemList.Items.AddRange(CommonUtilities.DeserializeJsontoObject<TargetProcessItemList>((string)tpClient.Response.ReturnedResponseObject).Items);
            }
            else
            { return tpClient.Response; }
            return null;
        }

        private static ProcessResponse targetProcessEntitiesByFeatureId(int featureId)
        {
            TPApiItemSelector tpClient = new TPApiItemSelector(_patTP);
            tpClient.GetEntitiesbyFeatureId(featureId, "UserStories");
            if (tpClient.Response.IsSuccess)
            {
                _targetProcessFeatureItemList = CommonUtilities.DeserializeJsontoObject<TargetProcessItemList>((string)tpClient.Response.ReturnedResponseObject);
            }
            else
            { return tpClient.Response; }
            tpClient.GetEntitiesbyFeatureId(featureId, "Bugs");
            if (tpClient.Response.IsSuccess)
            {
                _targetProcessFeatureItemList.Items.AddRange(CommonUtilities.DeserializeJsontoObject<TargetProcessItemList>((string)tpClient.Response.ReturnedResponseObject).Items);
            }
            else
            { return tpClient.Response; }
            return null;
        }

        private static ProcessResponse targetProcessSprintEntities()
        {
            TPApiItemSelector tpClient = new TPApiItemSelector(_patTP);
            tpClient.GetEntitiesbyTeamSprint(string.Join(",", _teamSprintNamesForAPI),"UserStories");
            if (tpClient.Response.IsSuccess)
            {
                _targetProcessItemList = CommonUtilities.DeserializeJsontoObject<TargetProcessItemList>((string)tpClient.Response.ReturnedResponseObject);
            }
            else
                { return tpClient.Response; }
            tpClient.GetEntitiesbyTeamSprint(string.Join(",", _teamSprintNamesForAPI),"Bugs");
            if (tpClient.Response.IsSuccess)
            {
                _targetProcessItemList.Items.AddRange(CommonUtilities.DeserializeJsontoObject<TargetProcessItemList>((string)tpClient.Response.ReturnedResponseObject).Items);
            }
            else
                { return tpClient.Response; }
            return null;
        }

        private static ProcessResponse targetProcessCommentEntriesPerUS(int userStoryId)
        {
            TPApiItemSelector tpClient = new TPApiItemSelector(_patTP);
            tpClient.GetIndividualTPEntityComments(userStoryId);
            if (tpClient.Response.IsSuccess)
            {
                _targetProcessCommentListForUS = CommonUtilities.DeserializeJsontoObject<TargetProcessCommentListForUS>((string)tpClient.Response.ReturnedResponseObject);
            }
            else
            { return tpClient.Response; }
            return null;
        }

        private static ProcessResponse targetProcessTaskEntities(int userStoryId)
        {
            TPApiItemSelector tpClient = new TPApiItemSelector(_patTP);
            tpClient.GetIndividualTPEntityTasks(userStoryId);
            if (tpClient.Response.IsSuccess)
            {
                _targetProcessItemTaskList = CommonUtilities.DeserializeJsontoObject<TargetProcessItemTaskList>((string)tpClient.Response.ReturnedResponseObject);
            }
            else
            { return tpClient.Response; }           
            return null;
        }

        private static ProcessResponse targetProcessAssignmentsforEntity(int userStoryId)
        {
            TPApiItemSelector tpClient = new TPApiItemSelector(_patTP);
            tpClient.GetIndividualTPAssignments(userStoryId);
            if (tpClient.Response.IsSuccess)
            {
                _targetProcessAssignmentsTaskList = CommonUtilities.DeserializeJsontoObject<TargetProcessItemAssignmentList>((string)tpClient.Response.ReturnedResponseObject);
            }
            else
            { return tpClient.Response; }
            return null;
        }

        private static ProcessResponse targetProcessSprintEntities(string teamName)
        {
            TPApiItemSelector tpClient = new TPApiItemSelector(_patTP);
            tpClient.GetEntitiesbyTeamSprint($"'{teamName}'","UserStories");
            if (tpClient.Response.IsSuccess)
            {
                _targetProcessItemList = CommonUtilities.DeserializeJsontoObject<TargetProcessItemList>((string)tpClient.Response.ReturnedResponseObject);
            }
            else
            { return tpClient.Response; }
            tpClient.GetEntitiesbyTeamSprint($"'{teamName}'","Bugs");
            if (tpClient.Response.IsSuccess)
            {
                _targetProcessItemList.Items.AddRange(CommonUtilities.DeserializeJsontoObject<TargetProcessItemList>((string)tpClient.Response.ReturnedResponseObject).Items);
            }
            else
            { return tpClient.Response; }
            return null;
        }

        private static ProcessResponse targetProcessSprintEntitiesFullHistory()
        {
            TPApiItemSelector tpClient = new TPApiItemSelector(_patTP);
            
            tpClient.GetEntitiesFullHistorybyTeamSprint(string.Join(",", _teamSprintNamesForAPI), EnumEntityType.UserStorySingle);
            if (tpClient.Response.IsSuccess)
            {
                _targetProcessItemStateHistoryList = 
                    CommonUtilities.DeserializeJsontoObject<TargetProcessItemStateHistoryList>((string)tpClient.Response.ReturnedResponseObject);
            }
            else
                { return tpClient.Response; }

            tpClient.GetEntitiesFullHistorybyTeamSprint(string.Join(",", _teamSprintNamesForAPI), EnumEntityType.BugSingle);
            if (tpClient.Response.IsSuccess)
            {
                _targetProcessItemStateHistoryList.Items.AddRange(
                    CommonUtilities.DeserializeJsontoObject<TargetProcessItemStateHistoryList>((string)tpClient.Response.ReturnedResponseObject).Items);
            }
            else
                { return tpClient.Response; }
            return null;
        }

        private static ProcessResponse targetProcessSprintEntitiesFullHistoryWithoutBugs()
        {
            TPApiItemSelector tpClient = new TPApiItemSelector(_patTP);

            tpClient.GetEntitiesFullHistorybyTeamSprint(string.Join(",", _teamSprintNamesForAPI), EnumEntityType.UserStorySingle);
            if (tpClient.Response.IsSuccess)
            {
                _targetProcessItemStateHistoryList =
                    CommonUtilities.DeserializeJsontoObject<TargetProcessItemStateHistoryList>((string)tpClient.Response.ReturnedResponseObject);
            }
            else
            { return tpClient.Response; }
           
            return null;
        }


        private static ProcessResponse targetProcessSprintEntitiesFullHistory(string teamName)
        {
            TPApiItemSelector tpClient = new TPApiItemSelector(_patTP);

            tpClient.GetEntitiesFullHistorybyTeamSprint($"'{teamName}'", EnumEntityType.UserStorySingle);
            if (tpClient.Response.IsSuccess)
            {
                _targetProcessItemStateHistoryList =
                    CommonUtilities.DeserializeJsontoObject<TargetProcessItemStateHistoryList>((string)tpClient.Response.ReturnedResponseObject);
            }
            else
            { return tpClient.Response; }

            tpClient.GetEntitiesFullHistorybyTeamSprint($"'{teamName}'", EnumEntityType.BugSingle);
            if (tpClient.Response.IsSuccess)
            {
                _targetProcessItemStateHistoryList.Items.AddRange(
                    CommonUtilities.DeserializeJsontoObject<TargetProcessItemStateHistoryList>((string)tpClient.Response.ReturnedResponseObject).Items);
            }
            else
            { return tpClient.Response; }
            return null;
        }

        private static ProcessResponse targetProcessGetEntityDetails(int entityId)
        {
            TPApiItemSelector tpClient = new TPApiItemSelector(_patTP);
            tpClient.   GetIndividualTPEntity(entityId, "UserStory");
            if (!tpClient.Response.IsSuccess || tpClient.Response.IsSuccess && !string.IsNullOrEmpty((string)tpClient.Response.ReturnedResponseObject))
                { return tpClient.Response; }
            tpClient.GetIndividualTPEntity(entityId, "Bug");
                if (!tpClient.Response.IsSuccess || tpClient.Response.IsSuccess && !string.IsNullOrEmpty((string)tpClient.Response.ReturnedResponseObject))
                    { return tpClient.Response; }
            return null;
        }

        private static ProcessResponse targetProcessGetEntityDetails(List<int> USids, EnumEntityType entityType)
        {
            string ids = String.Join(",", USids);
            TPApiItemSelector tpClient = new TPApiItemSelector(_patTP);
            tpClient.GetEntitiesbyListofIds(ids, entityType);
            return tpClient.Response;
        }

        private static void initializeSettings()
        {
            IConfiguration configuration = new ConfigurationBuilder()   // Building JSON configuration file from appsettings.json file
            .AddJsonFile("appsettings.json")
            .Build();
            try
            {
                _year = configuration["year"];
                _organization = configuration["organization"];
                _project = configuration["project"];
                _sprint = configuration["sprint"];
                _previousSprint = configuration["previousSprint"];
                _repoName = configuration["repoName"];
                _signOffTagFrom = configuration["signOffTagFrom"];
                _signOffTagTo = configuration["signOffTagTo"];
                _patTeamCity = configuration["teamCityPAT"];
                _patTP = configuration["targetProcessPAT"];
                _patAzureDevops = configuration["azureDevOpsPAT"];
                
                var teamNameList = configuration.GetSection("teamNames").Get<string[]>();                
                var teamSprintList = configuration.GetSection("teamSprintNames").Get<string[]>();
                var teamSprintListForAPI = configuration.GetSection("teamSprintNamesForAPI").Get<string[]>();
                _teamNames = teamNameList;
                _teamSprintNames = new string[teamSprintList.Count()];
                _teamSprintNamesForAPI = new string[teamSprintListForAPI.Count()];

                int i = 0;
                foreach(string sprintName in teamSprintList)
                {
                  _teamSprintNames[i] = string.Format(sprintName, _sprint);
                  i++;
                }

                int j = 0;
                foreach (string sprintName in teamSprintListForAPI)
                {
                    _teamSprintNamesForAPI[j] = string.Format(sprintName, _sprint);
                    j++;
                }
            }
            catch (Exception ex)
            {
                Console.Write(ex);
            }        
        }        
    }
}
