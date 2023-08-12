namespace Api.CommonLib.Setttings
{
    // "HostName": "amqp://root:password@192.168.49.2:30001",
    // "ExchangeName": "lmc_message_exchange",
    // "RoutingKey": "lmc.*.*",
    // "QueueName": "lmc_message_queue",
    // "Properties": {
    //     "PrefetchCount": 100,
    //     "AutoAck": false
    // }
    public class RabbitmqConfigSetting
    {
        public virtual string? HostName { get; set; }
        public virtual string? ExchangeName { get; set; }
        public virtual string? RoutingKey { get; set; }
        public virtual string? QueueName { get; set; }
        public virtual RabbitmqConfigPropertiesSetting? Properties { get; set; }

    }

    public class RabbitmqConfigPropertiesSetting
    {
        public virtual ushort PrefetchCount { get; set; }
        public virtual bool AutoAck { get; set; }
    }
}