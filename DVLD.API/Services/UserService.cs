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

namespace DVLD.API.Services;

public class UserService : IUserService
{
    private readonly DVLDContext _context;
    private readonly IPersonService _personService;
    private readonly IPasswordHasherService _passwordHasherService;
    private readonly IRoleService _roleService;

    public UserService(DVLDContext context, IPersonService personService, IPasswordHasherService passwordHasherService, IRoleService roleService)
    {
        _context = context;
        _personService = personService;
        _passwordHasherService = passwordHasherService;
        _roleService = roleService;
    }

    private async Task<User?> GetUserByIdWithDetailsAsync(int id, bool readOnly = false)
    {
        IQueryable<User> query = _context.Users
            .Include(u => u.Person)
            .Include(u => u.Role);

        if (readOnly)
            query = query.AsNoTracking();

        return await query.SingleOrDefaultAsync(u => u.UserID == id);
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
        string normalizedUserName = StringUtilities.NormalizeUserName(createUserDto.UserName);

        List<string> errors = await GetCreateUserValidationErrorsAsync(createUserDto.PersonID, normalizedUserName);

        if (errors.Count > 0)
            return ServiceResult<UserDto>.Failure(errors, FailureType.Conflict);

        int? roleId = await _roleService.GetEmployeeRoleIDAsync();

        if (roleId == null)
            return ServiceResult<UserDto>.Failure(["The role ID does not exist"], FailureType.InternalError);

        string passwordHash = _passwordHasherService.HashPassword(createUserDto.Password);

        User user = new User
        {
            PersonID = createUserDto.PersonID,
            UserName = normalizedUserName,
            PasswordHash = passwordHash,
            IsActive = true,
            RoleID = roleId.Value
        };

        _context.Users.Add(user);

        await _context.SaveChangesAsync();

        User savedUser = await _context.Users.Include(u => u.Person)
        .Include(u => u.Role)
        .SingleAsync(u => u.UserID == user.UserID);

        return ServiceResult<UserDto>.Success(savedUser.ToDto());
    }

    public async Task<ServiceResult<UserDto>> ChangeUserNameAsync(int id, ChangeUserNameDto dto)
    {
        User? user = await GetUserByIdWithDetailsAsync(id);

        if (user == null)
            return ServiceResult<UserDto>.Failure(["The requested user was not found"], FailureType.NotFound);

        string normalizedUserName = StringUtilities.NormalizeUserName(dto.UserName);
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
        User? user = await GetUserByIdWithDetailsAsync(id);

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

    public async Task<UserDto?> GetUserByIdAsync(int id)
    {
        User? user = await GetUserByIdWithDetailsAsync(id);

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
        User? user = await GetUserByIdWithDetailsAsync(id);

        if (user == null)
            return ServiceResult<UserDto>.Failure(["The requested user was not found"], FailureType.NotFound);

        if (user.IsActive == dto.IsActive)
            return ServiceResult<UserDto>.Failure([$"This user account is already {(user.IsActive ? "activated" : "deactivated")}"], FailureType.Conflict);

        user.IsActive = dto.IsActive;

        await _context.SaveChangesAsync();

        return ServiceResult<UserDto>.Success(user.ToDto());
    }
}