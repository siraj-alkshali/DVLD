namespace DVLD.API.Services.Interfaces;

public interface ICurrentUserService
{
    int UserID { get; }
    string UserName { get; }
}