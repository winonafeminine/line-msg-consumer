using Api.AuthLib.DTOs;
using Api.AuthLib.Interfaces;
using Api.CommonLib.DTOs;
using Api.CommonLib.Exceptions;
using Api.CommonLib.Interfaces;
using Api.CommonLib.Models;
using Api.CommonLib.Settings;
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
        private readonly ILineGroupInfo _lineGroup;
        public AuthRepository(ILogger<AuthRepository> logger, IOptions<AuthLineConfigSetting> lineConfigSetting, ILineGroupInfo lineGroup)
        {
            _authState = new List<LineAuthStateModel>();
            _logger = logger;
            _lineConfigSetting = lineConfigSetting;
            _lineGroup = lineGroup;
        }
        public Response CreateLineAuthState(AuthDto auth)
        {
            // throw new NotImplementedException();
            // generate the state
            string state = ObjectId.GenerateNewId().ToString();
            // var messageRpcResponse = _messageRpc.GetChannel();
            string lmcRedirectUri = System.Net.WebUtility.UrlEncode(_lineConfigSetting.Value.RedirectUri!);
            string lineRedirectUri = LineApiReference
                .GetLineAuthorizationUrl(_lineConfigSetting.Value.ClientId!, lmcRedirectUri, state);
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

        public async Task<Response> UpdateLineAuthState(string state, AuthDto auth)
        {
            LineAuthStateModel lineAuthState = _authState
                .FirstOrDefault(x=>x.State==state)!;

            string responseMessage = "";

            if(lineAuthState == null)
            {
                responseMessage = "State not found";
                _logger.LogError(responseMessage);
                throw new ErrorResponseException(
                    StatusCodes.Status404NotFound,
                    responseMessage,
                    new List<Error>()
                );
            }

            // issue line login access token
            LineLoginIssueTokenResponseDto tokenResponse = await _lineGroup.IssueLineLoginAccessToken(auth.Code!);

            // generate the platform access token

            // get line user profile
            LineLoginUserProfileResponseDto userProfile = await _lineGroup.GetLineLoginUserProfile(tokenResponse.AccessToken!);


            // save the data in memory
            lineAuthState.Code=auth.Code;
            lineAuthState.LineAccessToken=tokenResponse.AccessToken;
            lineAuthState.GroupUserId=userProfile.UserId;

            return new Response{
                StatusCode=StatusCodes.Status200OK,
                Data=new LineAuthStateModel{
                    State=lineAuthState.State,
                    GroupUserId=lineAuthState.GroupUserId
                }
            };
        }
    }
}