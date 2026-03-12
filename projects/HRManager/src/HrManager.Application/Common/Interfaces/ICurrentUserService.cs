namespace HrManager.Application.Common.Services;

public interface ICurrentUserService
{
    Guid UserId { get; }

    string Email { get; }

    string[] UserRoles { get; }
}