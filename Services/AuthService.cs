using LibraryApp.Web.DTOs;
using LibraryApp.Web.Models;
using LibraryApp.Web.Repositories;
using System.Threading.Tasks;
using System;

namespace LibraryApp.Web.Services
{
    public class AuthService
    {
        private readonly IUserRepository _repo;

        public AuthService(IUserRepository repo) => _repo = repo;

        private readonly string[] ValidRoles = new[] { "User", "Librarian", "Admin" };

        public async Task<bool> RegisterAsync(RegisterDto dto)
        {
            // Vérifier si l'utilisateur existe déjà
            var existing = await _repo.GetByEmailAsync(dto.Email);
            if (existing != null) return false;

            // Vérifier que le rôle est valide
            var role = ValidRoles.Contains(dto.Role) ? dto.Role : "User";

            var user = new User
            {
                Name = dto.Name,
                Email = dto.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
                Role = role
            };

            await _repo.CreateAsync(user);
            return true;
        }

        public async Task<User?> LoginAsync(LoginDto dto)
        {
            var user = await _repo.GetByEmailAsync(dto.Email);
            if (user == null || !user.IsActive) return null;

            return BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash) ? user : null;
        }
    }
}
