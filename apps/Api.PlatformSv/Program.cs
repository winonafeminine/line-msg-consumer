using Api.AuthLib.Grpcs;
using Api.AuthLib.Interfaces;
using Api.PlatformLib.Interfaces;
using Api.PlatformLib.Services;
using Api.PlatformSv.HostedServices;
using Api.ReferenceLib.Exceptions;
using Api.ReferenceLib.Interfaces;
using Api.ReferenceLib.Services;
using Api.ReferenceLib.Settings;
using Api.ReferenceLib.Setttings;
using Newtonsoft.Json.Serialization;
using RabbitMQ.Client;
using Simple.RabbitMQ;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;

// Add services to the container.

// configure controller to use Newtonsoft as a default serializer
builder.Services.AddCors(options =>
{
  options.AddPolicy(name: "platform", build =>
  {
    build.WithOrigins(configuration["AllowedHosts"])
        .AllowAnyHeader()
        .AllowAnyMethod();

  });
});
builder.Services.AddControllers()
    .AddNewtonsoftJson(options =>
    {
        options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
        options.SerializerSettings.ContractResolver = new DefaultContractResolver();
    });
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHostedService<PlatformDataCollector>();
// Register message subscriber
builder.Services.AddSingleton<IBasicConnection>(new BasicConnection(configuration["RabbitMQConfig:HostName"], true));
builder.Services.AddSingleton<IMessageSubscriber>(x =>
    new MessageSubscriber(
        x.GetRequiredService<IBasicConnection>(),
        configuration["RabbitMQConfig:ExchangeName"], // exhange name
        configuration["RabbitMQConfig:QueueName"], // queue name
        configuration["RabbitMQConfig:RoutingKey"], // routing key
        ExchangeType.Topic, // exchange type
        autoAck: bool.Parse(configuration["RabbitMQConfig:Properties:AutoAck"]),
        prefetchCount: ushort.Parse(configuration["RabbitMQConfig:Properties:PrefetchCount"])
    ));
builder.Services.AddScoped<IMessagePublisher>(x =>
    new MessagePublisher(
        x.GetRequiredService<IBasicConnection>(),
        configuration["RabbitMQConfig:ExchangeName"], // exhange name
        ExchangeType.Topic // exchange type
    ));
builder.Services.AddSingleton<IPlatformRepository, PlatformRepository>();
builder.Services.Configure<MongoConfigSetting>(configuration.GetSection("MongoConfig"));
builder.Services.Configure<GrpcConfigSetting>(configuration.GetSection("GrpcConfig"));
builder.Services.AddSingleton<IPlatformService, PlatformService>();
builder.Services.AddSingleton<IScopePublisher, ScopePublisher>();
builder.Services.AddSingleton<IAuthGrpcClientService, AuthGrpcClientService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}else{
    app.UseResponseExceptionHandler();
}

app.UseCors("platform");

// app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
