using Api.CommonLib.Interfaces;
using Api.MessageLib.Interfaces;
using Api.MessageLib.Models;
using Api.MessageLib.Stores;
using Api.ReferenceLib.Interfaces;
using Api.ReferenceLib.Setttings;
using Api.ReferenceLib.Stores;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using Newtonsoft.Json;

namespace Api.CommonLib.Services
{
    public class MessageConsumer : IMessageConsumer
    {
        private readonly ILogger<MessageConsumer> _logger;
        private readonly ILineGroupInfo _lineSv;
        private readonly IOptions<LineChannelSetting> _lineSetting;
        private readonly IObjectStorage _objSv;
        private readonly IMessageRepository _messageRepo;
        public MessageConsumer(ILogger<MessageConsumer> logger, ILineGroupInfo lineSv, IOptions<LineChannelSetting> lineSetting, IObjectStorage objSv, IMessageRepository messageRepo)
        {
            _logger = logger;
            _lineSv = lineSv;
            _lineSetting = lineSetting;
            _objSv = objSv;
            _messageRepo = messageRepo;
        }
        public async Task ConsumeMessageCreate(string message)
        {
            MessageModel msgModel = JsonConvert
                .DeserializeObject<MessageModel>(message)!;

            BsonDocument document = BsonDocument
                .Parse(message);

            try
            {
                string messageId = document["message_object"]["events"][0]["message"]["id"].ToString()!;
                string messageType = document["message_object"]["events"][0]["message"]["type"].ToString()!;

                if (messageType == MessageEventTypes.Location ||
                    messageType == MessageEventTypes.Text)
                {
                    return;
                }

                string staticUrl = string.Empty;
                // handl type=sticker
                if (messageType == MessageEventTypes.Sticker)
                {
                    string stickerId = document["message_object"]["events"][0]["message"]["stickerId"].ToString()!;
                    staticUrl = LineApiReference.GetStickerUrl(stickerId);
                }
                else
                {
                    // save file locally
                    var staticfile = await _lineSv.GetContent(messageId, _lineSetting.Value.ChannelAccessToken!);
                    // upload file to linode
                    staticUrl = await _objSv.UploadFile(staticfile);
                }

                // update static url in message
                document["message_object"]["events"][0]["message"]["static_url"] = staticUrl;

                try
                {
                    // MessageModel replaceModel = JsonConvert.DeserializeObject<MessageModel>(document.ToJson())!;
                    await _messageRepo.ReplaceMessage(document);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex.Message);
                }
            }
            catch
            {
                // _logger.LogError(ex.Message);
            }
            return;
        }
    }
}