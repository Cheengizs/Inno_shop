using Users.Domain.Models;

namespace Users.Application.RepositoryInterfaces;

public interface IUserRepository
{
    Task<UserModel?> GetByIdAsync(int id);               
    Task<UserModel?> GetByUsernameAsync(string username); 
    Task<UserModel?> GetByEmailAsync(string email);      
    Task<IEnumerable<UserModel>> GetAllAsync(int pageNumber, int pageSize);    

    Task<UserModel> AddAsync(UserModel user); 
    
    Task UpdateAsync(UserModel user); 
    Task SetPasswordHashAsync(int userId, string passwordHash); 
    Task SetEmailConfirmedAsync(int userId, bool isConfirmed);  
    
    Task DeleteAsync(int id);
    Task UpdateStatusAsync(int id, bool isActive);
}