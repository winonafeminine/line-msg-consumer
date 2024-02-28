using Api.LmcLib.DTOs;
using Api.LmcLib.Interfaces;
using Api.LmcLib.Models;
using Api.LmcLib.Settings;
using Api.LmcLib.Stores;
using Api.LmcLib.Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using Newtonsoft.Json;
using Simple.RabbitMQ;

namespace Api.LmcLib.Services
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
        private readonly IPlatformRepository _platformRepo;
        public AuthService(ILogger<AuthService> logger, IOptions<AuthLineConfigSetting> lineConfigSetting, ILineGroupInfo lineGroup, IJwtToken jwtToken, IHostEnvironment hostEnv, IMessagePublisher publisher, IAuthRepository authRepo, IPlatformRepository platformRepo)
        {
            _logger = logger;
            _lineConfigSetting = lineConfigSetting;
            _lineGroup = lineGroup;
            _jwtToken = jwtToken;
            _hostEnv = hostEnv;
            _publisher = publisher;
            _authRepo = authRepo;
            _platformRepo = platformRepo;

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
            JwtPayloadData payload = new JwtPayloadData();
            PlatformModel platformModel = new PlatformModel();
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
            // generate the platform_id
            string platformId = ObjectId.GenerateNewId().ToString();
            string secretKey = SecretKeyGenerator.GenerateSecretKey();
            // generate the platform access token
            string accessToken = _jwtToken.GenerateJwtToken(secretKey, JwtIssuers.Platform, platformId, DateTime.UtcNow.AddMonths(6));

            PlatformModel newPlatform = new PlatformModel
            {
                PlatformId = platformId,
                SecretKey = secretKey,
                AccessToken = accessToken

            };
            platformModel = newPlatform;
            try
            {
                // Use the injected _platformRepository to add the new platform
                await _platformRepo.AddPlatform(platformModel);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error adding new platform: {ex.Message}");
                // Handle the exception as needed
            }


            // issue line login access token
            LineLoginIssueTokenResponseDto tokenResponse = new LineLoginIssueTokenResponseDto();

            // get line user profile
            LineLoginUserProfileResponseDto userProfile = new LineLoginUserProfileResponseDto();
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

        public Task<LineAuthStateModel> CreateAuth(AuthDto auth)
        {
            throw new NotImplementedException();
        }
    }
}