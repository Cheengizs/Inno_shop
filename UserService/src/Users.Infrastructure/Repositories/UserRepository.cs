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
        var userFromDb = await _context.Users.FirstOrDefaultAsync(u => u.Id == id && u.IsActive);
        UserModel? result = _mapper.Map<UserModel>(userFromDb);
        return result;
    }

    public async Task<UserModel?> GetByUsernameAsync(string username)
    {
        var userFromDb = await _context.Users.FirstOrDefaultAsync(u => u.Username == username && u.IsActive);
        UserModel? result = _mapper.Map<UserModel>(userFromDb);
        return result;
    }

    public async Task<UserModel?> GetByEmailAsync(string email)
    {
        var userFromDb = await _context.Users.FirstOrDefaultAsync(u => u.Email == email && u.IsActive);
        UserModel? result = _mapper.Map<UserModel>(userFromDb);
        return result;
    }

    public async Task<IEnumerable<UserModel>> GetAllAsync(int pageNumber, int pageSize)
    {
        pageNumber = Math.Max(1, pageNumber);
        pageSize = Math.Clamp(pageSize, 1, 100);

        var usersFromDb = await _context
            .Users
            .Where(u => u.IsActive == true)
            .OrderBy(u => u.Id)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize).ToListAsync();

        return _mapper.Map<IEnumerable<UserModel>>(usersFromDb);
    }

    public async Task<IEnumerable<UserModel>> GetAllAsync()
    {
        var usersFromDb = await _context.Users.Where(u => u.IsActive).ToListAsync();
        var result = _mapper.Map<IEnumerable<UserModel>>(usersFromDb);
        return result;
    }

    public async Task<UserModel> AddAsync(UserModel user)
    {
        var userToAdd = _mapper.Map<UserEntity>(user);
        await _context.Users.AddAsync(userToAdd);
        await _context.SaveChangesAsync();
        UserModel result = _mapper.Map<UserModel>(userToAdd);
        return result;
    }

    public async Task UpdateAsync(UserModel user)
    {
        var userFromDb = await _context.Users.FirstOrDefaultAsync(u => u.Id == user.Id && u.IsActive);
        if (userFromDb is null)
            return;

        _mapper.Map(user, userFromDb);

        await _context.SaveChangesAsync();
    }

    public async Task SetPasswordHashAsync(int userId, string passwordHash)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId && u.IsActive);
        if (user is null) return;

        user.PasswordHash = passwordHash;
        await _context.SaveChangesAsync();
    }

    public async Task SetEmailConfirmedAsync(int userId, bool isConfirmed)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId && u.IsActive);
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

    public async Task UpdateStatusAsync(int id, bool isActive)
    {
        var user = await _context.Users.FindAsync(id);
        if (user != null)
        {
            user.IsActive = isActive;
            if (!isActive)
            {
                user.RefreshToken = string.Empty;
            }

            await _context.SaveChangesAsync();
        }
    }
}