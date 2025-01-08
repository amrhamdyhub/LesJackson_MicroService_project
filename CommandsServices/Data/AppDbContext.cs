﻿using CommandsServices.Models;
using Microsoft.EntityFrameworkCore;

namespace CommandsServices.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }
        public DbSet<Platform> Platforms { get; set; }
        public DbSet<Command> Commands { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Platform>
                (
                entity =>
                {
                    entity.HasMany(p => p.Commands)
                    .WithOne(p => p.Platform)
                    .HasForeignKey(p => p.PlatformId);
                }
            );

            modelBuilder.Entity<Command>
                (
                entity =>
                {
                    entity.HasOne(p => p.Platform)
                    .WithMany(p => p.Commands)
                    .HasForeignKey(p => p.PlatformId);
                }
            );
        }


    }
}
