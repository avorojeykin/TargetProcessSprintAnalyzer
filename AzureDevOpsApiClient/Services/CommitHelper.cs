using Common.Models;
using Common.Utilities;
using Microsoft.TeamFoundation.SourceControl.WebApi;
using Microsoft.VisualStudio.Services.WebApi;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RestSharp;
using System.Net;

namespace AzureDevOpsApiClient.Services
{
    public class CommitHelper
    {
        #region Properties
        public ProcessResponse Response { get; set; }
        #endregion

        #region Fields
        private readonly string _pat;
        private readonly string _organization;
        private readonly string _project;
        private readonly string _repoName;

        #endregion

        #region Constants
        private const string START = "********** Start {0} **********\r\n";
        private const string THE_END = "**********The End**********\r\n";
        private const string URL_BASE = "https://dev.azure.com/{0}";       
        private const string TAG_PUSH_DATE_FORMAT = "MM/dd/yyyy hh:mm:ss tt";
        private const int TOP = 10000;
        private const string BRANCH = "Development";
        private const string ERROR_GET_COMMITS_BY_TAGS = "Error getting commits by tag info. Details: {0}";
        private const string ERROR_GET_COMMITS_BY_VERSION_TAGS = "Error getting commits by version and tag info. Details: {0}";
        private const string ERROR_COMMIT_TAG_WAS_NOT_FOUND = "Commit for the Tag: {0} was not found.";
        #endregion

