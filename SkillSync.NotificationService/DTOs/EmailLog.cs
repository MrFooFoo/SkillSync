namespace SkillSync.NotificationService.DTOs
{
    public class EmailLog
    {
        public Guid Id { get; set; }
        public string Email { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
        public DateTime SentAt { get; set; }

        public bool IsSuccess { get; set; }

        public string ErrorMessage { get; set; }
    }
}
