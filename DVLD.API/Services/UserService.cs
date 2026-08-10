using DVLD.API.Common.Results;
using DVLD.API.DTOs.Users;
using DVLD.API.Mappings.Users;
using DVLD.API.Services.Interfaces;
using DVLD.DataAccess.Data;
using DVLD.DataAccess.Entities;
using DVLD.API.Common;
using Microsoft.EntityFrameworkCore;
using DVLD.API.Extensions;
using DVLD.API.Common.QueryParameters;
using DVLD.API.DTOs.Common;
using DVLD.API.Common.Constants;

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

    private async Task<User?> GetUserByIdWithDetailsAsync(int userId, bool readOnly = false)
    {
        IQueryable<User> query = _context.Users
        .Include(u => u.Person)
        .Include(u => u.Role);

        if (readOnly)
            query = query.AsNoTracking();

        return await query.SingleOrDefaultAsync(u => u.UserID == userId);
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

    private async Task<ServiceResult<Person>> ValidateAndGetPersonAsync(int personId, string userName)
    {
        Person? person = await _personService.GetPersonByIdAsync(personId);

        if (person == null)
            return ServiceResult<Person>.Failure(["This person does not exist"], FailureType.NotFound);

        if (await UserExistsForPersonAsync(personId))
            return ServiceResult<Person>.Failure(["This person already has a user account"], FailureType.Conflict);

        if (await UserNameExistsAsync(userName))
            return ServiceResult<Person>.Failure(["This username is already used"], FailureType.Conflict);

        return ServiceResult<Person>.Success(person);
    }

    private async Task<ServiceResult<User>> ValidateAndGetUserForUserNameChangeAsync(int userId, string normalizedUserName)
    {
        User? user = await GetUserByIdWithDetailsAsync(userId);

        if (user == null)
            return ServiceResult<User>.Failure(["This user does not exist"], FailureType.NotFound);

        if (await UserNameExistsForAnotherUserAsync(normalizedUserName, userId))
            return ServiceResult<User>.Failure(["This username has been already taken by another user"], FailureType.Conflict);

        return ServiceResult<User>.Success(user);
    }

    private async Task<ServiceResult<User>> ValidateAndGetUserForPasswordChangeAsync(int userId, ChangePasswordDto changePasswordDto)
    {
        User? user = await GetUserByIdWithDetailsAsync(userId);

        if (user == null)
            return ServiceResult<User>.Failure(["The requested user was not found"], FailureType.NotFound);

        if (!_passwordHasherService.VerifyPassword(changePasswordDto.CurrentPassword, user.PasswordHash))
            return ServiceResult<User>.Failure(["The current password is incorrect"], FailureType.ValidationError);

        if (_passwordHasherService.VerifyPassword(changePasswordDto.NewPassword, user.PasswordHash))
            return ServiceResult<User>.Failure(["The new password must be different from the current password"], FailureType.ValidationError);

        return ServiceResult<User>.Success(user);
    }

    public async Task<ServiceResult<UserDto>> CreateUserAsync(CreateUserDto createUserDto)
    {
        string normalizedUserName = StringUtilities.NormalizeUserName(createUserDto.UserName);

        ServiceResult<Person> createUserDtoValidation = await ValidateAndGetPersonAsync(createUserDto.PersonID, normalizedUserName);

        if (!createUserDtoValidation.IsSuccess)
            return ServiceResult<UserDto>.Failure(createUserDtoValidation.Errors, createUserDtoValidation.ResultType!.Value);

        Person person = createUserDtoValidation.Data!;

        string passwordHash = _passwordHasherService.HashPassword(createUserDto.Password);

        User user = createUserDto.ToEntity(normalizedUserName, passwordHash, enRoleType.Employee, person);

        _context.Users.Add(user);

        await _context.SaveChangesAsync();

        return ServiceResult<UserDto>.Success(user.ToDto());
    }

    public async Task<ServiceResult<UserDto>> ChangeUserNameAsync(int userId, ChangeUserNameDto changeUserNameDto)
    {
        string normalizedUserName = StringUtilities.NormalizeUserName(changeUserNameDto.UserName);

        ServiceResult<User> changeUserNameValidation = await ValidateAndGetUserForUserNameChangeAsync(userId, normalizedUserName);

        if (!changeUserNameValidation.IsSuccess)
            return ServiceResult<UserDto>.Failure(changeUserNameValidation.Errors, changeUserNameValidation.ResultType!.Value);

        User user = changeUserNameValidation.Data!;

        if (user.UserName == normalizedUserName)
            return ServiceResult<UserDto>.Success(user.ToDto());

        user.UserName = normalizedUserName;

        await _context.SaveChangesAsync();

        return ServiceResult<UserDto>.Success(user.ToDto());
    }

    public async Task<ServiceResult<UserDto>> ChangePasswordAsync(int userId, ChangePasswordDto changePasswordDto)
    {
        ServiceResult<User> changePasswordValidation = await ValidateAndGetUserForPasswordChangeAsync(userId, changePasswordDto);

        if (!changePasswordValidation.IsSuccess)
            return ServiceResult<UserDto>.Failure(changePasswordValidation.Errors, changePasswordValidation.ResultType!.Value);

        User user = changePasswordValidation.Data!;

        string passwordHash = _passwordHasherService.HashPassword(changePasswordDto.NewPassword);

        user.PasswordHash = passwordHash;

        await _context.SaveChangesAsync();

        return ServiceResult<UserDto>.Success(user.ToDto());
    }

    public async Task<PagedResultDto<UserDto>> GetAllUsersAsync(UsersQueryParameters parameters)
    {
        IQueryable<User> query = _context.Users.AsNoTracking()
        .Include(u => u.Person)
        .Include(u => u.Role)
        .ApplySearch(parameters.SearchTerm)
        .ApplyFilter(parameters)
        .ApplySort(parameters);

        int totalItems = await query.CountAsync();

        List<UserDto> items = await query
        .ApplyPagination(parameters)
        .Select(u => new UserDto(
            u.UserID,
            $"{u.Person.FirstName} {u.Person.LastName}",
            u.UserName,
            u.Role.RoleTitle))
            .ToListAsync();

        return new PagedResultDto<UserDto>
        {
            Items = items,
            TotalItems = totalItems,
            PageNumber = parameters.PageNumber,
            PageSize = parameters.PageSize
        };
    }

    public async Task<UserDto?> GetUserDtoByIdAsync(int userId)
    {
        User? user = await GetUserByIdWithDetailsAsync(userId);

        if (user == null)
            return null;

        return user.ToDto();
    }

    private async Task<ServiceResult<User>> ValidateAndGetUserForChangeUserStatusAsync(int userId, ChangeUserStatusDto changeUserStatusDto)
    {
        User? user = await GetUserByIdWithDetailsAsync(userId);

        if (user == null)
            return ServiceResult<User>.Failure(["The requested user was not found"], FailureType.NotFound);

        if (user.IsActive == changeUserStatusDto.IsActive)
            return ServiceResult<User>.Failure([$"This user account is already {(user.IsActive ? "activated" : "deactivated")}"], FailureType.Conflict);

        return ServiceResult<User>.Success(user);
    }

    public async Task<ServiceResult<UserDto>> ChangeUserStatusAsync(int userId, ChangeUserStatusDto changeUserStatusDto)
    {
        ServiceResult<User> changeUserStatusValidation = await ValidateAndGetUserForChangeUserStatusAsync(userId, changeUserStatusDto);

        if (!changeUserStatusValidation.IsSuccess)
            return ServiceResult<UserDto>.Failure(changeUserStatusValidation.Errors, changeUserStatusValidation.ResultType!.Value);

        User user = changeUserStatusValidation.Data!;

        user.IsActive = changeUserStatusDto.IsActive;

        await _context.SaveChangesAsync();

        return ServiceResult<UserDto>.Success(user.ToDto());
    }
}