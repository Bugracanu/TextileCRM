using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TextileCRM.Application.Interfaces;
using TextileCRM.Domain.Entities;

namespace TextileCRM.WebUI.Services
{
    public class MockAuthService : IAuthService
    {
        private readonly List<User> _users;

        public MockAuthService()
        {
            _users = UserStore.Instance.Users;
        }

        public async Task<User?> ValidateUserAsync(string username, string password)
        {
            var user = _users.FirstOrDefault(u => 
                u.Username == username && 
                u.PasswordHash == password && 
                u.IsActive);
            
            return await Task.FromResult(user);
        }

        public async Task<User?> GetUserByIdAsync(int id)
        {
            return await Task.FromResult(_users.FirstOrDefault(u => u.Id == id));
        }

        public async Task<User?> GetUserByUsernameAsync(string username)
        {
            return await Task.FromResult(_users.FirstOrDefault(u => u.Username == username));
        }
    }
}

