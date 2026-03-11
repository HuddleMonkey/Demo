using System.ComponentModel.DataAnnotations;

namespace Demo.Dto.Authentication;

/// <summary>
/// Request for logging in
/// </summary>
public class LoginRequest
{
    /// <summary>
    /// Email address/user name used to login
    /// </summary>
    [Required(ErrorMessage = "Enter an email to sign in")]
    [MaxLength(256)]
    [EmailAddress]
    public string Email { get; set; } = "";

    /// <summary>
    /// User Password
    /// </summary>
    [Required(ErrorMessage = "Enter a password to sign in")]
    [MaxLength(256)]
    [DataType(DataType.Password)]
    public string Password { get; set; } = "";
}
