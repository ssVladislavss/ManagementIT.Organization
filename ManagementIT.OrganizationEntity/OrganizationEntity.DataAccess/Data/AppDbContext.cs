using Microsoft.EntityFrameworkCore;
using OrganizationEntity.Core.Domain;

namespace OrganizationEntity.DataAccess.Data
{
    public class AppDbContext : DbContext
    {
        public DbSet<Building> Buildings { get; set; }
        public DbSet<Department> Departaments { get; set; }
        public DbSet<Employee> Employees { get; set; }
        public DbSet<EmployeePhoto> EmployeePhotos { get; set; }
        public DbSet<Position> Positions { get; set; }
        public DbSet<Room> Rooms { get; set; }
        public DbSet<Subdivision> Subdivisions { get; set; }

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
    }
}