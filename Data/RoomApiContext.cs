using Microsoft.EntityFrameworkCore;
using RoomBookingApi.Models;

namespace RoomBookingApi.Data{

    public class RoomApiContext(DbContextOptions<RoomApiContext> options) : DbContext(options){
        public DbSet<Room> Rooms { get; set; }
    }
}
