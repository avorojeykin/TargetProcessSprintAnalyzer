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

        public void uploadFileToDrive(string fileName)
        {
            initializeResponse();
            try
            {
                _url = $"{URL_BASE}:/Release Coordinator Scenarios/{fileName}:/content";
                Response.ReturnedResponseObject = callApi(accessToken,fileName);
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

        private string callApi(string myToken,string fileName)
        {            
            string result = string.Empty;
            using (RestClient client = new RestClient(_url))
            {
                FileInfo file = new FileInfo($"C:\\Users\\artur.vorojeykin\\Documents\\{fileName}");
            //    FileStream fs = File.Open(file.FullName, FileMode.Open);
            //    byte[] data = new byte[fs.Length];
            //    fs.Read(data, 0, (int)fs.Length);
                var request = new RestRequest(_url, Method.Put);
                request.AlwaysMultipartFormData = true;
                request.RequestFormat = DataFormat.Binary;
                request.AddHeader("Authorization", $"Bearer {myToken}");
                request.AddHeader("Content-Type", "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
                request.AddHeader("Content-Length", $"{file.Length}");
                request.AddFile(fileName, $"C:\\Users\\artur.vorojeykin\\Documents\\{fileName}", "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
            //    request.AddFile("test.xlsx", data,"test.xlsx", "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
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
