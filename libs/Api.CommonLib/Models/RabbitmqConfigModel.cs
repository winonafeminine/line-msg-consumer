namespace Api.CommonLib.Models
{
    // "HostName": "amqp://root:password@192.168.49.2:30001",
    // "ExchangeName": "lmc_message_exchange",
    // "RoutingKey": "lmc.*.*",
    // "QueueName": "lmc_message_queue",
    // "Properties": {
    //     "PrefetchCount": 100,
    //     "AutoAck": false
    // }
    public class RabbitmqConfigModel
    {
        public virtual string? HostName { get; set; }
        public virtual string? ExchangeName { get; set; }
        public virtual string? RoutingKey { get; set; }
        public virtual string? QueueName { get; set; }
        public virtual RabbitmqConfigPropertiesModel? Properties { get; set; }

    }

    public class RabbitmqConfigPropertiesModel
    {
        public virtual ushort PrefetchCount { get; set; }
        public virtual bool AutoAck { get; set; }
    }
}