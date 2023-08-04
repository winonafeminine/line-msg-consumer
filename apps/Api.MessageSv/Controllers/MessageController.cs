using Api.AuthLib.Protos;
using Api.CommonLib.Interfaces;
using Grpc.Net.Client;
using Microsoft.AspNetCore.Mvc;

namespace Api.MessageSv.Controllers
{
    [ApiController]
    [Route("/message/v1")]
    public class MessagesController : ControllerBase
    {
        private readonly ILineMessaging _lineMessagin;
        public MessagesController(ILineMessaging lineMessagin)
        {
            _lineMessagin = lineMessagin;
        }

        [HttpPost]
        [Route("webhook/line/{id}")]
        public async Task<ActionResult> RetrieveLineMessage([FromBody] object content, string id)
        {
            await Task.Yield();
            string signature = HttpContext.Request.Headers["X-Line-Signature"].ToString();
            await _lineMessagin.RetriveLineMessage(content, signature, id);
            return Ok();
        }

        [HttpGet]
        [Route("messages")]
        public async Task<ActionResult> GetMessages()
        {
            // The port number must match the port of the gRPC server.
            using var channel = GrpcChannel.ForAddress("http://localhost:5171");
            var client = new Greeter.GreeterClient(channel);
            var reply = await client.SayHelloAsync(
                              new HelloRequest { Name = "GreeterClient" });
            return Ok(reply);
        }
    }
}