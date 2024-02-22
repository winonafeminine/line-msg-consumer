using Amazon.S3;
using Amazon.S3.Model;
using Api.LmcLib.DTOs;
using Api.LmcLib.Interfaces;
using Api.LmcLib.Settings;
using Api.LmcLib.Stores;
using Microsoft.Extensions.Options;

namespace Api.LmcLib.Services
{
    public class ObjectStorageService : IObjectStorage
    {
        private readonly IOptions<ObjectStorageSetting> _objSetting;
        public ObjectStorageService(IOptions<ObjectStorageSetting> objSetting)
        {
            _objSetting = objSetting;
        }
        public async Task<string> UploadFile(StaticfileDto staticfileDto)
        {
            string url = $"https://{_objSetting.Value.ServiceUrl}";

            string[] separateDir = new string[]{
                MessageEventTypes.Image,
                MessageEventTypes.Audio,
                MessageEventTypes.Video,
            };

            string[] mediaTypes = staticfileDto.ContentType!.Split("/");
            string fileType = string.Empty;

            if (separateDir.Contains(mediaTypes.First()))
            {
                fileType = mediaTypes.First();
            }
            else
            {
                fileType = "file";
            }

            url = Path.Combine($"https://{_objSetting.Value.ServiceUrl}", fileType);

            var options = new AmazonS3Config
            {
                ServiceURL = url
            };

            var client = new AmazonS3Client(
                _objSetting.Value.AccessKey,
                _objSetting.Value.SecretKey,
                options
            );

            var request = new PutObjectRequest
            {
                BucketName = _objSetting.Value.BucketName,
                Key = staticfileDto.Key,
                ContentType = staticfileDto.ContentType,
                FilePath = staticfileDto.FilePath,
                CannedACL = S3CannedACL.PublicRead,
            };

            await client.PutObjectAsync(request);

            return Path.Combine($"https://{_objSetting.Value.BucketName}.{_objSetting.Value.ServiceUrl}", fileType, staticfileDto.Key!);
        }
    }
}