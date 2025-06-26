using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;

public class AppDbContext : DbContext
{
    public DbSet<Platform> Platforms { get; set; }
    public DbSet<Well> Wells { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder options) {
        var host = Environment.GetEnvironmentVariable("DB_HOST");
        var name = Environment.GetEnvironmentVariable("DB_NAME");
        var user = Environment.GetEnvironmentVariable("DB_USER");
        var password = Environment.GetEnvironmentVariable("DB_PASSWORD");
        var port = Environment.GetEnvironmentVariable("DB_PORT");

        if (string.IsNullOrWhiteSpace(host) || string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(user) || string.IsNullOrWhiteSpace(password) || string.IsNullOrWhiteSpace(port))
        {
            throw new InvalidOperationException("Database connection string not set. Please set the DB_CONNECTION_STRING environment variable.");
        }
        
        var connectionString = $"Server={host},{port};Database={name};User Id={user};Password={password};TrustServerCertificate=true;Encrypt=False;";
        
        options.UseSqlServer(connectionString);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Platform>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).ValueGeneratedNever();
        });

        modelBuilder.Entity<Well>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).ValueGeneratedNever();
            
            entity.HasOne(e => e.Platform)
                  .WithMany(e => e.Well)
                  .HasForeignKey(e => e.PlatformId)
                  .OnDelete(DeleteBehavior.Cascade);
        });
    }

    public Platform createOrUpdatePlatform(Platform platform){
        var existingPlatform = this.Platforms.Where(p => p.Id == platform.Id).FirstOrDefault();

        if (existingPlatform == null) {
            this.Platforms.Add(platform);
            return platform;
        } 

        existingPlatform.UniqueName = platform.UniqueName;
        existingPlatform.Latitude = platform.Latitude;
        existingPlatform.Longitude = platform.Longitude;
        existingPlatform.UpdatedAt = platform.UpdatedAt;

        return existingPlatform;
    }

    public Well createOrUpdateWell(Well well){
        var existingWell = this.Wells.Where(w => w.Id == well.Id).FirstOrDefault();
        if (existingWell == null) {
            this.Wells.Add(well);
            return well;
        }

        existingWell.PlatformId = well.PlatformId;
        existingWell.UniqueName = well.UniqueName;
        existingWell.Latitude = well.Latitude;
        existingWell.Longitude = well.Longitude;
        existingWell.UpdatedAt = well.UpdatedAt;

        return existingWell;
    }
}