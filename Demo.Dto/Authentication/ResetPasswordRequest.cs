using System.ComponentModel.DataAnnotations;

namespace Demo.Dto.Authentication;

/// <summary>
/// Model containing information to reset a user's password
/// </summary>
public class ResetPasswordRequest
{
    /// <summary>
    /// Email of the user
    /// </summary>
    [Required(ErrorMessage = "{0} is required")]
    [StringLength(256, ErrorMessage = "{0} must be less than {1} characters long")]
    [EmailAddress(ErrorMessage = "Please enter a valid email address")]
    [DataType(DataType.EmailAddress)]
    public string Email { get; set; } = "";

    /// <summary>
    /// New password
    /// </summary>
    [Required(ErrorMessage = "{0} is required")]
    [StringLength(256, MinimumLength = 6, ErrorMessage = "{0} must be between {2} and {1} characters long")]
    [DataType(DataType.Password)]
    [Display(Name = "New Password")]
    public string Password { get; set; } = "";

    /// <summary>
    /// Confirmed password
    /// </summary>
    [Required(ErrorMessage = "{0} is required")]
    [StringLength(256, MinimumLength = 6, ErrorMessage = "{0} must be between {2} and {1} characters long")]
    [DataType(DataType.Password)]
    [Display(Name = "Confirm Password")]
    [Compare("Password")]
    public string ConfirmPassword { get; set; } = "";

    /// <summary>
    /// Access code to allow password to be reset
    /// </summary>
    [Required(ErrorMessage = "{0} is required")]
    public string Code { get; set; } = "";
}