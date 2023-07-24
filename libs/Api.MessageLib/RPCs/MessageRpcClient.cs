using Api.CommonLib.DTOs;
using Api.CommonLib.Interfaces;
using Api.CommonLib.Stores;
using Api.MessageLib.Interfaces;
using Api.MessageLib.Settings;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace Api.MessageLib.RPCs
{
    public class MessageRpcClient : IMessageRpcClient
    {
        private readonly ICommonRpcClient _commonRpcClient;

        public MessageRpcClient(ICommonRpcClient commonRpcClient)
        {
            _commonRpcClient = commonRpcClient;
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