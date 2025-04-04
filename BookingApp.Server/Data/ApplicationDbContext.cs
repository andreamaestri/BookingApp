using BookingApp.Server.Model;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Text.Json;

namespace BookingApp.Server
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<AccommodationModel> Accommodations { get; set; } = null!;
        public DbSet<BookingModel> Bookings { get; set; } = null!;
        public DbSet<GuestModel> Guests { get; set; } = null!;
        public DbSet<ReviewModel> Reviews { get; set; } = null!;
        public DbSet<AvailabilityPeriod> AvailabilityPeriods { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Configure ICollection<AmenityType> to store as JSON
            var jsonOptions = new JsonSerializerOptions { WriteIndented = false };
            
            modelBuilder.Entity<AccommodationModel>()
                .Property(a => a.Amenities)
                .HasConversion(
                    v => JsonSerializer.Serialize(v, jsonOptions),
                    v => JsonSerializer.Deserialize<HashSet<AmenityType>>(v, jsonOptions) ?? new HashSet<AmenityType>());

            // Configure ICollection<string> to store as JSON
            modelBuilder.Entity<AccommodationModel>()
                .Property(a => a.ImageUrls)
                .HasConversion(
                    v => JsonSerializer.Serialize(v, jsonOptions),
                    v => JsonSerializer.Deserialize<List<string>>(v, jsonOptions) ?? new List<string>());

            // Configure relationships
            modelBuilder.Entity<BookingModel>()
                .HasOne(b => b.Accommodation)
                .WithMany(a => a.Bookings)
                .HasForeignKey(b => b.AccommodationId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<ReviewModel>()
                .HasOne(r => r.Booking)
                .WithOne(b => b.Review)
                .HasForeignKey<ReviewModel>(r => r.BookingId)
                .OnDelete(DeleteBehavior.Restrict);
                
            // Add relationship between ReviewModel and AccommodationModel
            modelBuilder.Entity<ReviewModel>()
                .HasOne(r => r.Accommodation)
                .WithMany(a => a.Reviews)
                .HasForeignKey(r => r.AccommodationId)
                .OnDelete(DeleteBehavior.Restrict);
                
            // Add relationship between AvailabilityPeriod and AccommodationModel
            modelBuilder.Entity<AvailabilityPeriod>()
                .HasOne(a => a.Accommodation)
                .WithMany(a => a.AvailabilityPeriods)
                .HasForeignKey(a => a.AccommodationId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
