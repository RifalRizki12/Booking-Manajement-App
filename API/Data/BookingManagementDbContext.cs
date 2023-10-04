using API.Models;
using Microsoft.EntityFrameworkCore;

namespace API.Data;

public class BookingManagementDbContext : DbContext
{
    public BookingManagementDbContext(DbContextOptions<BookingManagementDbContext> options) : base(options){}
    
    public DbSet<Account> Accounts { get; set; }
    public DbSet<AccountRole> AccountRoles { get; set; }
    public DbSet<Booking> Bookings { get; set; }
    public DbSet<Education> Educations { get; set; }
    public DbSet<Employee> Employees { get; set; }
    public DbSet<Role> Roles { get; set; }
    public DbSet<Room> Rooms { get; set; }
    public DbSet<University> Universities { get; set; }
    
    //pembutan method overrid untuk atribut uniq
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Employee>().HasIndex(e => e.Nik).IsUnique();
        modelBuilder.Entity<Employee>().HasIndex(e => e.Email).IsUnique();
        modelBuilder.Entity<Employee>().HasIndex(e => e.PhoneNumber).IsUnique();

        // One University has many Educations
        modelBuilder.Entity<University>()
            .HasMany(e => e.Educations)
            .WithOne(u => u.University)
            .HasForeignKey(e => e.UniversityGuid)
             .OnDelete(DeleteBehavior.Restrict);

        // one Education has one employee
        modelBuilder.Entity<Education>()
            .HasOne(em => em.Employee)
            .WithOne(ed => ed.Education)
            .HasForeignKey<Education>(ed => ed.Guid)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Employee>()
            .HasMany(b => b.Bookings)
            .WithOne(e => e.Employee)
            .HasForeignKey(e => e.EmployeeGuid)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Booking>()
            .HasOne(r => r.Room)
            .WithMany(b => b.Bookings)
            .HasForeignKey(b => b.RoomGuid)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Employee>()
            .HasOne(a => a.Account)
            .WithOne(e => e.Employee)
            .HasForeignKey<Account>(a => a.Guid)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Account>()
            .HasMany(ar => ar.AccountRoles)
            .WithOne(e => e.Account)
            .HasForeignKey(ar => ar.AccountGuid)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<AccountRole>()
            .HasOne(r => r.Role)
            .WithMany(ar => ar.AccountRoles)
            .HasForeignKey(ar => ar.RoleGuid)
            .OnDelete(DeleteBehavior.Restrict);

    }
}