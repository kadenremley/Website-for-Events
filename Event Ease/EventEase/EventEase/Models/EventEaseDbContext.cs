// Kaden Jason Remley ST10472838
// CLDV6211 POE Part 1
// EventEase - Venue Booking System
//
// Description:
// Defines database tables and relationships using Entity Framework Core.
//
// References:
// Microsoft (2025) DbContext in EF Core.
// Available at: https://learn.microsoft.com/en-us/ef/core/dbcontext-configuration/
// (Accessed: 11 April 2026)
//
// Microsoft (2025) Relationships in EF Core.
// Available at: https://learn.microsoft.com/en-us/ef/core/modeling/relationships
// (Accessed: 10 April 2026)
//
// Microsoft (2025) Fluent API in Entity Framework Core.
// Available at: https://learn.microsoft.com/en-us/ef/core/modeling/
// (Accessed: 07 April 2026)
//
// Dhanampal, T. (2026) CLDV6211 lecture notes on database integration.
// The Independent Institute of Education.
using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace EventEase.Models
{
    // Database context (handles connection and mapping between models and database)
    public partial class EventEaseDbContext : DbContext
    {
        // Default constructor
        public EventEaseDbContext()
        {
        }

        // Constructor with options (used for dependency injection)
        public EventEaseDbContext(DbContextOptions<EventEaseDbContext> options)
            : base(options)
        {
        }

        // Tables in the database
        public virtual DbSet<Booking> Bookings { get; set; }
        public virtual DbSet<Event> Events { get; set; }
        public virtual DbSet<Venue> Venues { get; set; }

        // Configure database connection
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            // Connection string is handled in Program.cs
            if (!optionsBuilder.IsConfigured)
            {
                // No hardcoded connection string here
            }
        }

        // Configure table structure and relationships
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // BOOKING TABLE CONFIGURATION
            modelBuilder.Entity<Booking>(entity =>
            {
                // Set primary key
                entity.HasKey(e => e.BookingId);

                // Set table name
                entity.ToTable("Booking");

                // Set column type for date
                entity.Property(e => e.BookingDate)
                    .HasColumnType("datetime");

                // Relationship: Booking → Event (many bookings, one event)
                entity.HasOne(d => d.Event)
                    .WithMany(p => p.Bookings)
                    .HasForeignKey(d => d.EventId)
                    .OnDelete(DeleteBehavior.ClientSetNull);

                // Relationship: Booking → Venue (many bookings, one venue)
                entity.HasOne(d => d.Venue)
                    .WithMany(p => p.Bookings)
                    .HasForeignKey(d => d.VenueId)
                    .OnDelete(DeleteBehavior.ClientSetNull);
            });

            // EVENT TABLE CONFIGURATION
            modelBuilder.Entity<Event>(entity =>
            {
                // Set primary key
                entity.HasKey(e => e.EventId);

                // Set table name
                entity.ToTable("Event");

                // Limit name length
                entity.Property(e => e.Name)
                    .HasMaxLength(100);

                // Set date column types
                entity.Property(e => e.StartDate)
                    .HasColumnType("datetime");

                entity.Property(e => e.EndDate)
                    .HasColumnType("datetime");

                // Optional image URL field
                entity.Property(e => e.ImageUrl)
                    .HasMaxLength(255)
                    .IsRequired(false);
            });

            // VENUE TABLE CONFIGURATION
            modelBuilder.Entity<Venue>(entity =>
            {
                // Set primary key
                entity.HasKey(e => e.VenueId);

                // Set table name
                entity.ToTable("Venue");

                // Limit name length
                entity.Property(e => e.Name)
                    .HasMaxLength(100);

                // Limit location length
                entity.Property(e => e.Location)
                    .HasMaxLength(150);

                // Optional image URL field
                entity.Property(e => e.ImageUrl)
                    .HasMaxLength(255)
                    .IsRequired(false);
            });

            OnModelCreatingPartial(modelBuilder);
        }

        // Partial method for additional configuration
        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}