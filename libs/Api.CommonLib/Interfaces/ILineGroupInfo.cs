using Api.CommonLib.DTOs;

namespace Api.CommonLib.Interfaces
{
    public interface ILineGroupInfo
    {
        public Task<GetGroupMemberProfileDto> GetGroupMemberProfile(string groupId, string userId, string channelAccessToken);
        public Task<GetGroupSummaryDto> GetGroupSummary(string groupId, string channelAccessToken);
    }
}