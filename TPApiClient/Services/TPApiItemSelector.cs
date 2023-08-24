using Common.Models;
using Common.Utilities;
using Common.Enumerators;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using Microsoft.Extensions.Logging;

namespace TPApiClient.Services
{
    public class TPApiItemSelector
    {
        #region Properties
        public ProcessResponse Response { get; set; }
        #endregion

        #region Fields        
        private readonly string _pat;
        private string _url;

        #endregion

        #region Constants
        private const string START = "********** Start {0} **********\r\n";
        private const string THE_END = "**********The End**********\r\n";
        private const string URL_BASE_V1 = "https://placeholder.tpondemand.com/api/v1/";
        private const string URL_BASE = "https://placeholder.tpondemand.com/api/v2/";
        private const string URL_FULL_HISTORY = "https://placeholder.tpondemand.com/api/history/v2/";
        private const string SELECT_STMT_INDIVIDUAL_ENTITY = "{id,resourcetype,teamiteration:{teamiteration.name},entitystate:{entitystate.id,entitystate.name}}";
        private const string SELECT_STMT_INDIVIDUAL_ENTITY_TASKS = "{Id,Name,Effort,EntityState,AssignedUser}";
        private const string WHERE_CLAUSE_TEAM_NAME = "(Team.Name='{0}')";
        private const string WHERE_CLAUSE_TEAM_SPRINT = "(TeamIteration.Name in [{0}])";
        private const string WHERE_CLAUSE_LIST_OF_IDS = "(id in [{0}])";
        private const string SELECT_STMT_TEAM_SPRINT = "{id,resourcetype,teamiteration:{teamiteration.name},entitystate:{entitystate.id,entitystate.name}}";
        private const string SELECT_STMT_TEAM_NAME = "{user:{user.FirstName,user.LastName},role:{role.id,role.name},team:{team.name}}";
        private const string SELECT_STMT_TEAM_SPRINT_USER_STORY_STATE_HISTORY = "{date,entitystate:{entitystate.id, entitystate.name},userstory:{currentuserstory.id,currentuserstory.name},teamiteration:{teamiteration.id,teamiteration.name},resourcetype}";
        private const string SELECT_STMT_TEAM_SPRINT_BUG_STATE_HISTORY = "{date,entitystate:{entitystate.id, entitystate.name},userstory:{currentbug.id,currentbug.name},teamiteration:{teamiteration.id,teamiteration.name},resourcetype}";
        private const string SELECT_STMT_COMMENTS = "{Comment:UserStory.Comments.Select({Owner,Description})}";
        private const string ERROR_RECEIVE_NOT_OK_HTTP_STATUS = "Received not OK status after http call. Response content: {0}\r\nResponse error: {1}";
        private const int TAKE = 10000;
        #endregion

        #region Constructors
        public TPApiItemSelector(string pat)
        {
            _pat = pat;
        }
        #endregion

        #region Public Methods
        public void GetIndividualTPEntity(int entityId, string entityType)
        {
            initializeResponse();
            try
            {
                _url = $"{URL_BASE}{entityType}/{entityId}?access_token={_pat}&select={SELECT_STMT_INDIVIDUAL_ENTITY}";
                Response.ReturnedResponseObject = callApi();
                
            }
            catch (Exception ex)
            {
                Response.IsSuccess = false;
                Response.SystemError = CommonUtilities.GetExceptionString(ref ex);               
            }
        }

        public void GetIndividualTPEntityComments(int entityId)
        {
            initializeResponse();
            try
            {
                _url = $"{URL_BASE}Task?Select={SELECT_STMT_COMMENTS}&where=(UserStory.Id={entityId})&access_token={_pat}&take=1";
                Response.ReturnedResponseObject = callApi();
            }
            catch (Exception ex)
            {
                Response.IsSuccess = false;
                Response.SystemError = CommonUtilities.GetExceptionString(ref ex);
            }
        }

