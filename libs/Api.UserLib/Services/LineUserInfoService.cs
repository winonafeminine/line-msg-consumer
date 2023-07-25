using Api.CommonLib.Exceptions;
using Api.CommonLib.Stores;
using Api.UserLib.DTOs;
using Api.UserLib.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Api.UserLib.Services
{
    public class LineUserInfoService : ILineUserInfo
    {
        private readonly ILogger<LineUserInfoService> _logger;
        public LineUserInfoService(ILogger<LineUserInfoService> logger)
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
                    _logger.LogError("One or more error occur");
                    _logger.LogError(strResponse);
                    throw new ErrorResponseException(
                        StatusCodes.Status400BadRequest,
                        "One or more error occur",
                        new List<Error>()
                    );
                }

                return JsonConvert.DeserializeObject<GetGroupMemberProfileDto>(strResponse)!;
            }
        }
    }
}