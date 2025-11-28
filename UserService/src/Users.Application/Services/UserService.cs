using AutoMapper;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.Extensions.Configuration;
using Users.Application.Commons;
using Users.Application.DTOs;
using Users.Application.RepositoryInterfaces;
using Users.Application.Results;
using Users.Application.Services.Interfaces;
using Users.Domain.Enums;
using Users.Domain.Models;

namespace Users.Application.Services;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;
    private readonly IValidator<RegisterRequest> _registerRequestValidator;
    private readonly IMapper _mapper;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IEmailService _emailService;
    private readonly IEmailTokenService _emailTokenService;
    private readonly IConfiguration _configuration;
    private readonly IJwtTokenService _jwtTokenService;

    public UserService(IUserRepository userRepository,
        IValidator<RegisterRequest> registerRequestValidator,
        IMapper mapper,
        IPasswordHasher passwordHasher,
        IEmailService emailService,
        IEmailTokenService emailTokenService,
        IConfiguration configuration,
        IJwtTokenService jwtTokenService)
    {
        _userRepository = userRepository;
        _registerRequestValidator = registerRequestValidator;
        _mapper = mapper;
        _passwordHasher = passwordHasher;
        _emailService = emailService;
        _emailTokenService = emailTokenService;
        _configuration = configuration;
        _jwtTokenService = jwtTokenService;
    }

    public async Task<UserServiceResult<UserResponse>> RegisterAsync(RegisterRequest request)
    {
        ValidationResult validationResult = await _registerRequestValidator.ValidateAsync(request);
        if (!validationResult.IsValid)
        {
            UserServiceResult<UserResponse> failResult =
                UserServiceResult<UserResponse>.Failure(validationResult.Errors.Select(x => x.ErrorMessage).ToList(),
                    ServiceErrorCode.Validation);

            return failResult;
        }

        var userFromRepo = await _userRepository.GetByEmailAsync(request.Email);
        if (userFromRepo != null)
        {
            UserServiceResult<UserResponse> failResult =
                UserServiceResult<UserResponse>.Failure(["This email already exists"], ServiceErrorCode.Conflict);
            return failResult;
        }

        userFromRepo = await _userRepository.GetByUsernameAsync(request.Username);
        if (userFromRepo != null)
        {
            UserServiceResult<UserResponse> failResult =
                UserServiceResult<UserResponse>.Failure(["This username already exists"], ServiceErrorCode.Conflict);
            return failResult;
        }

        string hashedPassword = _passwordHasher.Hash(request.Password);

        var userToAdd = new UserModel
        {
            Username = request.Username,
            FirstName = request.FirstName,
            LastName = request.LastName,
            Email = request.Email,
            PasswordHash = hashedPassword,
            Role = UserRole.User,
            IsActive = true,
            EmailConfirmed = false,
            CreatedAt = DateTime.UtcNow
        };

        UserModel addedUser = await _userRepository.AddAsync(userToAdd);
        UserResponse createdUser = _mapper.Map<UserResponse>(addedUser);

        var emailVerificationToken = _emailTokenService.GenerateEmailConfirmationToken(createdUser.Id);

        string _baseUrl = _configuration["AppSettings:BaseApiUrl"];
        var confirmLink =
            $"{_baseUrl}/api/auth/confirm-email?token={Uri.EscapeDataString(emailVerificationToken)}";

        var emailBody = $@"
<p>hello {addedUser.FirstName},</p>
<p>Thanks for registration! Verify your mail, tap the link:</p>
<p><a href='{confirmLink}'>verify mail</a></p>
<p>Link will expose after 24hours</p>";

        try
        {
            await _emailService.SendEmailAsync(addedUser.Email, "inno-shop email verification", emailBody);
        }
        catch (Exception e)
        {
            Console.WriteLine($"Failed to send verification email: {e.Message}");
        }

        UserServiceResult<UserResponse> successResult = UserServiceResult<UserResponse>.Success(createdUser);
        return successResult;
    }

    public async Task<UserServiceResult<LoginResponse>> LoginAsync(LoginRequest request)
    {
        var userFromRepo = await _userRepository.GetByUsernameAsync(request.Username);

        if (userFromRepo == null)
        {
            var failResult = UserServiceResult<LoginResponse>.Failure(["user with this username was not found"],
                ServiceErrorCode.NotFound);
            return failResult;
        }

        if (!_passwordHasher.Verify(request.Password, userFromRepo.PasswordHash))
        {
            return UserServiceResult<LoginResponse>.Failure(
                new List<string> { "Invalid password" },
                ServiceErrorCode.Unauthorized
            );
        }

        var accessToken = _jwtTokenService.GenerateAccessToken(userFromRepo);
        var refreshToken = _jwtTokenService.GenerateRefreshToken();
        await _userRepository.UpdateAsync(userFromRepo);

        var loginResponse = new LoginResponse(
            Token: accessToken,
            RefreshToken: refreshToken,
            UserId: userFromRepo.Id,
            Username: userFromRepo.Username,
            Email: userFromRepo.Email,
            Role: userFromRepo.Role.ToString()
        );

        var res = UserServiceResult<LoginResponse>.Success(loginResponse);
        return res;
    }

    public async Task<UserServiceResult> ValidateUserEmail(int userId)
    {
        var userFromRepo = await _userRepository.GetByIdAsync(userId);
        if (userFromRepo == null)
        {
            var failResult = UserServiceResult.Failure(["user with this email was not found"], ServiceErrorCode.NotFound);
            return failResult;
        }
        
        await _userRepository.SetEmailConfirmedAsync(userId, true);
        var result = UserServiceResult.Success();
        
        return result;
    }
}