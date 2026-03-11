using System.ComponentModel.DataAnnotations;

namespace Demo.Dto.Authentication;

/// <summary>
/// Model used to send email to user who forgot their password
/// </summary>
public class ForgotPasswordRequest
{
    /// <summary>
    /// Email to send the link to
    /// </summary>
    [Required(ErrorMessage = "Enter an email to receive the reset password request")]
    [MaxLength(256)]
    [EmailAddress]
    public string Email { get; set; } = "";
}
