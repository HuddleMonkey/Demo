using Demo.Application.Domain.Settings;
using Demo.Application.Features.Authentication.Infrastructure;
using Demo.Application.Features.Authentication.Interfaces;
using Demo.Application.Features.Content.Library.Infrastructure;
using Demo.Application.Features.Content.Library.Interfaces;
using Demo.Application.Features.Events.Infrastructure;
using Demo.Application.Features.Events.Interfaces;
using Demo.Application.Features.Media.Infrastructure;
using Demo.Application.Features.Media.Interfaces;
using Demo.Application.Features.Notifications.Infrastructure;
using Demo.Application.Features.Notifications.Interfaces;
using Demo.Application.Features.Organizations.Infrastructure;
using Demo.Application.Features.Organizations.Interfaces;
using Demo.Application.Features.Payments.Infrastructure;
using Demo.Application.Features.Payments.Interfaces;
using Demo.Application.Features.Storage.Infrastructure;
using Demo.Application.Features.Storage.Interfaces;
using Demo.Application.Features.Teams.Infrastructure;
using Demo.Application.Features.Teams.Interfaces;
using Demo.Application.Features.Users.Infrastructure;
using Demo.Application.Features.Users.Interfaces;
using Demo.Application.Features.Users.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace Demo.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services, ConfigurationManager configurationManager)
    {
        var settings = configurationManager.Get<DemoSettings>() ?? new();

        services.AddMediatR(cfg => {
            cfg.RegisterServicesFromAssembly(typeof(DependencyInjection).Assembly);
            cfg.LicenseKey = settings.Keys.MediatR;
        });
        services.AddValidatorsFromAssembly(typeof(DependencyInjection).Assembly);

        return services;
    }

    public static IServiceCollection AddInfrastructure(this IServiceCollection services, ConfigurationManager configurationManager)
    {
        var settings = configurationManager.Get<DemoSettings>() ?? new();

        // Stripe
        Stripe.StripeConfiguration.ApiKey = settings.Payments.SecretKey;

        // Databases
        services.AddDbContext<DemoDbContext>(options => options
            .UseSqlServer(configurationManager.GetConnectionString("DemoDbConnection"))
            .UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking)
        );

        // Identity
        services.AddIdentity<AppUser, IdentityRole>(options =>
        {
            options.Tokens.EmailConfirmationTokenProvider = "DemoEmailConfirmation";
            options.User.RequireUniqueEmail = true;
            options.SignIn.RequireConfirmedEmail = true;
            options.Password.RequiredLength = 6;
            options.Password.RequireDigit = false;
            options.Password.RequireLowercase = false;
            options.Password.RequireNonAlphanumeric = false;
            options.Password.RequireUppercase = false;
        })
            .AddEntityFrameworkStores<DemoDbContext>()
            .AddDefaultTokenProviders()
            .AddTokenProvider<EmailConfirmationTokenProvider<AppUser>>("DemoEmailConfirmation");

        services.Configure<DataProtectionTokenProviderOptions>(opt => opt.TokenLifespan = TimeSpan.FromHours(2));
        services.Configure<EmailConfirmationTokenProviderOptions>(opt => opt.TokenLifespan = TimeSpan.FromDays(10));

        // Authentication
        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidIssuer = settings.Token.Issuer,
                    ValidateIssuer = true,
                    ValidAudience = settings.Token.Audience,
                    ValidateAudience = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(settings.Token.Key)),
                    ValidateIssuerSigningKey = true,
                    ValidateLifetime = true
                };
            });

        // Authorization
        services.AddAuthorizationBuilder()
            .AddPolicy("ManageOrganization", policy => policy.RequireAssertion(context =>
                context.User.HasClaim(c => c.Type == DemoClaimTypes.OrganizationOwner) ||
                context.User.IsInRole(DemoRoles.Admin)))
            .AddPolicy("ManageUsers", policy => policy.RequireAssertion(context =>
                context.User.HasClaim(c => c.Type == DemoClaimTypes.OrganizationOwner) ||
                context.User.HasClaim(c => c.Type == DemoClaimTypes.OrganizationAdmin) ||
                context.User.HasClaim(c => c.Type == DemoClaimTypes.TeamLeader) ||
                context.User.IsInRole(DemoRoles.Admin)))
            .AddPolicy("ManageContent", policy => policy.RequireAssertion(context =>
                context.User.HasClaim(c => c.Type == DemoClaimTypes.OrganizationOwner) ||
                context.User.HasClaim(c => c.Type == DemoClaimTypes.OrganizationAdmin) ||
                context.User.HasClaim(c => c.Type == DemoClaimTypes.TeamLeader) ||
                context.User.IsInRole(DemoRoles.Admin)))
            .AddPolicy("ManageProviders", policy => policy.RequireAssertion(context =>
                context.User.HasClaim(c => c.Type == DemoClaimTypes.Provider) ||
                context.User.IsInRole(DemoRoles.Admin)));

        // Authentication
        services.AddScoped<ITokenService, TokenService>();

        // Events
        services.AddScoped<IEventRepository, SqlEventRepository>();
        services.AddScoped<ILocationRepository, SqlLocationRepository>();
        services.AddScoped<IPositionRepository, SqlPositionRepository>();
        services.AddScoped<IScheduleRepository, SqlScheduleRepository>();

        // Library
        services.AddScoped<IProviderUserRepository, SqlProviderUserRepository>();

        // Media
        services.AddScoped<IImageService, ImageSharpImageService>();

        // Notifications
        services.AddScoped<IEmailService, AzureEmailService>();

        // Organizations
        services.AddScoped<IOrganizationRepository, SqlOrganizationRepository>();

        // Payments
        services.AddScoped<IPaymentService, StripePaymentService>();

        // Storage
        services.AddMemoryCache();
        services.AddScoped<ICacheService, MemoryCacheService>();
        services.AddScoped<IStorageService, AzureStorageService>();

        // Teams
        services.AddScoped<ITeamRepository, SqlTeamRepository>();

        // Users
        services.AddScoped<IUserRepository, IdentityUserRepository>();
        services.AddScoped<IOrganizationUserRepository, SqlOrganizationUserRepository>();

        return services;
    }
}
