namespace Modules.Identity.Application;

using BridgingIT.DevKit.Common;

public interface IAccountService
{
    Task<Result> ChangePasswordAsync(string password, string newPassword, string userId);

    Task<Result> UpdateProfileAsync(string email, string firstName, string lastName, string phoneNumber, string userId);

    Task<Result<string>> GetProfilePictureAsync(string userId);

    Task<Result<string>> UpdateProfilePictureAsync(string fileName, string extension, byte[] data, string userId);
}