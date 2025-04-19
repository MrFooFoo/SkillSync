namespace SkillSync.UserService.Domain.Entities
{
        public class Skill{
            public Guid Id {get; set;}
            public string Name {get; set;} = string.Empty;
            public string Level {get; set;} = "Beginner";

            public Guid UserId {get; set;}
        
            public User? User {get; set;}
        }
}
