using SkillSync.UserService.Application.DTOs;
using SkillSync.UserService.Domain.Entities;
namespace SkillSync.UserService.Application.Contracts
{
    public interface IUserService {
        Task<UserDTO> RegisterAsync(RegisterRequest request);
        Task<AuthResponse> LoginAsync(LoginRequest request);
        Task<List<SkillDto>> GetSkillsAsync(Guid userId);
        Task AddSkillAsync(Guid userId, SkillDto skillDto);
    }
}