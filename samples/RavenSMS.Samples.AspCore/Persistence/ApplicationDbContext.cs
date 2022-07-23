﻿namespace RavenSMS;

public class ApplicationDbContext : DbContext, IRavenSmsDbContext
{
    public DbSet<RavenSmsClient> RavenSmsClients { get; set; } = default!;

    public DbSet<RavenSmsMessage> RavenSmsMessages { get; set; } = default!;

    public string ConnectionString { get; }

    public ApplicationDbContext()
    {
        ConnectionString = "Server=localhost;Database=ravensms;User=root;Password=root; Port=3306";
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseMySql(ConnectionString, serverVersion: ServerVersion.AutoDetect(ConnectionString));

    protected override void OnModelCreating(ModelBuilder modelBuilder)
        => modelBuilder.ApplyRavenSmsEntityConfiguration();
}
