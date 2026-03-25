namespace ONGES.Campaign.Infrastructure.Persistence;

using Microsoft.EntityFrameworkCore;
using Domain.Aggregates;
using Domain.ValueObjects;
using Domain.Entities;

/// <summary>
/// DbContext for the Campaign application.
/// Configures entities and their database relationships.
/// </summary>
public sealed class CampaignDbContext : DbContext
{
    public DbSet<CampaignAggregate> Campaigns => Set<CampaignAggregate>();

    public CampaignDbContext(DbContextOptions<CampaignDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Ignore domain events - they are not stored in the database
        modelBuilder.Ignore<BaseDomainEvent>();

        ConfigureCampaignAggregate(modelBuilder);
    }

    private static void ConfigureCampaignAggregate(ModelBuilder modelBuilder)
    {
        var builder = modelBuilder.Entity<CampaignAggregate>();

        builder.ToTable("Campaigns");

        builder.HasKey(c => c.Id);

        builder.Property(c => c.Id)
            .ValueGeneratedNever();

        builder.Property(c => c.Title)
            .IsRequired()
            .HasMaxLength(255);

        builder.Property(c => c.Description)
            .IsRequired()
            .HasMaxLength(1000);

        builder.Property(c => c.FinancialTarget)
            .HasConversion(
                money => money.Amount,
                amount => new Money(amount))
            .HasColumnName("FinancialTarget")
            .HasColumnType("decimal(18,2)");

        builder.Property(c => c.AmountRaised)
            .HasColumnType("decimal(18,2)")
            .HasDefaultValue(0);

        builder.Property(c => c.Status)
            .HasConversion(
                status => status.Value,
                value => CampaignStatus.FromValue((int)value))
            .HasColumnName("Status");

        builder.Property(c => c.StartDate)
            .IsRequired();

        builder.Property(c => c.EndDate)
            .IsRequired();

        builder.Property(c => c.CreatorId)
            .IsRequired();

        builder.Property(c => c.CreatedAt)
            .IsRequired()
            .HasDefaultValueSql("GETUTCDATE()");

        builder.Property(c => c.UpdatedAt);

        builder.HasIndex(c => c.Status);
        builder.HasIndex(c => c.CreatorId);
        builder.HasIndex(c => c.CreatedAt);
    }
}
