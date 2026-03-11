using Demo.Application.Features.Content.Library.Models;
using Demo.Application.Features.Events.Models;
using Demo.Application.Features.Organizations.Models;
using Demo.Application.Features.Teams.Models;
using Demo.Application.Features.Users.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace Demo.Application.Infrastructure.Data;

public class DemoDbContext(DbContextOptions<DemoDbContext> options) : IdentityDbContext<AppUser>(options)
{
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Events
        modelBuilder.Entity<Event>()
            .HasOne(c => c.CreatedBy)
            .WithMany()
            .HasForeignKey(c => c.CreatedByUserId);

        modelBuilder.Entity<Event>()
            .HasOne(c => c.ModifiedBy)
            .WithMany()
            .HasForeignKey(c => c.ModifiedByUserId);

        modelBuilder.Entity<ScheduleTemplate>()
            .HasOne(c => c.CreatedBy)
            .WithMany()
            .HasForeignKey(c => c.CreatedByUserId);

        modelBuilder.Entity<ScheduleTemplate>()
            .HasOne(c => c.ModifiedBy)
            .WithMany()
            .HasForeignKey(c => c.ModifiedByUserId);

        modelBuilder.Entity<Event>().ToTable("Events");
        modelBuilder.Entity<Location>().ToTable("EventLocations");
        modelBuilder.Entity<Position>().ToTable("EventPositions");
        modelBuilder.Entity<ScheduleTemplate>().ToTable("EventScheduleTemplates");
        modelBuilder.Entity<ScheduleTemplatePosition>().ToTable("EventScheduleTemplatePositions");
        modelBuilder.Entity<Series>().ToTable("EventSeries");
        modelBuilder.Entity<SeriesPart>().ToTable("EventSeriesParts");
        modelBuilder.Entity<Schedule>().ToTable("EventSchedules");

        // Library
        modelBuilder.Entity<ProviderUser>().ToTable("LibraryProviderUsers");

        // Organization
        modelBuilder.Entity<Organization>()
            .HasOne(o => o.Owner)
            .WithOne()
            .HasForeignKey<Organization>(o => o.OwnerId);

        // Teams
        modelBuilder.Entity<Team>()
            .HasMany(p => p.Members)
            .WithMany(p => p.Teams)
            .UsingEntity<Dictionary<string, object>>(
                "TeamMembers",
                j => j
                    .HasOne<AppUser>()
                    .WithMany()
                    .HasForeignKey("UserId"),
                j => j
                    .HasOne<Team>()
                    .WithMany()
                    .HasForeignKey("TeamId"));
    }

    // Events
    public DbSet<Event> Events { get; set; } = default!;
    public DbSet<Location> Locations { get; set; } = default!;
    public DbSet<Position> Positions { get; set; } = default!;
    public DbSet<ScheduleTemplate> ScheduleTemplates { get; set; } = default!;
    public DbSet<ScheduleTemplatePosition> ScheduleTemplatePositions { get; set; } = default!;
    public DbSet<Series> Series { get; set; } = default!;
    public DbSet<SeriesPart> SeriesParts { get; set; } = default!;
    public DbSet<Schedule> Schedules { get; set; } = default!;

    // Library
    public DbSet<ProviderUser> ProviderUsers { get; set; } = default!;

    // Organizations
    public DbSet<Organization> Organizations { get; set; } = default!;

    // Teams
    public DbSet<Team> Teams { get; set; } = default!;

    // Users
    public DbSet<UserClaim> UserClaimsTable { get; set; } = default!;
    public DbSet<OrganizationUser> OrganizationUsers { get; set; } = default!;
}
