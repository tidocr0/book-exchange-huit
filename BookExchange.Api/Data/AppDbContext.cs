using Microsoft.EntityFrameworkCore;
using BookExchange.Api.Models;

namespace BookExchange.Api.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; } = null!;
        public DbSet<Faculty> Faculties { get; set; } = null!;
        public DbSet<Subject> Subjects { get; set; } = null!;
        public DbSet<Listing> Listings { get; set; } = null!;
        public DbSet<ListingImage> ListingImages { get; set; } = null!;
        public DbSet<Meeting> Meetings { get; set; } = null!;
        public DbSet<Review> Reviews { get; set; } = null!;
        public DbSet<Report> Reports { get; set; } = null!;
        public DbSet<SellerAvailability> SellerAvailabilities { get; set; } = null!;
        public DbSet<SellerBlackoutDate> SellerBlackoutDates { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Disable cascade deletes globally where appropriate or explicitly configure relationships
            // to match the SQL schema.

            // User - Listing
            modelBuilder.Entity<Listing>()
                .HasOne(l => l.Seller)
                .WithMany(u => u.Listings)
                .HasForeignKey(l => l.SellerId)
                .OnDelete(DeleteBehavior.Restrict);

            // Faculty - Subject
            modelBuilder.Entity<Subject>()
                .HasOne(s => s.Faculty)
                .WithMany(f => f.Subjects)
                .HasForeignKey(s => s.FacultyId);

            // Subject - Listing
            modelBuilder.Entity<Listing>()
                .HasOne(l => l.Subject)
                .WithMany()
                .HasForeignKey(l => l.SubjectId)
                .OnDelete(DeleteBehavior.Restrict);

            // Listing - ListingImage
            modelBuilder.Entity<ListingImage>()
                .HasOne(li => li.Listing)
                .WithMany(l => l.Images)
                .HasForeignKey(li => li.ListingId)
                .OnDelete(DeleteBehavior.Cascade);

            // Listing - Meeting
            modelBuilder.Entity<Meeting>()
                .HasOne(m => m.Listing)
                .WithMany(l => l.Meetings)
                .HasForeignKey(m => m.ListingId)
                .OnDelete(DeleteBehavior.Restrict);

            // User - Meeting (Buyer)
            modelBuilder.Entity<Meeting>()
                .HasOne(m => m.Buyer)
                .WithMany(u => u.MeetingsAsBuyer)
                .HasForeignKey(m => m.BuyerId)
                .OnDelete(DeleteBehavior.Restrict);

            // Meeting - Review
            modelBuilder.Entity<Review>()
                .HasOne(r => r.Meeting)
                .WithOne(m => m.Review)
                .HasForeignKey<Review>(r => r.MeetingId)
                .OnDelete(DeleteBehavior.Restrict);

            // User - Review (Reviewer)
            modelBuilder.Entity<Review>()
                .HasOne(r => r.Reviewer)
                .WithMany(u => u.ReviewsGiven)
                .HasForeignKey(r => r.ReviewerId)
                .OnDelete(DeleteBehavior.Restrict);

            // User - Review (Reviewee)
            modelBuilder.Entity<Review>()
                .HasOne(r => r.Reviewee)
                .WithMany(u => u.ReviewsReceived)
                .HasForeignKey(r => r.RevieweeId)
                .OnDelete(DeleteBehavior.Restrict);

            // User - Report (Reporter)
            modelBuilder.Entity<Report>()
                .HasOne(r => r.Reporter)
                .WithMany(u => u.ReportsMade)
                .HasForeignKey(r => r.ReporterId)
                .OnDelete(DeleteBehavior.Restrict);

            // User - Report (ReportedUser)
            modelBuilder.Entity<Report>()
                .HasOne(r => r.ReportedUser)
                .WithMany(u => u.ReportsReceived)
                .HasForeignKey(r => r.ReportedUserId)
                .OnDelete(DeleteBehavior.Restrict);

            // Listing - Report
            modelBuilder.Entity<Report>()
                .HasOne(r => r.Listing)
                .WithMany()
                .HasForeignKey(r => r.ListingId)
                .OnDelete(DeleteBehavior.Restrict);

            // SellerAvailability
            modelBuilder.Entity<SellerAvailability>()
                .ToTable("SellerAvailability")
                .HasIndex(sa => new { sa.UserId, sa.DayOfWeek, sa.TimeSlot })
                .IsUnique();

            // SellerBlackoutDate
            modelBuilder.Entity<SellerBlackoutDate>()
                .ToTable("SellerBlackoutDates")
                .HasIndex(sb => new { sb.UserId, sb.BlackoutDate, sb.TimeSlot })
                .IsUnique();
        }
    }
}
