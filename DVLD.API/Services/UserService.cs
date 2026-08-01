using DVLD.API.Common.Results;
using DVLD.API.DTOs.Users;
using DVLD.API.Mappings.Users;
using DVLD.API.Services.Interfaces;
using DVLD.DataAccess.Data;
using DVLD.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;

namespace DVLD.API.Services;

public class UserService : IUserService
{
    private readonly DVLDContext _context;
    private readonly IPersonService _personService;
    private readonly IPasswordHasherService _passwordHasherService;

    public UserService(DVLDContext context, IPersonService personService, IPasswordHasherService passwordHasherService)
    {
        _context = context;
        _personService = personService;
        _passwordHasherService = passwordHasherService;
    }

    private string NormalizeUserName(string userName)
    {
        return userName.Trim().ToLower();
    }

    private async Task<bool> UserExistsForPersonAsync(int personId)
    {
        return await _context.Users.AnyAsync(u => u.PersonID == personId);
    }

    private async Task<bool> UserNameExistsAsync(string userName)
    {
        return await _context.Users.AnyAsync(u => u.UserName == userName);
    }

    private async Task<bool> UserNameExistsForAnotherUserAsync(string userName, int userId)
    {
        return await _context.Users.AnyAsync(u => u.UserName == userName && u.UserID != userId);
    }

    private async Task<List<string>> GetCreateUserValidationErrorsAsync(int personId, string userName)
    {
        List<string> errors = new List<string>();

        if (!await _personService.PersonExistsAsync(personId))
            errors.Add("The specified person does not exist");

        if (await UserExistsForPersonAsync(personId))
            errors.Add("This person already has an account");

        if (await UserNameExistsAsync(userName))
            errors.Add("This username already exists");

        return errors;
    }

    public async Task<ServiceResult<UserDto>> CreateUserAsync(CreateUserDto createUserDto)
    {
        string normalizedUserName = NormalizeUserName(createUserDto.UserName);

        List<string> errors = await GetCreateUserValidationErrorsAsync(createUserDto.PersonID, normalizedUserName);

        if (errors.Count > 0)
            return ServiceResult<UserDto>.Failure(errors, FailureType.Conflict);

        string passwordHash = _passwordHasherService.HashPassword(createUserDto.Password);

        User user = new User
        {
            PersonID = createUserDto.PersonID,
            UserName = normalizedUserName,
            PasswordHash = passwordHash,
            IsActive = true
        };

        _context.Users.Add(user);

        await _context.SaveChangesAsync();

        User savedUser = await _context.Users.AsNoTracking()
        .Include(u => u.Person)
        .SingleAsync(u => u.UserID == user.UserID);

        return ServiceResult<UserDto>.Success(savedUser.ToDto());
    }

    public async Task<ServiceResult<UserDto>> ChangeUserNameAsync(int id, ChangeUserNameDto dto)
    {
        User? user = await _context.Users.Include(u => u.Person)
        .SingleOrDefaultAsync(u => u.UserID == id);

        if (user == null)
            return ServiceResult<UserDto>.Failure(["The requested user was not found"], FailureType.NotFound);

        string normalizedUserName = NormalizeUserName(dto.UserName);
        if (user.UserName == normalizedUserName)
            return ServiceResult<UserDto>.Success(user.ToDto());

        if (await UserNameExistsForAnotherUserAsync(dto.UserName, user.UserID))
            return ServiceResult<UserDto>.Failure(["This username is already taken by another user"], FailureType.Conflict);

        user.UserName = normalizedUserName;

        await _context.SaveChangesAsync();

        return ServiceResult<UserDto>.Success(user.ToDto());
    }

    public async Task<ServiceResult<UserDto>> ChangePasswordAsync(int id, ChangePasswordDto dto)
    {
        User? user = await _context.Users.Include(u => u.Person)
        .SingleOrDefaultAsync(u => u.UserID == id);

        if (user == null)
            return ServiceResult<UserDto>.Failure(["The requested user was not found"], FailureType.NotFound);

        if (!_passwordHasherService.VerifyPassword(dto.CurrentPassword, user.PasswordHash))
            return ServiceResult<UserDto>.Failure(["The current password is incorrect"], FailureType.Validation);

        if (_passwordHasherService.VerifyPassword(dto.NewPassword, user.PasswordHash))
        {
            return ServiceResult<UserDto>.Failure(
                ["The new password must be different from the current password"],
                FailureType.Validation);
        }

        string passwordHash = _passwordHasherService.HashPassword(dto.NewPassword);

        user.PasswordHash = passwordHash;

        await _context.SaveChangesAsync();

        return ServiceResult<UserDto>.Success(user.ToDto());
    }

    public async Task<IEnumerable<UserDto>> GetAllUsersAsync()
    {
        return await _context.Users.AsNoTracking()
        .Select(u => new UserDto(
            u.UserID,
            $"{u.Person.FirstName} {u.Person.LastName}",
            u.UserName
        )).ToListAsync();
    }

    public async Task<UserDto?> GetUserByIdAsync(int id)
    {
        User? user = await _context.Users.AsNoTracking()
        .Include(u => u.Person)
        .SingleOrDefaultAsync(u => u.UserID == id);

        if (user == null)
            return null;

        return user.ToDto();
    }

    public async Task<bool> UserExistsAsync(int id)
    {
        return await _context.Users.AnyAsync(u => u.UserID == id);
    }

    public async Task<ServiceResult<UserDto>> ChangeUserStatusAsync(int id, ChangeUserStatusDto dto)
    {
        User? user = await _context.Users.Include(u => u.Person)
        .SingleOrDefaultAsync(u => u.UserID == id);

        if (user == null)
            return ServiceResult<UserDto>.Failure(["The requested user was not found"], FailureType.NotFound);

        if (user.IsActive == dto.IsActive)
            return ServiceResult<UserDto>.Failure([$"This user account is already {(user.IsActive ? "activated" : "deactivated")}"], FailureType.Conflict);

        user.IsActive = dto.IsActive;

        await _context.SaveChangesAsync();

        return ServiceResult<UserDto>.Success(user.ToDto());
    }
}