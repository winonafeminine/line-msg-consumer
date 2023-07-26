using Api.CommonLib.DTOs;
using Api.CommonLib.Exceptions;
using Api.CommonLib.Interfaces;
using Api.CommonLib.Stores;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Api.CommonLib.Services
{
    public class LineGroupInfoService : ILineGroupInfo
    {
        private readonly ILogger<LineGroupInfoService> _logger;
        public LineGroupInfoService(ILogger<LineGroupInfoService> logger)
        {
            _logger = logger;
        }
        public async Task<GetGroupMemberProfileDto> GetGroupMemberProfile(string groupId, string userId, string channelAccessToken)
        {
            string fullUrl = LineApiReference.GetGroupMemberProfile(groupId, userId);
            // StringContent stringContent = new StringContent();
            using( var httpClient = new HttpClient())
            {
                httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {channelAccessToken}");
                HttpResponseMessage response = await httpClient.GetAsync(fullUrl);
                response.EnsureSuccessStatusCode();
                string strResponse = await response.Content.ReadAsStringAsync();

                if(!response.IsSuccessStatusCode)
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
            using( var httpClient = new HttpClient())
            {
                httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {channelAccessToken}");
                HttpResponseMessage response = await httpClient.GetAsync(fullUrl);
                response.EnsureSuccessStatusCode();
                string strResponse = await response.Content.ReadAsStringAsync();

                if(!response.IsSuccessStatusCode)
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
    }
}