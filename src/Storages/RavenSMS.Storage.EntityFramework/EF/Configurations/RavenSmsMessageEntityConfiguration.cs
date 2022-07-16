﻿namespace RavenSMS.Stores.EntityFramework;

/// <summary>
/// the entity configuration for <see cref="RavenSmsMessage"/> entity
/// </summary>
public class RavenSmsMessageEntityConfiguration : IEntityTypeConfiguration<RavenSmsMessage>
{
    /// <inheritdoc/>
    public void Configure(EntityTypeBuilder<RavenSmsMessage> builder)
    {
        builder.Property(e => e.Id)
            .HasMaxLength(17);

        builder.Property(e => e.Priority)
            .HasConversion(new EnumToStringConverter<Priority>())
            .HasMaxLength(20);

        builder.Property(e => e.Status)
            .HasConversion(new EnumToStringConverter<RavenSmsMessageStatus>())
            .HasMaxLength(20);

        builder.Property(e => e.Body)
            .HasMaxLength(500);

        builder.Property(e => e.JobQueueId)
            .HasMaxLength(100);

        builder.Property(e => e.ClientId)
            .HasMaxLength(17);

        builder.Property(e => e.ClientId)
            .HasMaxLength(17);

        builder.Property(e => e.To)
            .HasMaxLength(20);
    }
}
