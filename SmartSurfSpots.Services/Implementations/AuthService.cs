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

        /// <summary>
        /// Regista um novo utilizador, faz hash da password e gera o primeiro token.
        /// </summary>
        public async Task<LoginResponse> RegisterAsync(RegisterRequest request)
        {
            // 1. Validar unicidade
            var existingUser = await _userRepository.GetByEmailAsync(request.Email);
            if (existingUser != null)
            {
                throw new Exception("Email already registered");
            }

            // 2. Criar entidade e Hash da Password (Nunca guardar plain text!)
            var user = new User
            {
                Name = request.Name,
                Email = request.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
                CreatedAt = DateTime.UtcNow
            };

            await _userRepository.AddAsync(user);

            // 3. Gerar token para login automático após registo
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

        /// <summary>
        /// Verifica credenciais e retorna o token de acesso.
        /// </summary>
        public async Task<LoginResponse> LoginAsync(LoginRequest request)
        {
            var user = await _userRepository.GetByEmailAsync(request.Email);

            // É boa prática usar mensagens genéricas para não revelar se o email existe ou não
            if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
            {
                throw new Exception("Invalid email or password");
            }

            var token = _jwtHelper.GenerateToken(user);

            return new LoginResponse
            {
                Token = token,
                User = new UserDto { Id = user.Id, Name = user.Name, Email = user.Email }
            };
        }

        public async Task<UserDto> GetUserByIdAsync(int userId)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null) throw new Exception("User not found");

            return new UserDto { Id = user.Id, Name = user.Name, Email = user.Email };
        }
    }
}