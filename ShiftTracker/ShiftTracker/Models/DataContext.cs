using Microsoft.EntityFrameworkCore;
using System.Diagnostics.CodeAnalysis;

namespace ShiftTracker.Models
{
    public class DataContext: DbContext
    {
        public DataContext(DbContextOptions<DataContext> options)
            : base(options)
        {
        }

        public DbSet<Shift> Shifts { get; set; } = null!;
        public DbSet<Shift> Location { get; set; } = null!;
    }
}
