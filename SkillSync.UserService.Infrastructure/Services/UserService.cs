using Microsoft.EntityFrameworkCore;
using SkillSync.UserService.Application.Auth;
using SkillSync.UserService.Application.Contracts;
using SkillSync.UserService.Application.DTOs;
using SkillSync.UserService.Domain.Entities;
using SkillSync.UserService.Infrastructure.Messaging;
using SkillSync.UserService.Infrastructure.Persistance;
namespace SkillSync.UserService.Infrastructure.Services
{
    public class UserService : IUserService
    {
        private readonly AppDbContext _context;
        private readonly IJwtGenerator _jwtGenerator;

        private readonly IKafkaProducer _kafkaProducer;

        public UserService(AppDbContext context, IJwtGenerator jwtGenerator, IKafkaProducer kafkaProducer)
        {
            _context = context;
            _jwtGenerator = jwtGenerator;
            _kafkaProducer = kafkaProducer;
        }

        public async Task<UserDTO>  RegisterAsync(RegisterRequest registerRequest)
        {
            var exists = await _context.Users.AnyAsync(u => u.Email == registerRequest.Email);
            if(exists)
                throw new Exception("Email already registered");
            var user = new User{
                Id = Guid.NewGuid(),
                Username = registerRequest.UserName,
                Email = registerRequest.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(registerRequest.Password)
            };
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            await _kafkaProducer.PublishAsync("user-registered", new UserRegisteredEvent 
            {
                Email = user.Email,
                FullName = user.Username,
                UserId = user.Id,
                CreatedAt = DateTime.Now
            });

            return new UserDTO {Id = user.Id, Email = user.Email};
            
        }

        public async Task<AuthResponse> LoginAsync(LoginRequest loginRequest)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == loginRequest.Email);
            if(user == null || !BCrypt.Net.BCrypt.Verify(loginRequest.Password, user.PasswordHash))
            {
                throw new Exception("Invalid email or password");
            }
            return new AuthResponse
            {
                UserName = user.Username,
                Token = _jwtGenerator.GenerateToken(user)
            };
        }

        public async Task<List<SkillDto>> GetSkillsAsync(Guid userId)
        {
            var user = await _context.Users.Include(u => u.Skills).FirstOrDefaultAsync(u => u.Id == userId);
            if(user ==  null)
               throw new Exception("User not found");
            return user.Skills.Select(s => new SkillDto{
                Name = s.Name,
                Level = s.Level
            }).ToList();
        }

        public async Task AddSkillAsync(Guid userId, SkillDto skillDto)
        {
             var user = await _context.Users.Include(u => u.Skills).FirstOrDefaultAsync(u => u.Id == userId);
            if(user ==  null)
               throw new Exception("User not found");
            var skill = new Skill
            {
                Name = skillDto.Name,
                Level = skillDto.Level,
                UserId = user.Id
            };
            _context.Skills.Add(skill);
            await _context.SaveChangesAsync();
        }
    }
}