        public void GetIndividualTPEntityTasks(int entityId)
        {
            initializeResponse();
            try
            {
                _url = $"{URL_BASE}Tasks/?where=(Userstory.Id = {entityId})&access_token={_pat}&select={SELECT_STMT_INDIVIDUAL_ENTITY_TASKS}&take={TAKE}";
                Response.ReturnedResponseObject = callApi();
            }
            catch (Exception ex)
            {
                Response.IsSuccess = false;
                Response.SystemError = CommonUtilities.GetExceptionString(ref ex);
            }
        }

        public void GetIndividualTPAssignments(int entityId)
        {
            initializeResponse();
            try
            {
                _url = $"{URL_BASE_V1}UserStories/{entityId}?include=[Assignments[GeneralUser,Role]]&access_token={_pat}&format=json";
                Response.ReturnedResponseObject = callApi();
            }
            catch (Exception ex)
            {
                Response.IsSuccess = false;
                Response.SystemError = CommonUtilities.GetExceptionString(ref ex);
            }
        }

        public void GetEntitiesbyUserStoryName(string userStoryName, string entityType)
        {
            initializeResponse();
            try
            {
                _url = $"{URL_BASE}{entityType}?filter=?'{userStoryName}'in Name &select={SELECT_STMT_INDIVIDUAL_ENTITY}&take={TAKE}&access_token={_pat}";
                Response.ReturnedResponseObject = callApi();
            }
            catch (Exception ex)
            {
                Response.IsSuccess = false;
                Response.SystemError = CommonUtilities.GetExceptionString(ref ex);
            }
        }

        public void GetDevelopersByTeam(string teamName, string entityType)
        {
            initializeResponse();
            try
            {
                _url = $"{URL_BASE}{entityType}?where={string.Format(WHERE_CLAUSE_TEAM_NAME, teamName)}&select={SELECT_STMT_TEAM_NAME}&take={TAKE}&access_token={_pat}";
                Response.ReturnedResponseObject = callApi();
            }
            catch (Exception ex)
            {
                Response.IsSuccess = false;
                Response.SystemError = CommonUtilities.GetExceptionString(ref ex);
            }
        }

        public void GetEntitiesbyTeamSprint(string teamSprints, string entityType)
        {
            initializeResponse();
            try
            {
                _url = $"{URL_BASE}{entityType}?where={string.Format(WHERE_CLAUSE_TEAM_SPRINT, teamSprints)}&select={SELECT_STMT_TEAM_SPRINT}&take={TAKE}&access_token={_pat}";
                Response.ReturnedResponseObject = callApi();
            }
            catch (Exception ex)
            {
                Response.IsSuccess = false;
                Response.SystemError = CommonUtilities.GetExceptionString(ref ex);
            }
        }

        public void GetEntitiesbyFeatureId(int featureId, string entityType)
        {
            initializeResponse();
            try
            {
                _url = $"{URL_BASE}{entityType}?filter=?feature.id is {featureId}&select={SELECT_STMT_TEAM_SPRINT}&take={TAKE}&access_token={_pat}";
                Response.ReturnedResponseObject = callApi();
            }
            catch (Exception ex)
            {
                Response.IsSuccess = false;
                Response.SystemError = CommonUtilities.GetExceptionString(ref ex);
            }
        }

        public void GetEntitiesbyEpicName(string epicName, string entityType)
        {
            initializeResponse();
            try
            {
                _url = $"{URL_BASE}{entityType}?filter=?feature.epic.name is '{epicName}'&select={SELECT_STMT_TEAM_SPRINT}&take={TAKE}&access_token={_pat}";
                Response.ReturnedResponseObject = callApi();
            }
            catch (Exception ex)
            {
                Response.IsSuccess = false;
                Response.SystemError = CommonUtilities.GetExceptionString(ref ex);
            }
        }

