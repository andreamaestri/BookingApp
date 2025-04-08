using BookingApp.Server.Model;

namespace BookingApp.Server.Data
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
        public DbSet<AvailabilityPeriod> AvailabilityPeriods { get; set; } = null!;
        public DbSet<ReviewModel> Reviews { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<BookingModel>()
                .HasOne(b => b.Accommodation)
                .WithMany(a => a.Bookings)
                .HasForeignKey(b => b.AccommodationId);

            modelBuilder.Entity<BookingModel>()
                .HasOne(b => b.Guest)
                .WithMany(g => g.Bookings)
                .HasForeignKey(b => b.GuestId);

            // Fix for multiple cascade paths in ReviewModel
            modelBuilder.Entity<ReviewModel>()
                .HasOne(r => r.Booking)
                .WithMany()
                .HasForeignKey(r => r.BookingId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<ReviewModel>()
                .HasOne(r => r.Guest)
                .WithMany()
                .HasForeignKey(r => r.GuestId)
                .OnDelete(DeleteBehavior.Restrict);
            
            // Configure decimal precision for AverageRating to address the warning
            modelBuilder.Entity<AccommodationModel>()
                .Property(a => a.AverageRating)
                .HasPrecision(3, 1);
        }
    }
}
