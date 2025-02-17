using Microsoft.EntityFrameworkCore;
using RoomBookingApi.Models;

namespace RoomBookingApi.Data
{

    public class RoomApiContext(DbContextOptions<RoomApiContext> options) : DbContext(options)
    {
        public DbSet<Room> Rooms { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Booking> Bookings { get; set; }
        public DbSet<Guest> Guests { get; set; }
        public DbSet<Equipment> Equipments { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Guest>()
                .HasOne(g => g.Booking)
                .WithMany(b => b.Guests)
                .HasForeignKey(g => g.IdBooking);

            modelBuilder.Entity<Guest>()
                .HasOne(g => g.User)
                .WithMany(u => u.Guests)
                .HasForeignKey(g => g.IdUser);
        }
    }
}
