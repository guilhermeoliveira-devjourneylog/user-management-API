using System.Collections.Generic;
using UserManagementAPI.Models;

namespace UserManagementAPI.Interfaces
{
    public interface IUserRepository
    {
        IEnumerable<User> GetAllUsers();
        User? GetUserById(int id);
        void AddUser(User user);
        bool UpdateUser(User updatedUser);
        bool DeleteUser(int id);
    }
}
