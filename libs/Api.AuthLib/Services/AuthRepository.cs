using Api.AuthLib.DTOs;
using Api.AuthLib.Interfaces;
using Api.AuthLib.Models;
using Api.AuthLib.Settings;
using Api.CommonLib.Interfaces;
using Api.CommonLib.Models;
using Api.CommonLib.Setttings;
using Api.CommonLib.Stores;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MongoDB.Bson;

namespace Api.AuthLib.Services
{
    public class AuthRepository : IAuthRepository
    {
        private IList<LineAuthStateModel> _authState;
        private readonly ILogger<AuthRepository> _logger;
        private readonly IOptions<AuthLineConfigSetting> _lineConfigSetting;
        private readonly IOptions<LineChannelSetting> _channelSetting;
        public AuthRepository(ILogger<AuthRepository> logger, IOptions<AuthLineConfigSetting> lineConfigSetting, IOptions<LineChannelSetting> channelSetting)
        {
            _authState = new List<LineAuthStateModel>();
            _logger = logger;
            _lineConfigSetting = lineConfigSetting;
            _channelSetting = channelSetting;
        }
        public Response CreateLineAuthState(AuthDto auth)
        {
            // throw new NotImplementedException();
            // generate the state
            string state = ObjectId.GenerateNewId().ToString();
            // var messageRpcResponse = _messageRpc.GetChannel();
            string lmcRedirectUri = System.Net.WebUtility.UrlEncode(_lineConfigSetting.Value.RedirectUri!);
            string lineRedirectUri = LineApiReference
                .GetLineAuthorizationUrl(_channelSetting.Value.ClientId!, lmcRedirectUri, state);
            _authState.Add(
                new LineAuthStateModel{
                    State=state,
                    AppRedirectUri=auth.AppRedirectUri
                }
            );

            return new Response{
                Data=new AuthDto{
                    LineRedirectUri=lineRedirectUri
                },
                StatusCode=StatusCodes.Status201Created
            };

        }

        public Response GetLineAuthStates()
        {
            return new Response{
                Data=_authState,
                StatusCode=StatusCodes.Status200OK
            };
        }
    }
}