        #region Constructors
        public CommitHelper(string pat, string organization, string project, string repoName)
        {
            _pat = pat;
            _organization = organization;
            _project = project;
            _repoName = repoName;
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Select commits using push date from a tag spacified and 
        /// </summary>
        /// <param name="tagSelectFrom"></param>
        /// <param name="tagSelectTo"></param>
        public void GetCommits(string tagSelectFrom, string tagSelectTo = null)
        {
            InitializeResponse();
            Response.Log = string.Format(START, "Start selecting commits by tag push date(s)");
            try
            {
                GetCommitsbetweenTags(tagSelectFrom, tagSelectTo);
            }
            catch (Exception ex)
            {
                Response.IsSuccess = false;
                Response.SystemError = CommonUtilities.GetExceptionString(ref ex);
            }
            finally
            {
                Response.Log = THE_END;
            }
        }

        public void GetCommits(string tagSelectFrom, GitVersionType versionType, string version, string tagSelectTo = null)
        {
            InitializeResponse();
            Response.Log = string.Format(START, "Start selecting commits by tag push date(s) and verion details");
            try
            {
                GetCommitsbyVersionandbetweenTags(tagSelectFrom, tagSelectTo, versionType, version);
            }
            catch (Exception ex)
            {
                Response.IsSuccess = false;
                Response.SystemError = CommonUtilities.GetExceptionString(ref ex);
            }
            finally
            {
                Response.Log = THE_END;
            }
        }
        #endregion

        #region Private Methodsc
        private void InitializeResponse()
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

        private void GetCommitsbetweenTags(string tagSelectFrom, string tagSelectTo)
        {
            List<GitCommitRef> result = new List<GitCommitRef>();
            string pushDateFrom = string.Empty;
            string pushDateTo = string.Empty;
            try
            {
                VssBasicCredential credentials = new VssBasicCredential(string.Empty, _pat);
                VssConnection connection = new VssConnection(new Uri(string.Format(URL_BASE, _organization)), credentials);
                Response.Log = "Open connection to Azure Git";
                using (GitHttpClient gitClient = connection.GetClient<GitHttpClient>())
                {
                    Response.Log = $"Get repo \"{_repoName}\"";
                    GitRepository repo = gitClient.GetRepositoryAsync(_project, _repoName).Result;
                    Response.Log = $"Find push date of the commit associated with Tag: \"{tagSelectFrom}\"";
                    pushDateFrom = GetTaggedCommitPushDate(gitClient, repo.Id, tagSelectFrom);
                    Response.Log = $"Push date to use as date from: \"{pushDateFrom}\"";
                    if (!string.IsNullOrEmpty(tagSelectTo))
                        { pushDateTo = GetTaggedCommitPushDate(gitClient, repo.Id, tagSelectTo); }
                    Response.Log = $"Push date (optional) to use as date to: \"{pushDateFrom}\"";
                    Response.Log = "Create query";
                    GitQueryCommitsCriteria commitQuery = CreateQuery(pushDateFrom, pushDateTo);
                    Response.Log = "Run query to get commits";
                    result = gitClient.GetCommitsAsync(repo.Id, searchCriteria: commitQuery, top: TOP).Result;
                }
            }
            catch (Exception ex)
            {
                Response.Log = CommonUtilities.GetExceptionString(ref ex);
                throw new Exception(string.Format(ERROR_GET_COMMITS_BY_TAGS, CommonUtilities.GetExceptionString(ref ex)));
            }
            finally
            {
                Response.ReturnedResponseObject = result;
            }
        }

        private void GetCommitsbyVersionandbetweenTags(string tagSelectFrom, string tagSelectTo, GitVersionType versionType, string version)
        {
            List<GitCommitRef> result = new List<GitCommitRef>();
            string pushDateFrom = string.Empty;
            string pushDateTo = string.Empty;
            try
            {
                VssBasicCredential credentials = new VssBasicCredential(string.Empty, _pat);
                VssConnection connection = new VssConnection(new Uri(string.Format(URL_BASE, _organization)), credentials);
                Response.Log = "Open connection to Azure Git";
                using (GitHttpClient gitClient = connection.GetClient<GitHttpClient>())
                {
                    Response.Log = $"Get repo \"{_repoName}\"";
                    GitRepository repo = gitClient.GetRepositoryAsync(_project, _repoName).Result;
                    Response.Log = $"Find push date of the commit associated with Tag: \"{tagSelectFrom}\"";
                    pushDateFrom = GetTaggedCommitPushDate(gitClient, repo.Id, tagSelectFrom);
                    Response.Log = $"Push date to use as date from: \"{pushDateFrom}\"";
                    if (!string.IsNullOrEmpty(tagSelectTo))
                    { pushDateTo = GetTaggedCommitPushDate(gitClient, repo.Id, tagSelectTo); }
                    Response.Log = $"Push date (optional) to use as date to: \"{pushDateFrom}\"";
                    Response.Log = "Create query";
                    GitQueryCommitsCriteria commitQuery = CreateQuery(pushDateFrom, pushDateTo, versionType, version);
                    Response.Log = "Run query to get commits";
                    result = gitClient.GetCommitsAsync(repo.Id, searchCriteria: commitQuery, top: TOP).Result;
                }
            }
            catch (Exception ex)
            {
                Response.Log = CommonUtilities.GetExceptionString(ref ex);
                throw new Exception(string.Format(ERROR_GET_COMMITS_BY_VERSION_TAGS, CommonUtilities.GetExceptionString(ref ex)));
            }
            finally
            {
                Response.ReturnedResponseObject = result;
            }
        }

        private string GetTaggedCommitPushDate(GitHttpClient gitClient, Guid repoId, string tagName)
        {
            var refTags = gitClient.GetRefsAsync(repoId, filterContains: tagName, peelTags: true).Result;
            string commitId = refTags[0].PeeledObjectId ?? refTags[0].ObjectId;
            GitCommit commit = gitClient.GetCommitAsync(commitId, repoId).Result;
            if (commit == null)
                { throw new Exception(string.Format(ERROR_COMMIT_TAG_WAS_NOT_FOUND, tagName)); }
            return commit.Push.Date.ToString(TAG_PUSH_DATE_FORMAT);
        }

        private GitQueryCommitsCriteria CreateQuery(string fromDate, string toDate)
        {
            GitQueryCommitsCriteria commitQuery = null;
            if (string.IsNullOrEmpty(toDate))
                { commitQuery = new GitQueryCommitsCriteria { FromDate = fromDate }; }
            else
                { commitQuery = new GitQueryCommitsCriteria { FromDate = fromDate, ToDate = toDate }; }
            return commitQuery;
        }

        private GitQueryCommitsCriteria CreateQuery(string fromDate, string toDate, GitVersionType versionType, string version)
        {
            GitQueryCommitsCriteria commitQuery = null;
            if (string.IsNullOrEmpty(toDate))
            { 
                commitQuery = new GitQueryCommitsCriteria 
                {
                    ItemVersion = new GitVersionDescriptor { Version = version, VersionType = versionType },
                    FromDate = fromDate 
                }; 
            }
            else
            { 
                commitQuery = new GitQueryCommitsCriteria 
                {
                    ItemVersion = new GitVersionDescriptor { Version = version, VersionType = versionType },
                    FromDate = fromDate, 
                    ToDate = toDate 
                }; 
            }
            return commitQuery;
        }
        #endregion             
    }
}
