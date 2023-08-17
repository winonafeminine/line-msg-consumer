using Api.AuthLib.DTOs;
using Api.AuthLib.Interfaces;
using Api.AuthLib.Models;
using Api.AuthLib.Settings;
using Api.AuthLib.Stores;
using Api.ReferenceLib.DTOs;
using Api.ReferenceLib.Exceptions;
using Api.ReferenceLib.Interfaces;
using Api.ReferenceLib.Models;
using Api.ReferenceLib.Stores;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using Newtonsoft.Json;

namespace Api.AuthLib.Services
{
    public class AuthRepository : IAuthRepository
    {
        private IList<LineAuthStateModel> _authState;
        private readonly ILogger<AuthRepository> _logger;
        private readonly IOptions<AuthLineConfigSetting> _lineConfigSetting;
        private readonly ILineGroupInfo _lineGroup;
        private readonly IJwtToken _jwtToken;
        private readonly IHostEnvironment _hostEnv;
        private readonly IScopePublisher _publisher;
        public AuthRepository(ILogger<AuthRepository> logger, IOptions<AuthLineConfigSetting> lineConfigSetting, ILineGroupInfo lineGroup, IJwtToken jwtToken, IHostEnvironment hostEnv, IScopePublisher publisher)
        {
            _authState = new List<LineAuthStateModel>();
            _logger = logger;
            _lineConfigSetting = lineConfigSetting;
            _lineGroup = lineGroup;
            _jwtToken = jwtToken;
            _hostEnv = hostEnv;
            _publisher = publisher;
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
                new LineAuthStateModel
                {
                    State = state,
                    AppRedirectUri = auth.AppRedirectUri
                }
            );

            return new Response
            {
                Data = new AuthDto
                {
                    LineRedirectUri = lineRedirectUri
                },
                StatusCode = StatusCodes.Status201Created
            };

        }

        public Response GetLineAuthStates()
        {
            return new Response
            {
                Data = _authState,
                StatusCode = StatusCodes.Status200OK
            };
        }

        public async Task<Response> UpdateLineAuthState(string state, AuthDto auth)
        {
            LineAuthStateModel lineAuthState = _authState
                .FirstOrDefault(x => x.State == state)!;

            string responseMessage = "";

            if (lineAuthState == null)
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
            LineLoginIssueTokenResponseDto tokenResponse = new LineLoginIssueTokenResponseDto();

            // get line user profile
            LineLoginUserProfileResponseDto userProfile = new LineLoginUserProfileResponseDto();

            // generate the platform_id
            string platformId = ObjectId.GenerateNewId().ToString();

            // generate the platform access token
            string secretKey = SecretKeyGenerator.GenerateSecretKey();
            string accessToken = _jwtToken.GenerateJwtToken(secretKey, JwtIssuers.Platform, platformId, DateTime.UtcNow.AddMonths(6));

            // if (!_hostEnv.IsDevelopment())
            // {
            // }
            // issue line login access token
            tokenResponse = await _lineGroup.IssueLineLoginAccessToken(auth.Code!);
            // get line user profile
            userProfile = await _lineGroup.GetLineLoginUserProfile(tokenResponse.AccessToken!);

            // save the data in memory
            lineAuthState.Code = auth.Code;
            lineAuthState.LineAccessToken = tokenResponse.AccessToken;
            lineAuthState.Token = tokenResponse;
            lineAuthState.UserProfile = userProfile;
            lineAuthState.GroupUserId = userProfile.UserId;
            lineAuthState.DisplayName = userProfile.DisplayName;
            lineAuthState.PictureUrl = userProfile.PictureUrl;
            lineAuthState.StatusMessage = userProfile.StatusMessage;
            lineAuthState.SecretKey = secretKey;
            lineAuthState.AccessToken = accessToken;
            lineAuthState.PlatformId = platformId;

            // publish the LineAuthStateModel
            string strLineAuthState = JsonConvert.SerializeObject(lineAuthState);
            string routingKey = RoutingKeys.Auth["update"];
            _publisher.Publish(strLineAuthState, routingKey, null);

            return new Response
            {
                StatusCode = StatusCodes.Status200OK,
                Data = new LineAuthStateModel
                {
                    State = lineAuthState.State,
                    GroupUserId = lineAuthState.GroupUserId,
                    AccessToken = accessToken,
                    PlatformId = platformId,
                    AppRedirectUri = lineAuthState.AppRedirectUri
                }
            };
        }

        public Response RemoveStateByPlatformId(string platformId)
        {
            LineAuthStateModel authModel = _authState.FirstOrDefault(x => x.PlatformId == platformId)!;

            string resMsg = string.Empty;
            if (authModel == null)
            {
                resMsg = "State not found";
                _logger.LogError(resMsg);
                throw new ErrorResponseException(
                    StatusCodes.Status404NotFound,
                    resMsg,
                    new List<Error>()
                );
            }

            _authState.Remove(authModel);
            resMsg = "State removed!";
            _logger.LogInformation(resMsg);
            return new Response
            {
                Message = resMsg,
                StatusCode = StatusCodes.Status200OK,
            };
        }
    }
}