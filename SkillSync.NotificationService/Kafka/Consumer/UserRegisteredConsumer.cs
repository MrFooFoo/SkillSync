using Confluent.Kafka;
using SkillSync.NotificationService.DTOs;
using SkillSync.NotificationService.Persistance;
using SkillSync.UserService.Application.DTOs;
using System.Text.Json;

namespace SkillSync.NotificationService.Kafka.Consumer
{
    public class UserRegisteredConsumer : BackgroundService
    {
        private readonly ILogger<UserRegisteredConsumer> _logger;
        private readonly IConsumer<Ignore, string> _consumer;
        private readonly NotificationDbContext _context;

        public UserRegisteredConsumer(ILogger<UserRegisteredConsumer> logger, IConfiguration config, NotificationDbContext context)
        {
            _logger = logger;
            _context = context;

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
           Task.Run(() =>
           {
                _consumer.Subscribe("user-registered");
                while (!stoppingToken.IsCancellationRequested)
                {
                    var consumeResult = _consumer.Consume(stoppingToken);
                    var userEvent = JsonSerializer.Deserialize<UserRegisteredEvent>(consumeResult.Message.Value);
                    Console.WriteLine($"Sending welcome email to {userEvent.Email}");
                    SendWelcomeEmail(userEvent);
                }
           }, stoppingToken);
            return Task.CompletedTask;
        }

        private void SendWelcomeEmail(UserRegisteredEvent userEvent)
        {
            var log = new EmailLog
            {
                Id = userEvent.UserId,
                Email = userEvent.Email,
                Subject = "Welcome to Skill Sync",
                Body = $"Hi {userEvent.FullName}",
                SentAt = DateTime.UtcNow,
            };
            try
            {
                Console.WriteLine($"Email sent to {userEvent.Email}");
                log.IsSuccess = true;
            }
            catch (Exception ex)
            {
                log.IsSuccess= true;
                log.ErrorMessage = ex.Message;
            }
            _context.EmailLogs.Add(log);
            _context.SaveChanges();
        }

        public override void Dispose()
        {
            _consumer.Close();
            _consumer.Dispose();
            base.Dispose();
        }
    }
}