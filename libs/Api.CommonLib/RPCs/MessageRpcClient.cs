using Api.CommonLib.DTOs;
using Api.CommonLib.Interfaces;
using Api.CommonLib.Models;
using Api.CommonLib.Setttings;
using Api.CommonLib.Stores;
using Newtonsoft.Json;

namespace Api.CommonLib.RPCs
{
    public class MessageRpcClient : IMessageRpcClient
    {
        private readonly ICommonRpcClient _commonRpcClient;

        public MessageRpcClient(ICommonRpcClient commonRpcClient)
        {
            _commonRpcClient = commonRpcClient;
        }

        public Task AddUser(UserModel user)
        {
            throw new NotImplementedException();
        }

        public LineChannelSetting GetChannel()
        {
            CommonRpcRequest rpcRequest = new CommonRpcRequest{
                Action=RpcActions.Message["GetChannel"]
            };

            string request = JsonConvert.SerializeObject(rpcRequest);

            string response = _commonRpcClient
                .SendRPCRequest(request, RpcQueueNames.Message);

            LineChannelSetting channelSetting = JsonConvert
                .DeserializeObject<LineChannelSetting>(response)!;
            
            return channelSetting;
        }
    }
}