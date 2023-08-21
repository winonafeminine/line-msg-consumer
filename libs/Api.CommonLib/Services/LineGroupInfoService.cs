using Api.AuthLib.Settings;
using Api.MessageLib.Stores;
using Api.ReferenceLib.DTOs;
using Api.ReferenceLib.Exceptions;
using Api.ReferenceLib.Interfaces;
using Api.ReferenceLib.Stores;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace Api.CommonLib.Services
{
    public class LineGroupInfoService : ILineGroupInfo
    {
        private readonly ILogger<LineGroupInfoService> _logger;
        private readonly IOptions<AuthLineConfigSetting> _lineLoginSetting;
        private readonly IWebHostEnvironment _webHostEnv;
        public LineGroupInfoService(ILogger<LineGroupInfoService> logger, IOptions<AuthLineConfigSetting> lineLoginSetting, IWebHostEnvironment webHostEnv)
        {
            _logger = logger;
            _lineLoginSetting = lineLoginSetting;
            _webHostEnv = webHostEnv;
        }

        public async Task<StaticfileDto> GetContent(string messageId, string accessToken)
        {
            using (var _httpClient = new HttpClient())
            {
                string url = LineApiReference.GetContent(messageId);
                var request = new HttpRequestMessage(HttpMethod.Get, url);
                request.Headers.Add("Authorization", $"Bearer {accessToken}");

                var response = await _httpClient.SendAsync(request);
                response.EnsureSuccessStatusCode();
                string contentType = response.Content.Headers.ContentType!.ToString();
                string[] mediaTypes = contentType.Split("/");

                string rootPath = _webHostEnv.ContentRootPath;
                string fileName = $"{messageId}.{mediaTypes.Last()}";
                string fullPath = Path.Combine(rootPath, Staticfile.Path, fileName);
                using (var contentStream = await response.Content.ReadAsStreamAsync())
                using (var fileStream = File.Create(fullPath))
                {
                    await contentStream.CopyToAsync(fileStream);
                }
                return new StaticfileDto{
                    Key = fileName,
                    FilePath = fullPath,
                    ContentType = contentType
                };
            }
        }

        public async Task<GetGroupMemberProfileDto> GetGroupMemberProfile(string groupId, string userId, string channelAccessToken)
        {
            string fullUrl = LineApiReference.GetGroupMemberProfile(groupId, userId);
            // StringContent stringContent = new StringContent();
            using (var httpClient = new HttpClient())
            {
                httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {channelAccessToken}");
                HttpResponseMessage response = await httpClient.GetAsync(fullUrl);
                response.EnsureSuccessStatusCode();
                string strResponse = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogError("Failed getting member profile");
                    _logger.LogError(strResponse);
                    throw new ErrorResponseException(
                        StatusCodes.Status400BadRequest,
                        "Failed getting member profile",
                        new List<Error>()
                    );
                }

                return JsonConvert.DeserializeObject<GetGroupMemberProfileDto>(strResponse)!;
            }
        }

        public async Task<GetGroupSummaryDto> GetGroupSummary(string groupId, string channelAccessToken)
        {
            string fullUrl = LineApiReference.GetGroupSummary(groupId);
            // _logger.LogInformation(fullUrl);
            // StringContent stringContent = new StringContent();
            using (var httpClient = new HttpClient())
            {
                httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {channelAccessToken}");
                HttpResponseMessage response = await httpClient.GetAsync(fullUrl);
                response.EnsureSuccessStatusCode();
                string strResponse = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogError("Failed getting group summary");
                    _logger.LogError(strResponse);
                    throw new ErrorResponseException(
                        StatusCodes.Status400BadRequest,
                        "Failed getting group summary",
                        new List<Error>()
                    );
                }

                return JsonConvert.DeserializeObject<GetGroupSummaryDto>(strResponse)!;
            }
        }

        public async Task<LineLoginUserProfileResponseDto> GetLineLoginUserProfile(string accessToken)
        {
            using (var httpClient = new HttpClient())
            {
                string fullUrl = LineApiReference.GetLineLoginUserProfileUrl();
                _logger.LogInformation($"Making the request to: {fullUrl}");

                httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {accessToken}");
                HttpResponseMessage response = await httpClient.GetAsync(fullUrl);
                response.EnsureSuccessStatusCode();
                string strResponse = await response.Content.ReadAsStringAsync();
                string resMessage = "Failed getting Line Login user profile";

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogError(resMessage);
                    _logger.LogError(strResponse);
                    throw new ErrorResponseException(
                        StatusCodes.Status400BadRequest,
                        resMessage,
                        new List<Error>()
                    );
                }
                resMessage = "Successfully getting Line Login user profile";
                _logger.LogInformation(resMessage);
                return JsonConvert.DeserializeObject<LineLoginUserProfileResponseDto>(strResponse)!;
            }
        }

        public async Task<LineLoginIssueTokenResponseDto> IssueLineLoginAccessToken(string code)
        {
            string fullUrl = LineApiReference.GetLineLoginIssueTokenUrl();
            _logger.LogInformation($"Making the request to: {fullUrl}");
            // StringContent stringContent = new StringContent();
            using (var httpClient = new HttpClient())
            {
                Dictionary<string, string> formData = new Dictionary<string, string>
                {
                    { "grant_type", "authorization_code" },
                    { "code", code },
                    { "redirect_uri",  _lineLoginSetting.Value.RedirectUri! },
                    { "client_id",  _lineLoginSetting.Value.ClientId! },
                    { "client_secret",  _lineLoginSetting.Value.SecretId! },
                    // Add more key-value pairs as needed
                };

                // Convert your key-value pair collection to a URL-encoded string
                HttpContent content = new FormUrlEncodedContent(formData);
                content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/x-www-form-urlencoded");

                HttpResponseMessage response = await httpClient.PostAsync(fullUrl, content);
                // response.EnsureSuccessStatusCode();
                string strResponse = await response.Content.ReadAsStringAsync();
                string message = "Failed issueing Line login access token";

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogError(message);
                    _logger.LogError(strResponse);
                    throw new ErrorResponseException(
                        StatusCodes.Status400BadRequest,
                        message,
                        new List<Error>()
                    );
                }
                message = "Successfully issueing Line login access token";
                _logger.LogInformation(message);
                return JsonConvert.DeserializeObject<LineLoginIssueTokenResponseDto>(strResponse)!;
            }
        }
    }
}