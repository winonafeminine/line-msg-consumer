using Api.AuthLib.Grpcs;
using Api.AuthLib.Interfaces;
using Api.CommonLib.Services;
using Api.CommonLib.Setttings;
using Api.MessageLib.Interfaces;
using Api.MessageLib.Services;
using Api.MessageSv.Grpcs;
using Api.MessageSv.HostedServices;
using Api.ReferenceLib.Exceptions;
using Api.ReferenceLib.Interfaces;
using Api.ReferenceLib.Services;
using Api.ReferenceLib.Settings;
using Api.ReferenceLib.Setttings;
using Api.UserLib.Interfaces;
using Api.UserLib.Services;
using Newtonsoft.Json.Serialization;
using RabbitMQ.Client;
using Simple.RabbitMQ;

var builder = WebApplication.CreateBuilder(args);

IConfiguration configuration = builder.Configuration;

// Add services to the container.

// configure controller to use Newtonsoft as a default serializer
// add some comment here
builder.Services.AddCors(options =>
{
  options.AddPolicy(name: "message", build =>
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
builder.Services.AddSingleton<IMessageService, MessageService>();
builder.Services.AddSingleton<ILineMessageValidation, LineMessageValidation>();
builder.Services.AddSingleton<IBasicConnection>(new BasicConnection(configuration["RabbitMQConfig:HostName"], true));
// Register message subscriber
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
// Register message publisher
builder.Services.AddScoped<IMessagePublisher>(x =>
    new MessagePublisher(
        x.GetRequiredService<IBasicConnection>(),
        configuration["RabbitMQConfig:ExchangeName"], // exhange name
        ExchangeType.Topic // exchange type
    ));
builder.Services.AddHostedService<MessageDataCollector>();
builder.Services.Configure<RabbitmqConfigSetting>(configuration.GetSection("RabbitMQConfig"));
builder.Services.Configure<LineChannelSetting>(configuration.GetSection("LineConfig:Channel"));
builder.Services.Configure<GrpcConfigSetting>(configuration.GetSection("GrpcConfig"));
builder.Services.Configure<MongoConfigSetting>(configuration.GetSection("MongoConfig"));
builder.Services.AddSingleton<IUserRepository, UserRepository>();
builder.Services.AddSingleton<IChatRepository, ChatRepository>();
builder.Services.AddSingleton<IAuthGrpcClientService, AuthGrpcClientService>();
builder.Services.AddSingleton<IScopePublisher, ScopePublisher>();
builder.Services.AddSingleton<ISpecialKeywordHandler, SpecialKeywordHandler>();
builder.Services.AddSingleton<IMessageRepository, MessageRepository>();
builder.Services.AddGrpc();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
else
{
    app.UseResponseExceptionHandler();
}

app.UseCors("message");
// app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();
app.MapGrpcService<MessageGrpcServerService>();


app.Run();
