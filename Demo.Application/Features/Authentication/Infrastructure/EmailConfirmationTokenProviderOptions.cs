using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Demo.Application.Features.Authentication.Infrastructure;

/// <summary>
/// Defines a custom token provider to allow a special setting for the email confirmation token life span. This
/// is set with a call in Startup.cs:
/// services.Configure<EmailConfirmationTokenProviderOptions>(opt => opt.TokenLifespan = TimeSpan.FromDays(5));
/// </summary>
/// <typeparam name="TUser"></typeparam>
public class EmailConfirmationTokenProvider<TUser>(
    IDataProtectionProvider dataProtectionProvider,
    IOptions<EmailConfirmationTokenProviderOptions> options,
    ILogger<DataProtectorTokenProvider<TUser>> logger)
    : DataProtectorTokenProvider<TUser>(dataProtectionProvider, options, logger) where TUser : class
{
}

public class EmailConfirmationTokenProviderOptions : DataProtectionTokenProviderOptions
{
}
