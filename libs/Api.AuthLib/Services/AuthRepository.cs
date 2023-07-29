using Api.AuthLib.DTOs;
using Api.AuthLib.Interfaces;
using Api.AuthLib.Models;
using Api.AuthLib.Settings;
using Api.CommonLib.Interfaces;
using Api.CommonLib.Models;
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
        private readonly IMessageRpcClient _messageRpc;
        private readonly IOptions<AuthLineConfigSetting> _lineConfigSetting;
        public AuthRepository(ILogger<AuthRepository> logger, IMessageRpcClient messageRpc, IOptions<AuthLineConfigSetting> lineConfigSetting)
        {
            _authState = new List<LineAuthStateModel>();
            _logger = logger;
            _messageRpc = messageRpc;
            _lineConfigSetting = lineConfigSetting;
        }
        public Response CreateLineAuthState(AuthDto auth)
        {
            // throw new NotImplementedException();
            // generate the state
            string state = ObjectId.GenerateNewId().ToString();
            var messageRpcResponse = _messageRpc.GetChannel();
            string lmcRedirectUri = System.Net.WebUtility.UrlEncode(_lineConfigSetting.Value.RedirectUri!);
            string lineRedirectUri = LineApiReference
                .GetLineAuthorizationUrl(messageRpcResponse.ClientId!, lmcRedirectUri, state);
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