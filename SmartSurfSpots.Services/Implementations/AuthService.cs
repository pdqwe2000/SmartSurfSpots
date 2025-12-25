using SmartSurfSpots.Data.Repositories;
using SmartSurfSpots.Domain.DTOs;
using SmartSurfSpots.Domain.Entities;
using SmartSurfSpots.Services.Helpers;
using SmartSurfSpots.Services.Interfaces;
using System;
using System.Threading.Tasks;

namespace SmartSurfSpots.Services.Implementations
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly JwtHelper _jwtHelper;

        public AuthService(IUserRepository userRepository, JwtHelper jwtHelper)
        {
            _userRepository = userRepository;
            _jwtHelper = jwtHelper;
        }

        public async Task<LoginResponse> RegisterAsync(RegisterRequest request)
        {
            // Verificar se email já existe
            var existingUser = await _userRepository.GetByEmailAsync(request.Email);
            if (existingUser != null)
            {
                throw new Exception("Email already registered");
            }

            // Criar novo utilizador
            var user = new User
            {
                Name = request.Name,
                Email = request.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
                CreatedAt = DateTime.Now
            };

            await _userRepository.AddAsync(user);

            // Gerar token
            var token = _jwtHelper.GenerateToken(user);

            return new LoginResponse
            {
                Token = token,
                User = new UserDto
                {
                    Id = user.Id,
                    Name = user.Name,
                    Email = user.Email
                }
            };
        }

        public async Task<LoginResponse> LoginAsync(LoginRequest request)
        {
            // Buscar utilizador por email
            var user = await _userRepository.GetByEmailAsync(request.Email);
            if (user == null)
            {
                throw new Exception("Invalid email or password");
            }

            // Verificar password
            if (!BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
            {
                throw new Exception("Invalid email or password");
            }

            // Gerar token
            var token = _jwtHelper.GenerateToken(user);

            return new LoginResponse
            {
                Token = token,
                User = new UserDto
                {
                    Id = user.Id,
                    Name = user.Name,
                    Email = user.Email
                }
            };
        }

        public async Task<UserDto> GetUserByIdAsync(int userId)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
            {
                throw new Exception("User not found");
            }

            return new UserDto
            {
                Id = user.Id,
                Name = user.Name,
                Email = user.Email
            };
        }
    }
}  