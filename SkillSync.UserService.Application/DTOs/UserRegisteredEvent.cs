namespace SkillSync.UserService.Application.DTOs;
public class UserRegisteredEvent
{
    public string Email {get; set;} = string.Empty;

    public Guid UserId {get; set;}

    public DateTime CreatedAt {get; set;}
    public string FullName { get; set; }
}