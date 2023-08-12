using Api.ReferenceLib.DTOs;

namespace Api.ReferenceLib.Interfaces
{
    public interface ILineGroupInfo
    {
        // line messaging api
        public Task<GetGroupMemberProfileDto> GetGroupMemberProfile(string groupId, string userId, string channelAccessToken);
        public Task<GetGroupSummaryDto> GetGroupSummary(string groupId, string channelAccessToken);

        // line login
        public Task<LineLoginIssueTokenResponseDto> IssueLineLoginAccessToken(string code);
        public Task<LineLoginUserProfileResponseDto> GetLineLoginUserProfile(string accessToken);
    }
}