using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using v1Remastered.Models;

public class AppDbContext : IdentityDbContext<AppUserIdentityModel>
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public DbSet<UserDetailsModel> UserDetails { get; set; }
    public DbSet<UserVaccineDetailsModel> UserVaccineDetails { get; set; }
    public DbSet<BookingDetailsModel> BookingDetails { get; set; }
    public DbSet<HospitalDetailsModel> HospitalDetails { get; set; }
}