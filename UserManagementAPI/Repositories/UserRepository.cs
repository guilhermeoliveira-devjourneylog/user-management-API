using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using UserManagementAPI.Interfaces;
using UserManagementAPI.Models;

namespace UserManagementAPI.Repositories
{
    public class UserRepository : IUserRepository
    {
        private const string FilePath = "Data/users.json";
        private readonly List<User> _users;
        private readonly object _lock = new();
        private readonly ILogger<UserRepository> _logger;

        public UserRepository(ILogger<UserRepository> logger)
        {
            _logger = logger;
            var directory = Path.GetDirectoryName(FilePath);
            if (!Directory.Exists(directory))
            {
                if (directory != null)
                {
                    Directory.CreateDirectory(directory);
                    _logger.LogInformation($"Directory '{directory}' created.");
                }
                _logger.LogInformation($"Directory '{directory}' created.");
            }

            if (File.Exists(FilePath))
            {
                var jsonData = File.ReadAllText(FilePath);
                _users = JsonSerializer.Deserialize<List<User>>(jsonData, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true, 
                    AllowTrailingCommas = true,         
                    DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull
                }) ?? new List<User>();

                _logger.LogInformation("User data loaded from file.");
            }
            else
            {
                _users = new List<User>();
                _logger.LogInformation("No existing user data found. Initialized empty list.");
            }
        }

        private void SaveToFile()
        {
            try
            {
                var jsonData = JsonSerializer.Serialize(_users, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(FilePath, jsonData);
                _logger.LogInformation("User data successfully saved to file.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving data to file.");
            }
        }

        private void ValidateUser(User user)
        {
            if (string.IsNullOrWhiteSpace(user.FirstName) || string.IsNullOrWhiteSpace(user.LastName))
            {
                throw new ArgumentException("First and last names cannot be empty.");
            }

            if (string.IsNullOrWhiteSpace(user.Email) || !user.Email.Contains("@"))
            {
                throw new ArgumentException("Invalid email address.");
            }
        }

        public IEnumerable<User> GetAllUsers()
        {
            lock (_lock)
            {
                return _users.AsReadOnly();
            }
        }

        public User? GetUserById(int id)
        {
            lock (_lock)
            {
                return _users.FirstOrDefault(u => u.Id == id);
            }
        }

        public void AddUser(User user)
        {
            ValidateUser(user);
            if (_users.Any(u => u.Email == user.Email))
            {
                throw new ArgumentException("A user with the same email already exists.");
            }

            lock (_lock)
            {
                user.Id = _users.Count > 0 ? _users.Max(u => u.Id) + 1 : 1;
                _users.Add(user);
                SaveToFile();
            }
        }

        public bool UpdateUser(User updatedUser)
        {
            ValidateUser(updatedUser);
            lock (_lock)
            {
                var user = _users.FirstOrDefault(u => u.Id == updatedUser.Id);
                if (user == null)
                {
                    throw new KeyNotFoundException($"User with ID {updatedUser.Id} not found.");
                }

                user.FirstName = updatedUser.FirstName;
                user.LastName = updatedUser.LastName;
                user.Email = updatedUser.Email;
                user.Department = updatedUser.Department;

                SaveToFile();
                return true;
            }
        }

        public bool DeleteUser(int id)
        {
            lock (_lock)
            {
                var user = _users.FirstOrDefault(u => u.Id == id);
                if (user == null)
                {
                    throw new KeyNotFoundException($"User with ID {id} not found.");
                }

                _users.Remove(user);
                SaveToFile();
                return true;
            }
        }
    }
}
