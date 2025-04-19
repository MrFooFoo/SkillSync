using SkillSync.UserService.Domain.Entities;

namespace SkillSync.UserService.Application.Auth;
public interface IJwtGenerator{
    string GenerateToken(User user);
}