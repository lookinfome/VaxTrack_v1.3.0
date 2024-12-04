using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using v1Remastered.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Add authentication service
builder.Services.AddTransient<IAuthService, AauthService>();

// Add account service
builder.Services.AddTransient<IAccountService, AccountService>();

// Add user profile service
builder.Services.AddTransient<IUserProfileService, UserProfileService>();

// Add user vaccine details service
builder.Services.AddTransient<IUserVaccineDetailsService, UserVaccineDetailsService>();

// Add booking details service
builder.Services.AddTransient<IBookingService, BookingService>();

// Add hospital details service
builder.Services.AddTransient<IHospitalService, HospitalService>();

// Add admin details service
builder.Services.AddTransient<IAdminService, AdminService>();

// Add user feedback service
builder.Services.AddTransient<IUserFeedbackService, UserFeedbackService>();


// Add SQLite DB
builder.Services.AddDbContext<AppDbContext>(options => 
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

// Add Identity services
builder.Services.AddIdentity<AppUserIdentityModel, IdentityRole>()
    .AddEntityFrameworkStores<AppDbContext>()
    .AddDefaultTokenProviders();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication(); // Add this line
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();