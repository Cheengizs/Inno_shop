using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Users.Application.RepositoryInterfaces;
using Users.Domain.Models;
using Users.Infrastructure.DbContexts;
using Users.Infrastructure.Entities;

namespace Users.Infrastructure.Repositories;

public class UserRepository : IUserRepository
{
    private readonly UsersDbContext _context;
    private readonly IMapper _mapper;

    public UserRepository(UsersDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }
    
    public async Task<UserModel?> GetByIdAsync(int id)
    {
        var userFromDb = await _context.Users.FindAsync(id);
        UserModel? result = _mapper.Map<UserModel>(userFromDb);
        return result;
    }

    public async Task<UserModel?> GetByUsernameAsync(string username)
    {
        var userFromDb = await _context.Users.FirstOrDefaultAsync(u => u.Username == username);
        UserModel? result = _mapper.Map<UserModel>(userFromDb);
        return result;
    }

    public async Task<UserModel?> GetByEmailAsync(string email)
    {
        var userFromDb = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
        UserModel? result = _mapper.Map<UserModel>(userFromDb);
        return result;
    }

    public async Task<IEnumerable<UserModel>> GetAllAsync()
    {
        var usersFromDb = await _context.Users.ToListAsync();
        var result = _mapper.Map<IEnumerable<UserModel>>(usersFromDb);
        return result;
    }

    public async Task AddAsync(UserModel user)
    {
        var userToAdd = _mapper.Map<UserEntity>(user);
        await _context.Users.AddAsync(userToAdd);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(UserModel user)
    {
        var userFromDb = await _context.Users.FindAsync(user.Id);
        if (userFromDb is null) 
            return;
        
        _mapper.Map(user, userFromDb);

        await _context.SaveChangesAsync();
    }

    public async Task SetPasswordHashAsync(int userId, string passwordHash)
    {
        var user = await _context.Users.FindAsync(userId);
        if (user is null) return; 
    
        user.PasswordHash = passwordHash;
        await _context.SaveChangesAsync();
    }

    public async Task SetEmailConfirmedAsync(int userId, bool isConfirmed)
    {
        var user = await _context.Users.FindAsync(userId);
        if (user is null) return;
    
        user.EmailConfirmed = isConfirmed;
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var user = await _context.Users.FindAsync(id);
        if (user is null) return;
    
        _context.Users.Remove(user);
        await _context.SaveChangesAsync();
    }
}