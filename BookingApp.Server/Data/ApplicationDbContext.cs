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
        }
    }
}
