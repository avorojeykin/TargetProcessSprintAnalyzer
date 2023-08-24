using Aspose.Cells;
using Common.Models;
using Common.Utilities;
using RestSharp;
using RestSharp.Authenticators;
using System;
using System.IO;
using System.Net;

namespace OneDriveAPIClient
{
    public class OneDriveUploadItems
    {
        #region Properties
        public ProcessResponse Response { get; set; }
        #endregion

        #region Fields
        private string _url;
        private string accessToken;
        #endregion

        #region Constants
        private const string START = "********** Start {0} **********\r\n";
        private const string THE_END = "**********The End**********\r\n";
        private const string URL_BASE = "https://graph.microsoft.com/v1.0/me/drive/root";
        private const string ERROR_RECEIVE_NOT_OK_HTTP_STATUS = "Received not OK status after http call. Response content: {0}\r\nResponse error: {1}";
        #endregion

        public OneDriveUploadItems(string accessToken) {
            this.accessToken = accessToken;
        }

        #region Public Methods

        public void UploadFileToDrive(string fileName)
        {
            InitializeResponse();
            try
            {
                _url = $"{URL_BASE}:/{fileName}:/content";
                Response.ReturnedResponseObject = CallApi(accessToken,fileName);
            }
            catch (Exception ex)
            {
                Response.IsSuccess = false;
                Response.SystemError = CommonUtilities.GetExceptionString(ref ex);
            }            
        }       

        #endregion

        #region Private Methods
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

        private string CallApi(string myToken,string fileName)
        {            
            string result = string.Empty;
            using (RestClient client = new RestClient(_url))
            {
                FileInfo file = new FileInfo($"C:\\Users\\Username\\Documents\\{fileName}"); //Add File Path
                var request = new RestRequest(_url, Method.Put);
                request.AlwaysMultipartFormData = true;
                request.RequestFormat = DataFormat.Binary;
                request.AddHeader("Authorization", $"Bearer {myToken}");
                request.AddHeader("Content-Type", "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
                request.AddHeader("Content-Length", $"{file.Length}");
                request.AddFile(fileName, $"C:\\Users\\Username\\Documents\\{fileName}", "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"); //Add File Path
                request.Timeout = (300 * 1000);
                RestResponse response = client.Execute(request);
                if (response.StatusCode == HttpStatusCode.Created)
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
