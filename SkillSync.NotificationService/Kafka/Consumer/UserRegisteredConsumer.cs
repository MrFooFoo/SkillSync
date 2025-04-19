using Confluent.Kafka;
using System.Text.Json;

namespace SkillSync.NotificationService.Kafka.Consumer
{
    public class UserRegisteredConsumer : BackgroundService
    {
        private readonly ILogger<UserRegisteredConsumer> _logger;
        private readonly IConsumer<Ignore, string> _consumer;

        public UserRegisteredConsumer(ILogger<UserRegisteredConsumer> logger, IConfiguration config)
        {
            _logger = logger;

            var consumerConfig = new ConsumerConfig
            {
                BootstrapServers = config["Kafka:BootstrapServers"],
                GroupId = "notification-group",
                AutoOffsetReset = AutoOffsetReset.Earliest
            };

            _consumer = new ConsumerBuilder<Ignore, string>(consumerConfig).Build();
            _consumer.Subscribe("user-registered-topic");
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            return Task.Run(() =>
            {
                while (!stoppingToken.IsCancellationRequested)
                {
                    var consumeResult = _consumer.Consume(stoppingToken);
                    var json = consumeResult.Message.Value;

                    _logger.LogInformation($"Received user registration event: {json}");

                    // TODO: Deserialize and act
                }
            });
        }

        public override void Dispose()
        {
            _consumer.Close();
            _consumer.Dispose();
            base.Dispose();
        }
    }
}