using Microsoft.EntityFrameworkCore;
using MyMaui.Models;

namespace MyMaui.Data
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options): base(options) { } // required constructor
        public DbSet<Item> Item { get; set; } // DbSet for users table
    }
}
