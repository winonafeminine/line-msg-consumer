using Api.UserLib.DTOs;

namespace Api.UserLib.Interfaces
{
    public interface ILineUserInfo
    {
        public Task<GetGroupMemberProfileDto> GetGroupMemberProfile(string groupId, string userId, string channelAccessToken);
    }
}