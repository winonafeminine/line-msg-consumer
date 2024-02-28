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
using Simple.RabbitMQ;

namespace Api.AuthLib.Services
{
    public class AuthService : IAuthService
    {
        private readonly ILogger<AuthService> _logger;
        private readonly IOptions<AuthLineConfigSetting> _lineConfigSetting;
        private readonly ILineGroupInfo _lineGroup;
        private readonly IJwtToken _jwtToken;
        private readonly IHostEnvironment _hostEnv;
        private readonly IMessagePublisher _publisher;
        private readonly IAuthRepository _authRepo;
        public AuthService(ILogger<AuthService> logger, IOptions<AuthLineConfigSetting> lineConfigSetting, ILineGroupInfo lineGroup, IJwtToken jwtToken, IHostEnvironment hostEnv, IMessagePublisher publisher, IAuthRepository authRepo)
        {
            _logger = logger;
            _lineConfigSetting = lineConfigSetting;
            _lineGroup = lineGroup;
            _jwtToken = jwtToken;
            _hostEnv = hostEnv;
            _publisher = publisher;
            _authRepo = authRepo;
        }
        public async Task<Response> CreateLineAuthState(AuthDto auth)
        {
            // throw new NotImplementedException();
            // generate the state
            // var messageRpcResponse = _messageRpc.GetChannel();

            LineAuthStateModel authModel = await _authRepo.CreateAuth(auth);
            string lmcRedirectUri = System.Net.WebUtility.UrlEncode(_lineConfigSetting.Value.RedirectUri!);
            string lineRedirectUri = LineApiReference
                .GetLineAuthorizationUrl(_lineConfigSetting.Value.ClientId!, lmcRedirectUri, authModel.State!);

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
                Data = _authRepo.GetAuths(),
                StatusCode = StatusCodes.Status200OK
            };
        }

        public async Task<Response> UpdateLineAuthState(string state, AuthDto auth)
        {
            LineAuthStateModel lineAuthState = await _authRepo.FindAuth(state);

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
            try
            {
                string strLineAuthState = JsonConvert.SerializeObject(lineAuthState);
                string routingKey = RoutingKeys.Auth["update"];
                _publisher.Publish(strLineAuthState, routingKey, null);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
            }
            finally
            {
                _publisher.Dispose();
            }

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

        public async Task<Response> RemoveStateByPlatformId(string platformId)
        {
            return await _authRepo.RemoveStateByPlatformId(platformId);
        }

        
    }
}