        public void GetEntitiesbyListofIds(string ids, EnumEntityType entityType)
        {
            initializeResponse();
            try
            {
                switch (entityType)
                {
                    case EnumEntityType.UserStoryPlural:
                        _url = $"{URL_BASE}{CommonUtilities.GetEnumDescription(entityType)}?where={string.Format(WHERE_CLAUSE_LIST_OF_IDS, ids)}" +
                            $"&select={SELECT_STMT_TEAM_SPRINT}&take={TAKE}&access_token={_pat}";
                        break;
                    case EnumEntityType.BugPlural:
                        _url = $"{URL_BASE}{CommonUtilities.GetEnumDescription(entityType)}?where={string.Format(WHERE_CLAUSE_LIST_OF_IDS, ids)}" +
                            $"&select={SELECT_STMT_TEAM_SPRINT}&take={TAKE}&access_token={_pat}";
                        break;
                    case EnumEntityType.TaskPlural:
                        _url = $"{URL_BASE}{CommonUtilities.GetEnumDescription(entityType)}?where={string.Format(WHERE_CLAUSE_LIST_OF_IDS, ids)}" +
                            $"&select={SELECT_STMT_TEAM_SPRINT}&take={TAKE}&access_token={_pat}";
                        break;
                    case EnumEntityType.RequestPlural:
                        _url = $"{URL_BASE}{CommonUtilities.GetEnumDescription(entityType)}?where={string.Format(WHERE_CLAUSE_LIST_OF_IDS, ids)}" +
                            $"&select={SELECT_STMT_TEAM_SPRINT}&take={TAKE}&access_token={_pat}";
                        break;
                    default:
                        Console.Write("Error: select scenario!");
                        break;
                }

                Response.ReturnedResponseObject = callApi();
            }
            catch (Exception ex)
            {
                Response.IsSuccess = false;
                Response.SystemError = CommonUtilities.GetExceptionString(ref ex);
            }
        }

        public void GetEntitiesFullHistorybyTeamSprint(string teamSprints, EnumEntityType entityType)
        {
            initializeResponse();
            try
            {
                switch (entityType)
                {
                    case EnumEntityType.UserStorySingle:
                        _url = $"{URL_FULL_HISTORY}{CommonUtilities.GetEnumDescription(entityType)}?where={string.Format(WHERE_CLAUSE_TEAM_SPRINT, teamSprints)}" +
                            $"&select={SELECT_STMT_TEAM_SPRINT_USER_STORY_STATE_HISTORY}&take={TAKE}&access_token={_pat}&dateformat=iso&orderBy=currentuserstory.id,date";
                        break;
                    case EnumEntityType.BugSingle:
                        _url = $"{URL_FULL_HISTORY}{CommonUtilities.GetEnumDescription(entityType)}?where={string.Format(WHERE_CLAUSE_TEAM_SPRINT, teamSprints)}" +
                            $"&select={SELECT_STMT_TEAM_SPRINT_BUG_STATE_HISTORY}&take={TAKE}&access_token={_pat}&dateformat=iso&orderBy=currentbug.id,date";
                        break;
                    default:
                        Console.Write("Error: select scenario!");
                        break;
                }

                Response.ReturnedResponseObject = callApi();
            }
            catch (Exception ex)
            {
                Response.IsSuccess = false;
                Response.SystemError = CommonUtilities.GetExceptionString(ref ex);
            }
        }
        #endregion

        #region Private Methods
        private void initializeResponse()
        {
            Response = new ProcessResponse
            {
                IsSuccess = true,
                ErrorDescription = string.Empty,
                SystemError = string.Empty,
                WarningDescription = string.Empty,
                ReturnedResponseObject = null,
                ProcessDetails = string.Empty
            };
        }

        private string callApi()
        {
            string result = string.Empty;
            using (RestClient client = new RestClient(_url))
            {
                var request = new RestRequest(_url, Method.Get);
                request.Timeout = (300 * 1000);
                RestResponse response;
                response = client.ExecuteGetAsync(request).Result;
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    result = response.Content;
                }
                else
                {
                    string responseError = response.ErrorMessage ?? string.Empty;
                    throw new Exception(string.Format(ERROR_RECEIVE_NOT_OK_HTTP_STATUS, response.Content, responseError));
                }
            }
            return result;
        }
        #endregion
    }
}
