namespace Api.CommonLib.Stores
{
    public class RpcQueueNames
    {
        public static IDictionary<string, string> Message = new Dictionary<string, string>{
            { "GetChannel", "lmc_message_rpc_queue" }
        };
    }
}