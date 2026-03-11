using Demo.Application.Features.Users.Models;

namespace Demo.Application.Features.Users.Interfaces;

public interface IUserRepository
{
    /// <summary>
    /// Gets users matching the ids
    /// </summary>
    /// <param name="ids">User ids to retrieve</param>
    /// <returns>List of AppUser</returns>
    Task<List<AppUser>> GetUsersAsync(List<string> ids);

    /// <summary>
    /// Gets a user by their ID.
    /// </summary>
    /// <param name="id">ID of the user</param>
    /// <returns>AppUser, or NULL if not found</returns>
    Task<AppUser?> GetUserByIdAsync(string id);

    /// <summary>
    /// Gets a user by their email.
    /// </summary>
    /// <param name="email">Email of the user</param>
    /// <returns>AppUser, or NULL if not found</returns>
    Task<AppUser?> GetUserByEmailAsync(string email);

    /// <summary>
    /// Gets the user's claims
    /// </summary>
    /// <param name="user">User</param>
    /// <returns>List of user's claims</returns>
    Task<IList<Claim>> GetUserClaimsAsync(AppUser user);

    /// <summary>
    /// Checks if a user is in the role
    /// </summary>
    /// <param name="id">ID of the user</param>
    /// <param name="role">Name of the role to check</param>
    /// <returns>True/False if user is in role</returns>
    Task<bool> IsInRoleAsync(AppUser user, string role);

    /// <summary>
    /// Generates a password reset token for the user
    /// </summary>
    /// <param name="user">User</param>
    /// <returns>Password reset token</returns>
    Task<string> GeneratePasswordResetTokenAsync(AppUser user);

    /// <summary>
    /// Resets the user's password
    /// </summary>
    /// <param name="user">The user whose password should be reset.</param>
    /// <param name="token">The password reset token to verify.</param>
    /// <param name="newPassword">The new password to set if reset token verification succeeds.</param>
    /// <returns>Result with Empty</returns>
    Task<Result<Empty>> ResetPasswordAsync(AppUser user, string token, string newPassword);

    /// <summary>
    /// Updates a user
    /// </summary>
    /// <param name="user">User to save</param>
    /// <returns>Result with Empty</returns>
    Task<Result<Empty>> UpdateUserAsync(AppUser user);
}
