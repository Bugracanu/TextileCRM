using System;
using System.Collections.Generic;
using TextileCRM.Domain.Entities;

namespace TextileCRM.WebUI.Services
{
    public class UserStore
    {
        private static UserStore? _instance;
        private static readonly object _lock = new object();
        
        public List<User> Users { get; private set; }

        private UserStore()
        {
            Users = new List<User>
            {
                new User 
                { 
                    Id = 1, 
                    Username = "admin", 
                    PasswordHash = "admin123",
                    Role = "Admin", 
                    IsActive = true,
                    CreatedDate = DateTime.Now.AddMonths(-6)
                },
                new User 
                { 
                    Id = 2, 
                    Username = "manager", 
                    PasswordHash = "manager123",
                    Role = "Manager", 
                    IsActive = true,
                    CreatedDate = DateTime.Now.AddMonths(-3)
                },
                new User 
                { 
                    Id = 3, 
                    Username = "user", 
                    PasswordHash = "user123",
                    Role = "User", 
                    IsActive = true,
                    CreatedDate = DateTime.Now.AddMonths(-1)
                }
            };
        }

        public static UserStore Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_lock)
                    {
                        if (_instance == null)
                        {
                            _instance = new UserStore();
                        }
                    }
                }
                return _instance;
            }
        }
    }
}

